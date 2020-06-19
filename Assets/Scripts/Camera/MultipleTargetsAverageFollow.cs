using UnityEngine;
using System.Collections;

public class MultipleTargetsAverageFollow : MonoBehaviour {
    [HideInInspector]
    public Vector3 offset;

    [SerializeField]
    private float lerpSpeed = 5f;

    //the targets themselves.
    [SerializeField]
    private Transform[] targets;

    //tempPostiion is the vector3 variable that will be calculated.
    private Vector3 averagePos;

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

    [HideInInspector]
    public FocusArea focusArea; 
    public static MultipleTargetsAverageFollow instance;

    private Vector3 averagePosition(bool withAreaFocus = false)
    {
        Vector3 temp = Vector3.zero;
        for (int i = 0; i < targets.Length; i++)
        {
            temp += targets[i].position;
        }

        float amountOfOtherTargets = 0;

        if (withAreaFocus && focusArea != null)
        {
            temp += new Vector3(focusArea.transform.position.x, 0, focusArea.transform.position.z);
            amountOfOtherTargets++;
        }

        temp /= (targets.Length + amountOfOtherTargets);


        return temp;
    }
    public float furthestTargetDistance()
    {
        float distance = 0;
        for (int i = 0; i < targets.Length; i++)
        {
            distance = Mathf.Max(Vector3.Distance( averagePos, targets[i].position), distance);
        }
        if (focusArea != null)
        {
            distance = Mathf.Max(Vector3.Distance(averagePos, focusArea.transform.position), distance);
        }
        return distance;
    }

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        offset = transform.position - averagePosition();
        camera = GetComponent<Camera>();
    }

    void Update () {

        averagePos = averagePosition(true);

        //transform.position = tempPosition;
        float offsetScale = (minZoom + furthestTargetDistance() * maxZoom);
        //Debug.Log(offsetScale);
        offset = Vector3.Lerp(offset, offset.normalized * offsetScale, Time.deltaTime * zoomSpeed);
        transform.position = Vector3.Lerp(transform.position, averagePos + offset, lerpSpeed * Time.deltaTime);
        
        //camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, Mathf.Max(minZoom, Mathf.Min(maxZoom, furthestTargetDistance() * 2f)), Time.deltaTime * zoomSpeed);
        listener.position = averagePosition();
	}
}
