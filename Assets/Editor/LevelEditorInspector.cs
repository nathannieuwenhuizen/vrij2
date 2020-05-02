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

            myScript.UpdateHover(GetSpawnPosition(new Vector3(0, 0, 0)));

            // Mark the event as used
            Event.current.Use();

        } // End if __editMode
    } // End OnSceneGUI



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