using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Human))]
public class HumanEditor : Editor
{
    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();
        Human myScript = (Human)target;
        //if (GUILayout.Button("Place Rock (will delete the previous rocks!)"))
        //{
        //    if (myScript != null)
        //    {
        //        myScript.PlaceRocks();
        //    }
        //}
        myScript.UpdateLight();

    }

}
