using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
public class Button : InteractableObject
{
    [SerializeField]
    private UnityEvent pushEvent;

    private LineRenderer lr;
    [SerializeField]
    private Transform lineTarget;

    public void Start()
    {
        scale = 0;
        lr = GetComponent<LineRenderer>();
        lr.SetPosition(1, lineTarget.transform.position - transform.position);

    }
    public override void Interact(Gnome gnome = null)
    {
        base.Interact(gnome);
        if (pushEvent != null)
        {
            pushEvent.Invoke();
        }
    }
}
