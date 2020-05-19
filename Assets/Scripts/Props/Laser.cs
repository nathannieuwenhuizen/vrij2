using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{

    private LineRenderer lr;

    [SerializeField]
    private UnityEvent hitEvent;


    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    public Gnome RaycastGnome()
    {
        Gnome result = null;
        RaycastHit hit;

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 100))
        {
            if (hit.collider.GetComponent<Gnome>() != null)
            {
                result = hit.collider.GetComponent<Gnome>();
            }
            lr.SetPosition(1, hit.point - transform.position);
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.yellow);
        }
        return result;
            
    }

    void Update()
    {
        if (RaycastGnome() != null && hitEvent != null)
        {
            hitEvent.Invoke();
        }
    }
}
