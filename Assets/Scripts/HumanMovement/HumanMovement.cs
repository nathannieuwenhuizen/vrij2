using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanMovement : MonoBehaviour
{

    [SerializeField]
    private Transform[] wayPoints;

    public float walkSpeed = 0.1f;
    public float rotateSpeed = 2f;

    public bool IsMoving = false;


    [SerializeField]
    private float randomOffset = 0.1f;

    private int cIndex = 0;

    public void StartPatrolling()
    {
        if (wayPoints.Length > 0)
        {
            StartCoroutine(WalkLoop(transformToPositions(wayPoints), false, cIndex));
        }
    }

    public void StartChase(Transform target)
    {
        StartCoroutine(Chasing(target));
    }

    public void StopMovement()
    {
        StopAllCoroutines();
    }


    Vector3[] transformToPositions(Transform[] transforms)
    {
        Vector3[] list = new Vector3[transforms.Length];

        for (int i = 0; i < transforms.Length; i++)
        {
            list[i] = transforms[i].position;
        }
        return list;
    }
    public IEnumerator Orienting(Vector3 destination)
    {
        Vector3 newDirection;

        while (Mathf.Abs(Vector2.Angle(ToXZVector(transform.forward), ToXZVector(destination - transform.position) )) > 1f)
        {
            newDirection = Vector3.RotateTowards(transform.forward, destination - transform.position, Time.deltaTime * rotateSpeed, 0.0f);
            newDirection.y = 0;

            transform.rotation = Quaternion.LookRotation(newDirection);
            yield return new WaitForFixedUpdate();
        }
        newDirection = Vector3.RotateTowards(transform.forward, destination - transform.position, 1f, 0.0f);
        newDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(newDirection);

    }
    public Vector2 ToXZVector( Vector3 input)
    {
        return new Vector2(input.x, input.z);
    }
    public IEnumerator Walking(Vector3 destination)
    {
        while (Vector2.Distance(ToXZVector(transform.position), ToXZVector(destination)) > 0.3f)
        {
            transform.Translate(transform.InverseTransformDirection(transform.forward) * walkSpeed);
            yield return new WaitForFixedUpdate();
        }
        transform.position = new Vector3(destination.x, transform.position.y, destination.z); 
    }

    public IEnumerator WalkLoop(Vector3[] positions, bool invert, int startIndex)
    {
        int index = startIndex;
        Vector3 offset = new Vector3(( Random.value  - 0.5f) * randomOffset, 0, (Random.value - 0.5f) * randomOffset);
        IsMoving = true;

        yield return StartCoroutine(Orienting(positions[index] + offset * 2f));
        IsMoving = true;

        yield return StartCoroutine(Walking(positions[index] + offset * 2f));

        IsMoving = true;
        StopAllCoroutines();
        index += invert ? -1 : 1;
        index = (index + positions.Length) % positions.Length;
        cIndex = index;
        StartCoroutine(WalkLoop(positions, invert, index));

    }

    public IEnumerator Chasing(Transform target)
    {
        Vector3 newDirection;

        while (true)
        {
            Vector3 delta = target.position - transform.position;
            delta.y = 0;
            newDirection = Vector3.RotateTowards(transform.forward,  delta, Time.deltaTime * rotateSpeed, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            transform.Translate(transform.InverseTransformDirection(transform.forward) * walkSpeed);
            yield return new WaitForFixedUpdate();
        }
    }

    public IEnumerator GoTo(Vector3 pos)
    {
        yield return StartCoroutine(Orienting(pos));
        yield return StartCoroutine(Walking(pos));
    }

    public void Search(Vector3 pos, float angle, float searchDuration)
    {
        StartCoroutine(Searching(pos, angle, searchDuration));
    }
    public IEnumerator Searching(Vector3 pos, float angle, float searchDuration)
    {
        IsMoving = true;
        yield return StartCoroutine(GoTo(pos));
        yield return StartCoroutine(LookingAround(angle, searchDuration));
        IsMoving = false;
        Debug.Log("is moving is false");
    }
    public IEnumerator LookingAround(float angle, float searchDuration)
    {
        float startRotation = transform.rotation.y;
        float index = 0;
        while (index < searchDuration)
        {
            index += Time.deltaTime;
            transform.rotation = Quaternion.Euler( new Vector3(0, startRotation + (angle / 2f + Mathf.Sin(index /searchDuration *Mathf.PI * 2) * angle), 0));
            yield return new WaitForFixedUpdate();
            Debug.Log("index: " + index); 

        }
    }

    void OnDrawGizmosSelected()
    {

        if (IsMoving) return;
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;

        for (int i = 0; i < wayPoints.Length; i++)
        {
            Gizmos.DrawSphere(wayPoints[i].position, .1f);
            Gizmos.DrawLine(wayPoints[i].position, wayPoints[(i + 1) % wayPoints.Length].position);
        }
    }

}
