using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Human : MonoBehaviour
{

    [Range(0,180)]
    [SerializeField]
    private float detectionAngle = 90f;
    [Range(1,10)]
    [SerializeField]
    private float detectionRange = 2f;

    [SerializeField]
    private Light spotLight;

    [SerializeField]
    private GnomeMovement[] gnomes;

    private void Start()
    {
        gnomes = FindObjectsOfType<GnomeMovement>();
    }

    private void Update()
    {

        spotLight.color = detectedGnome() != null ? Color.red : Color.yellow;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 rightLine = Vector3.Lerp(transform.forward, -transform.right, detectionAngle / 180).normalized * detectionRange;
        Vector3 leftLine = Vector3.Lerp(transform.forward, transform.right, detectionAngle / 180).normalized * detectionRange;

        UnityEditor.Handles.DrawWireArc(transform.position, transform.up, rightLine, detectionAngle, detectionRange);
        UnityEditor.Handles.DrawLine(transform.position, transform.position + rightLine);
        UnityEditor.Handles.DrawLine(transform.position, transform.position + leftLine);
    }

    GnomeMovement detectedGnome()
    {
        GnomeMovement result = null;

        foreach(GnomeMovement gnome in gnomes)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, gnome.transform.position - transform.position, out hit, detectionRange))
            {
                if (Vector3.Angle(transform.forward, hit.transform.position - transform.position) < detectionAngle / 2)
                {
                    if (hit.transform.GetComponent<GnomeMovement>() != null)
                    {
                        result = hit.transform.GetComponent<GnomeMovement>();
                    }
                }
            }
        } 
        return result;
    }


    //editor only
    public void UpdateLight()
    {
        if (spotLight == null) return;
        spotLight.spotAngle = detectionAngle;
        spotLight.range = detectionRange;
    }
}
