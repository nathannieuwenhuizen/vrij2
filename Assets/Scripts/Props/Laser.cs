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

    private float laserWidth;
    [SerializeField]
    private float minLaserWidth;
    [SerializeField]
    private float maxLaserWidth;

    [SerializeField]
    private FloorButton button;
    [SerializeField]
    private bool reverse = false;
    private bool isOn = true;

    [SerializeField]
    private AudioSource audioS;

    void Start()
    {
        lr = GetComponent<LineRenderer>();

        if (audioS != null)
        {
            audioS.volume = 1f * Settings.SFX;
        }
    }

    public Gnome RaycastGnome()
    {
        Gnome result = null;
        RaycastHit hit;

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
        {
            if (hit.collider.GetComponent<Gnome>() != null)
            {
                result = hit.collider.GetComponent<Gnome>();
            } 
            lr.SetPosition(0,  transform.position);
            lr.SetPosition(1,  hit.point);
            //Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.yellow);
        }
        return result;
            
    }

    void CheckButton()
    {
        if (button != null)
        {
            isOn = reverse ? !button.Pushed : button.Pushed;
        }
    }
    void Update()
    { 

        CheckButton();

        if (isOn)
        {
            lr.enabled = true;

            laserWidth = minLaserWidth + Mathf.Sin(Time.deltaTime) * (maxLaserWidth - minLaserWidth);
            lr.startWidth = lr.endWidth = laserWidth;

            if (RaycastGnome() != null && hitEvent != null)
            {
                hitEvent.Invoke();
            }
        } else
        {
            lr.enabled = false;
        }
    }

}
