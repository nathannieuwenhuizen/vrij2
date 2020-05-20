using UnityEngine;
using System.Collections;

public class MultipleTargetsAverageFollow : MonoBehaviour {
    private Vector3 offset;

    [SerializeField]
    private float lerpSpeed = 5f;

    //the targets themselves.
    [SerializeField]
    private Transform[] targets;

    //tempPostiion is the vector3 variable that will be calculated.
    private Vector3 tempPosition;

    private Camera camera;

    [Header("zoom info")]
    [SerializeField]
    private float maxZoom = 10;
    [SerializeField]
    private float minZoom = 2;
    [SerializeField]
    private float zoomSpeed = 1f;

    [SerializeField]
    private Transform listener;

    public Vector3 averagePosition()
    {
        Vector3 temp = Vector3.zero;
        for (int i = 0; i < targets.Length; i++)
        {
            temp += targets[i].position;
        }
        temp /= targets.Length;
        return temp;
    }

    public float furthestTargetDistance()
    {
        float distance = 0;
        for (int i = 0; i < targets.Length; i++)
        {
            distance = Mathf.Max(Vector3.Distance( tempPosition - offset, targets[i].position), distance);
        }
        return distance;
    }

    private void Start()
    {
        offset = transform.position - averagePosition();
        camera = GetComponent<Camera>();
    }

    void Update () {

        tempPosition = averagePosition() + offset;

        //transform.position = tempPosition;
        transform.position = Vector3.Lerp(transform.position, tempPosition, lerpSpeed * Time.deltaTime);

        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, Mathf.Max(minZoom, Mathf.Min(maxZoom, furthestTargetDistance() * 2f)), Time.deltaTime * zoomSpeed);
        listener.position = averagePosition();
	}
}
