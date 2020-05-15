using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class HumanMovement : MonoBehaviour
{

    [SerializeField]
    private Transform[] wayPoints;


    public bool IsMoving = false;


    [SerializeField]
    private float randomOffset = 0.1f;

    private NavMeshAgent agent;
    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }


    public void StartPatrolling()
    {
        if (wayPoints.Length > 0)
        {
            StartCoroutine(WalkLoop(transformToPositions(wayPoints), false, ClosestWayPointIndex()));
        }
    }

    public int ClosestWayPointIndex()
    {
        float distance = Mathf.Infinity;
        int result = 0;
        for(int i = 0; i < wayPoints.Length; i++)
        {
            float cdist = Vector3.Distance(wayPoints[i].position, transform.position);
            if (cdist < distance)
            {
                distance = cdist;
                result = i;
            }
        }
        return result;
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

    public Vector2 ToXZVector( Vector3 input)
    {
        return new Vector2(input.x, input.z);
    }

    public IEnumerator WalkLoop(Vector3[] positions, bool invert, int startIndex)
    {
        int index = startIndex;
        Vector3 offset = new Vector3(( Random.value  - 0.5f) * randomOffset, 0, (Random.value - 0.5f) * randomOffset);
        IsMoving = true;
        yield return GoTo(positions[index] + offset * 2f);
        IsMoving = true;

        StopAllCoroutines();
        index += invert ? -1 : 1;
        index = (index + positions.Length) % positions.Length;
        StartCoroutine(WalkLoop(positions, invert, index));

    }

    public IEnumerator Chasing(Transform target)
    {
        Vector3 newDirection;
        agent.enabled = true;

        while (true)
        {
            agent.SetDestination(target.position);
            yield return new WaitForFixedUpdate();
        }
    }

    public IEnumerator GoTo(Vector3 pos)
    {
        agent.enabled = true;
        agent.SetDestination(pos);

        while (Vector2.Distance(ToXZVector(transform.position), ToXZVector(pos)) > 0.3f)
        {
            yield return new WaitForFixedUpdate();
        }
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
    }
    public IEnumerator LookingAround(float angle, float searchDuration)
    {
        agent.enabled = false;
        float startRotation = transform.rotation.eulerAngles.y;
        float index = 0;
        while (index < searchDuration)
        {
            index += Time.deltaTime;
            transform.rotation = Quaternion.Euler( new Vector3(0, startRotation + (angle / 2f + Mathf.Sin(index /searchDuration *Mathf.PI * 2) * angle), 0));
            yield return new WaitForFixedUpdate();
            Debug.Log("index: " + index); 

        }
        agent.enabled = true;

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
