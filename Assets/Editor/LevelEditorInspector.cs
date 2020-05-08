using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelEditor))]
public class ObjectGroupInspector : Editor
{
    private static Vector3 startPos;
    private static Vector3 endPos;

    void OnSceneGUI()
    {
        LevelEditor myScript = (LevelEditor)target;


        // If we are in edit mode and the user clicks (right click, middle click or alt+left click)
        if (myScript.editmode)
        {

            if (Event.current.type == EventType.MouseUp)
            { 
                if (Event.current.button == 1)
                {
                    endPos = GetSpawnPosition(new Vector3(0, 0, 0));

                    myScript.EditEvent(startPos, endPos);
                }

            } else if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 1)
                {
                    startPos = GetSpawnPosition(new Vector3(0, 0, 0));
                }
            }

            myScript.UpdateHovePosition(GetSpawnPosition(new Vector3(0, 0, 0)));

            // Mark the event as used
            Event.current.Use();

        } // End if __editMode
    } // End OnSceneGUI


    string[] _choices = new[] { "foo", "foobar" };
    int _choiceIndex = 0;
    int _oldChoiceIndex = 0;

    int orientationChoice = 0;

    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();
        LevelEditor myScript = (LevelEditor)target;
        if (GUILayout.Button("Place Rock (will delete the previous rocks!)"))
        {
            if (myScript != null)
            {
                //myScript.PlaceRocks();
            }
        }
        _choiceIndex = EditorGUILayout.Popup(_choiceIndex, toStringArray(myScript));
        if (_choiceIndex != _oldChoiceIndex)
        {
            Debug.Log("Choice index: " + _choiceIndex);
            if (_choiceIndex == 0)
            {
                myScript.SelectedObject = myScript.tileObject;
            }
            else
            {
                myScript.SelectedObject = myScript.props[_choiceIndex - 1];
            }
        }
        _oldChoiceIndex = _choiceIndex;


        orientationChoice = EditorGUILayout.Popup(orientationChoice, new string[] { "left", "right", "front", "back" });
        myScript.PropOrientation = (Orientation)orientationChoice;



        // Save the changes back to the object
        EditorUtility.SetDirty(myScript);
    }

    public string[] toStringArray(LevelEditor script)
    {
        string[] result = new string[script.props.Length + 1];
        for (int i = 0; i < script.props.Length; i++)
        {
            if (script.props[i].prefab != null)
            {
                result[i + 1] = script.props[i].prefab.name;
            }
        }

        result[0] = "floor";
        return result;
    }


    public Vector3 GetSpawnPosition(Vector3 aPlayerPos)
    {

        Plane plane = new Plane(Vector3.up, aPlayerPos);
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        float dist;
        if (plane.Raycast(ray, out dist))
        {
            //Debug.Log("ray cast dist: " + dist + ray.GetPoint(dist));
            return ray.GetPoint(dist);
        }
        return Vector3.zero;
    }

}