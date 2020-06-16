using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorButton : MonoBehaviour
{
    private List<Gnome> standingGnomes;

    [SerializeField]
    private MeshRenderer mr;

    private bool pushed = false;
    public bool Pushed {
        get
        {
            return pushed;
        }
        set
        {
            pushed = value;
            UpdateColor(value ? Color.green : Color.red);
        }
    }

    void Start()
    {
        Pushed = false;
        standingGnomes = new List<Gnome>();
    }

    public void Push(Gnome gnome)
    {
        if (!standingGnomes.Contains(gnome))
            standingGnomes.Add(gnome);
        if (Pushed == false)
        {
            Pushed = true;
        }
    }
    public void Release(Gnome gnome)
    {
        standingGnomes.Remove(gnome);

        if (standingGnomes.Count == 0 && Pushed)
        {
            Pushed = false;
        }
    }
    public void UpdateColor(Color color)
    {
        mr.material.color = color;
    }

}
