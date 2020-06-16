using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusArea : MonoBehaviour
{

    private List<Gnome> gnomesInField;


    private void Start()
    {
        gnomesInField = new List<Gnome>();
    }
    public void Focus(Gnome gnome)
    {
        gnomesInField.Add(gnome);
        if (gnomesInField.Count >= 2)
        {
            MultipleTargetsAverageFollow.instance.focusArea = this;
        }
    }
    public void Unfocus(Gnome gnome)
    {
        MultipleTargetsAverageFollow.instance.focusArea = null;
        gnomesInField.Remove(gnome);
    }
}
