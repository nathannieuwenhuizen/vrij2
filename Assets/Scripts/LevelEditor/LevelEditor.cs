﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public enum ClickEvent
{
    add,
    remove
}
public enum Orientation
{
    left,
    right, 
    forth,
    back
}


public class LevelEditor : MonoBehaviour
{
    public bool editmode = true;
    public ClickEvent clickevent;

    private float propRotation = 0;
    private Orientation propOrientation = Orientation.forth;


    [SerializeField]
    private bool PropAtWall = false;
    [Range(0, 1f)]
    [SerializeField]
    private float distanceToWall = 0.8f;

    public EditorObject[] props;

    [SerializeField]
    public EditorObject tileObject;
    [SerializeField]
    private EditorObject wallObject;
    [SerializeField]
    private EditorObject pillarObject;

    [SerializeField]
    private GameObject hoverObject;

    [SerializeField]
    private Material hoverMaterial;

    [SerializeField]
    private EditorObject selectedObject;



    [SerializeField]
    private int tileSize = 5;

    [SerializeField]
    public int Size
    {
        get { return tileSize; }
        set { tileSize = value; }
    }

    public EditorObject SelectedObject
    {
        get { return selectedObject; }
        set {
            selectedObject = value;
            UpdateHoverObject();

        }
    }

    public Orientation PropOrientation
    {
        get { return propOrientation; }
        set {
            propOrientation = value;
            float rotation = 0;
            switch (propOrientation)
            {
                case Orientation.left:
                    rotation = -90;
                    break;
                case Orientation.right:
                    rotation = 90;
                    break;
                case Orientation.back:
                    rotation = 180;
                    break;
            }
            propRotation = rotation;
            if (hoverObject != null) {
                hoverObject.transform.rotation = Quaternion.Euler(new Vector3(0, propRotation, 0));
            }
        }
    }

    public void UpdateHoverObject()
    {
        if (hoverObject != null)
        {
            DestroyImmediate(hoverObject.gameObject);
            hoverObject = null;
        }
        if (hoverObject == null)
        {
            hoverObject = Instantiate(selectedObject.prefab, transform);
            CheckHover();
        }
    }

    public void UpdateList(EditorObject obj)
    {
        obj.FindParent(this);

        List<GameObject> list = new List<GameObject>();
        Transform[] children = obj.parent.GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] != obj.parent)
            {
                list.Add(children[i].gameObject);
            }
        }
    }

    public void EditEvent(Vector3 startPos, Vector3 endPos)
    {
        UpdateList(tileObject);
        UpdateList(wallObject);
        UpdateList(pillarObject);
        UpdateList(selectedObject);


        //round at position;
        startPos = new Vector3(Mathf.Round(startPos.x / tileSize) * tileSize, Mathf.Round(startPos.y / tileSize) * tileSize, Mathf.Round(startPos.z / tileSize) * tileSize);
        endPos = new Vector3(Mathf.Round(endPos.x / tileSize) * tileSize, Mathf.Round(endPos.y / tileSize) * tileSize, Mathf.Round(endPos.z / tileSize) * tileSize);

        if (selectedObject == tileObject)
        {
            Vector3[] grid = gridPositions(startPos, endPos);
            if (clickevent == ClickEvent.add)
            {
                foreach (Vector3 tilePos in grid)
                {
                    SpawnObject(tileObject, tilePos);
                }
                UpdateWalls(grid);

            }
            if (clickevent == ClickEvent.remove)
            {
                foreach (Vector3 tilePos in grid)
                {
                    RemoveObject(tileObject, tilePos);
                }
                UpdateWallsWhenRemove(grid);
            }
        }
        else
        {
            if (clickevent == ClickEvent.add)
            {
                SpawnObject(selectedObject, PropAtWall ? hoverObject.transform.position : endPos, PropAtWall ? hoverObject.transform.rotation.eulerAngles.y : propRotation);
            }
            if (clickevent == ClickEvent.remove)
            {
                RemoveObject(selectedObject, PropAtWall ? hoverObject.transform.position : endPos);
            }

        }

    }

    public void UpdateHovePosition(Vector3 position)
    {
        CheckHover();
        Vector3 roundedUpPos = new Vector3(
            Mathf.Round(position.x / tileSize) * tileSize, 
            Mathf.Round(position.y / tileSize) * tileSize, 
            Mathf.Round(position.z / tileSize) * tileSize);

        hoverObject.transform.position = roundedUpPos;
        if (PropAtWall && selectedObject != tileObject)
        {
            float closestDistance = tileSize * 200f;
            //GameObject wallToAttach = null;
            Vector3 positionTOAttach = position;
            Vector3[] positions = {
            new Vector3(0, 0, -tileSize /2f),
            new Vector3(-tileSize /2f, 0, 0),
            new Vector3(0, 0, tileSize / 2f),
            new Vector3(tileSize /2f, 0, 0) };

            int cIndex = 0;
            int rotationIndex = 0;
            foreach (Vector3 pos in positions)
            {
                float dist = Vector3.Distance(position, roundedUpPos + pos);
                if (dist < closestDistance) 
                {

                    closestDistance = dist;
                    //wallToAttach = wall;
                    positionTOAttach = roundedUpPos + pos;
                    rotationIndex = cIndex;
                }
                cIndex++;


            }
                hoverObject.transform.position =  roundedUpPos + (positionTOAttach - roundedUpPos) * distanceToWall;
                hoverObject.transform.rotation =Quaternion.Euler(0,90 * rotationIndex ,0);
        }
        else {
            hoverObject.transform.rotation = hoverObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

    }
    public void CheckHover()
    {
        if (hoverObject == null)
        {
            return;
        }
        hoverObject.GetComponentInChildren<MeshRenderer>().material = hoverMaterial;
        if (clickevent == ClickEvent.remove)
        {
            hoverObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.color = new Color(1, 0, 0, .5f);
        } else if (clickevent == ClickEvent.add)
        {
            hoverObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.color = new Color(0, 1, 0, .5f);
        }
        hoverObject.name = "hover";

    }

    public Transform createEMptyGameObject(string name)
    {
        GameObject newObj = new GameObject(name);
        newObj.transform.parent = transform;
        return newObj.transform;
    }

    public void SpawnObject(EditorObject obj, Vector3 position, float rotation = 0)
    {
        //Debug.Log("Spawn Obj");
        if (DetectedObject(obj, position) != null) { return; } //check if object is already placed there somehow.

        //instantiatePrefab
        #if (UNITY_EDITOR)
        GameObject prefab_instance = PrefabUtility.InstantiatePrefab(obj.prefab as GameObject) as GameObject;
        prefab_instance.transform.parent = obj.parent;
        prefab_instance.transform.position = position;

        prefab_instance.transform.Rotate(new Vector3(0, rotation, 0));
        //add to list
        obj.list.Add(prefab_instance);
        #endif
    }

    public void RemoveObject(EditorObject obj, Vector3 position)
    {
        GameObject selectedObj = DetectedObject(obj, position);
        if (selectedObj != null)
        {
            DestroyImmediate(selectedObj.gameObject);
            obj.list.Remove(selectedObj);
        }
    }


    public void RemoveSurroundingWalls(Vector3 pos)
    {
        RemoveObject(wallObject, pos + new Vector3(-tileSize / 2f, 0, 0));
        RemoveObject(wallObject, pos + new Vector3(tileSize / 2f, 0, 0));
        RemoveObject(wallObject, pos + new Vector3(0, 0, -tileSize / 2f));
        RemoveObject(wallObject, pos + new Vector3(0, 0, tileSize / 2f));

        RemoveObject(pillarObject, pos + new Vector3(-tileSize / 2f, 0, -tileSize /2f));
        RemoveObject(pillarObject, pos + new Vector3(tileSize / 2f, 0, -tileSize /2f));
        RemoveObject(pillarObject, pos + new Vector3(-tileSize / 2f, 0, tileSize /2f));
        RemoveObject(pillarObject, pos + new Vector3(tileSize / 2f, 0, tileSize /2f));
    }

    public void UpdateNeighBourPillars(Vector3 position)
    {
        bool originIsNull = DetectedObject(tileObject, position) == null;
        Vector3[] positions = {
            new Vector3(0, 0, tileSize),
            new Vector3(0, 0, -tileSize),
            new Vector3(tileSize, 0, 0),
            new Vector3(-tileSize, 0, 0),

            new Vector3(tileSize, 0, tileSize),
            new Vector3(-tileSize, 0, tileSize),
            new Vector3(tileSize, 0, -tileSize),
            new Vector3(-tileSize, 0, -tileSize),
        };

        foreach(Vector3 pos in positions)
        {
            bool directionIsNull = DetectedObject(tileObject, position + pos) == null;
            if (directionIsNull != originIsNull)
            {
                bool left = false;
                bool up = false;
                bool right = false;
                bool down = false;
                if (DetectedObject(tileObject, position + pos + new Vector3(-tileSize, 0, 0)) == null)
                {
                    left = true;
                }
                if (DetectedObject(tileObject, position + pos + new Vector3(tileSize, 0, 0)) == null)
                {
                    right = true;
                }
                if (DetectedObject(tileObject, position + pos + new Vector3(0, 0, -tileSize)) == null)
                {
                    down = true;
                }
                if (DetectedObject(tileObject, position + pos + new Vector3(0, 0, tileSize)) == null)
                {
                    up = true;
                }

                //spawn pillars
                if (left == up && left != directionIsNull)
                {
                    SpawnObject(pillarObject, position + pos + new Vector3(-tileSize / 2f, 0, tileSize / 2f));
                }
                if (left == down && left != directionIsNull)
                {
                    SpawnObject(pillarObject, position + pos + new Vector3(-tileSize / 2f, 0, -tileSize / 2f));
                }
                if (right == up && right != directionIsNull)
                {
                    SpawnObject(pillarObject, position + pos + new Vector3(tileSize / 2f, 0, tileSize / 2f));
                }
                if (right == down && right != directionIsNull)
                {
                    SpawnObject(pillarObject, position + pos + new Vector3(tileSize / 2f, 0, -tileSize / 2f));
                }


            }
        }
    }

    public void UpdateWalls(Vector3[] positions)
    {
        foreach (Vector3 pos in positions)
        {
            RemoveSurroundingWalls(pos);
            //left
            bool left = false;
            bool up = false;
            bool right = false;
            bool down = false;

            if (pos.x == positions[0].x)
            {
                if (DetectedObject(tileObject, pos + new Vector3(-tileSize, 0, 0)) == null){
                    left = true;
                    SpawnObject(wallObject, pos + new Vector3(-tileSize / 2f, 0, 0), 90);
                }

            }
            //right
            if (pos.x == positions[positions.Length - 1].x)
            {
                if (DetectedObject(tileObject, pos + new Vector3(tileSize, 0, 0)) == null){
                    right = true;
                    SpawnObject(wallObject, pos + new Vector3(tileSize / 2f, 0, 0) , -90);
                }
            }
            //bottom 
            if (pos.z == positions[0].z)
            {
                if (DetectedObject(tileObject, pos + new Vector3(0, 0, -tileSize)) == null){
                    down = true;
                    SpawnObject(wallObject, pos + new Vector3(0, 0, -tileSize / 2f) , 180);
                }
            }
            //top
            if (pos.z == positions[positions.Length - 1].z)
            {
                if (DetectedObject(tileObject, pos + new Vector3(0, 0, tileSize)) == null) {
                    up = true;
                    SpawnObject(wallObject, pos + new Vector3(0, 0, tileSize / 2f));
                }
            }

            //spawn pillars
            if (left == true && up == true)
            {
                SpawnObject(pillarObject, pos + new Vector3(-tileSize /2f, 0, tileSize /2f));
            }
            if (left == true && down == true)
            {
                SpawnObject(pillarObject, pos + new Vector3(-tileSize /2f, 0, -tileSize /2f));
            }
            if (right == true && up == true)
            {
                SpawnObject(pillarObject, pos + new Vector3(tileSize /2f, 0, tileSize /2f));
            }
            if (right == true && down == true)
            {
                SpawnObject(pillarObject, pos + new Vector3(tileSize /2f, 0, -tileSize /2f));
            }

            //spawnneighbourPillars
            if (left || right || up || down)
            {
                UpdateNeighBourPillars(pos);
            }
        }
    }
    public void UpdateWallsWhenRemove(Vector3[] positions)
    {
        foreach (Vector3 pos in positions)
        {
            RemoveSurroundingWalls(pos);


            bool left = false;
            bool up = false;
            bool right = false;
            bool down = false;


            //left
            if (pos.x == positions[0].x)
            {
                if (DetectedObject(tileObject, pos + new Vector3(-tileSize, 0, 0)) != null){ left = true; SpawnObject(wallObject, pos + new Vector3(-tileSize / 2f, 0, 0), 90); }
            }
            //right
            if (pos.x == positions[positions.Length - 1].x)
            {
                if (DetectedObject(tileObject, pos + new Vector3(tileSize, 0, 0)) != null){ right = true; SpawnObject(wallObject, pos + new Vector3(tileSize / 2f, 0, 0) , -90); }
            }
            //bottom 
            if (pos.z == positions[0].z)
            {
                if (DetectedObject(tileObject, pos + new Vector3(0, 0, -tileSize)) != null){ down = true; SpawnObject(wallObject, pos + new Vector3(0, 0, -tileSize / 2f) , 180); }
            }
            //top
            if (pos.z == positions[positions.Length - 1].z)
            {
                if (DetectedObject(tileObject, pos + new Vector3(0, 0, tileSize)) != null) { up = true;  SpawnObject(wallObject, pos + new Vector3(0, 0, tileSize / 2f));  }
            }


            //spawn pillars
            if (left == true && up == true)
            {
                SpawnObject(pillarObject, pos + new Vector3(-tileSize / 2f, 0, tileSize / 2f));
            }
            if (left == true && down == true)
            {
                SpawnObject(pillarObject, pos + new Vector3(-tileSize / 2f, 0, -tileSize / 2f));
            }
            if (right == true && up == true)
            {
                SpawnObject(pillarObject, pos + new Vector3(tileSize / 2f, 0, tileSize / 2f));
            }
            if (right == true && down == true)
            {
                SpawnObject(pillarObject, pos + new Vector3(tileSize / 2f, 0, -tileSize / 2f));
            }

            //spawnneighbourPillars
            if (left || right || up || down)
            {
                UpdateNeighBourPillars(pos);
            }

        }
    }

    Vector3[] gridPositions(Vector3 start, Vector3 end)
    {
        List<Vector3> result = new List<Vector3>();

        Vector3 minPos = new Vector3(Mathf.Min(start.x, end.x), start.y, Mathf.Min(start.z, end.z));
        Vector3 maxPos = new Vector3(Mathf.Max(start.x, end.x), start.y, Mathf.Max(start.z, end.z));
        int amount = (int)(maxPos.x - minPos.x) * (int)(maxPos.z - minPos.z);
        if (amount > 10000) {
            Debug.LogError("Too many objects to spawn, may cause infinite loop");
            return result.ToArray();
        }

        for (int x = (int)minPos.x; x <= maxPos.x; x += tileSize)
        { 
            for (int z = (int)minPos.z; z <= maxPos.z; z += tileSize)
            {
                result.Add(new Vector3( x, minPos.y, z));
                //Debug.Log(result[result.Count - 1]);
            }
        }
        return result.ToArray();
    }


    public GameObject DetectedObject(EditorObject obj, Vector3 position)
    {
        return obj.list.Find(x => {
            if (x == null) { return false; }
            return x.transform.position == position;
        }
        );
    }
}

[System.Serializable]
public class EditorObject
{
    public GameObject prefab;
    [HideInInspector]
    public Transform parent = null;
    public void FindParent(LevelEditor origin)
    {
        parent = origin.transform.Find(prefab.name + " holder");
        if (parent == null)
        {
            parent = origin.createEMptyGameObject(prefab.name + " holder");
        }
    }
    [HideInInspector]
    public List<GameObject> list = new List<GameObject>();
}
