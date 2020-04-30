using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrenchCoat : InteractableObject
{
    private bool isWorn = false;
    public override void Interact(GnomeMovement gnome = null)
    {
        base.Interact(gnome);
        Wear(gnome);
    }

    public void Wear(GnomeMovement gnome)
    {
        if (gnome == null) return;
        if (gnome.playerAboveMe != null) { Wear(gnome.playerAboveMe); return; }

        transform.parent = gnome.transform;
        gnome.TrenchCoat = this;
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
    public void TakeOff(GnomeMovement gnome, bool _popupIsActive = false)
    {
        transform.parent = null;
        gnome.TrenchCoat = null;
        popupIsActive = _popupIsActive;
    }
}
