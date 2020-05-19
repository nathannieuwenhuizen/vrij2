using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crown : ArtWork
{
    public override void Interact(Gnome gnome = null)
    {
        base.Interact(gnome);
        GameManager.instance.CrownIsStolen();
        //gameObject.SetActive(false);
    }
}
