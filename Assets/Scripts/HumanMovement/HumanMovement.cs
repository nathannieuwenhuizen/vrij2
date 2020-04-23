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

    private void Start()
    {
        if (wayPoints.Length > 0)
        {
            StartCoroutine(WalkLoop(transformToPositions(wayPoints), false, 0));
        }
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

        while (Mathf.Abs(Vector3.Angle(transform.forward, destination - transform.position)) > 1f)
        {
            newDirection = Vector3.RotateTowards(transform.forward, destination - transform.position, Time.deltaTime * rotateSpeed, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            yield return new WaitForFixedUpdate();
        }
        newDirection = Vector3.RotateTowards(transform.forward, destination - transform.position, 1f, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

    }
    public IEnumerator Walking(Vector3 destination)
    {
        while (Vector3.Distance(transform.position, destination) > 0.3f)
        {
            transform.Translate(transform.InverseTransformDirection(transform.forward) * walkSpeed);
            yield return new WaitForFixedUpdate();
        }
        transform.position = destination;
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
        StartCoroutine(WalkLoop(positions, invert, index));

    }

    public IEnumerator GoTo(Vector3 pos, bool destoryWhenFinish = false)
    {
        if (!IsMoving)
        {
            IsMoving = true;
            yield return StartCoroutine(Orienting(pos));
            yield return StartCoroutine(Walking(pos));
            IsMoving = false;
        }
        if (destoryWhenFinish)
        {
            Destroy(this.gameObject);
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
