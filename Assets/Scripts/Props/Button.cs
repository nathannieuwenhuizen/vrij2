using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button : InteractableObject
{
    [SerializeField]
    private UnityEvent pushEvent;

    public override void Interact(Gnome gnome = null)
    {
        base.Interact(gnome);
        if (pushEvent != null)
        {
            pushEvent.Invoke();
        }
    }
}
