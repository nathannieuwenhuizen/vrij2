using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crown : ArtWork
{
    public override void Interact(Gnome gnome = null)
    {
        goesToHead = true;
        base.Interact(gnome);
        gnome.HasCrown = true;
        GameManager.instance.CrownIsStolen(gnome);
    }
} 
