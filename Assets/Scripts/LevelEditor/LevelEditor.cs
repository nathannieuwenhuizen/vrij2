using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public enum ClickEvent
{
    add,
    remove
}


public class LevelEditor : MonoBehaviour
{
    public bool editmode = true;
    public ClickEvent clickevent;


    [SerializeField]
    private EditorObject tileObject;

    [SerializeField]
    private GameObject wallPrefab;

    [SerializeField]
    private GameObject pillarPrefab;

    [SerializeField]
    private GameObject wallParent;

    [SerializeField]
    private GameObject pillarParent;

    [SerializeField]
    private GameObject hoverObject;

    [SerializeField]
    private Material hoverMaterial;

    [SerializeField]
    private int tileSize = 5;

    private List<GameObject> walls = new List<GameObject>(); 
    private List<GameObject> pillars = new List<GameObject>();

    [SerializeField]
    public int Size
    {
        get { return tileSize; }
        set { tileSize = value; }
    }

    public List<GameObject> CreateList(GameObject parent)
    {
        List<GameObject> list = new List<GameObject>();
        Transform[] children = parent.GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] != parent.transform)
            {
                list.Add(children[i].gameObject);
            }
        }
        return list;
    }

    public void EditEvent(Vector3 startPos, Vector3 endPos)
    {
        tileObject.list = CreateList(tileObject.parent);
        
        walls = CreateList(wallParent);
        pillars = CreateList(pillarParent);

        //round at position;
        startPos = new Vector3(Mathf.Round(startPos.x / tileSize) * tileSize, Mathf.Round(startPos.y / tileSize) * tileSize, Mathf.Round(startPos.z / tileSize) * tileSize);
        endPos = new Vector3(Mathf.Round(endPos.x / tileSize) * tileSize, Mathf.Round(endPos.y / tileSize) * tileSize, Mathf.Round(endPos.z / tileSize) * tileSize);
        Vector3[] grid = gridPositions(startPos, endPos);

        if (clickevent == ClickEvent.add)
        {
            foreach (Vector3 tilePos in grid)
            {
                SpawnObject(tilePos, tileObject.parent, tileObject.list);
            }
            UpdateWalls(grid);

        }
        if (clickevent == ClickEvent.remove)
        {
            foreach (Vector3 tilePos in grid)
            {
                RemoveObject(tilePos);
            }
            UpdateWallsWhenRemove(grid);

        }

    }

    public void UpdateHover(Vector3 position)
    {
        CheckHover();
        position = new Vector3(Mathf.Round(position.x / tileSize) * tileSize, Mathf.Round(position.y / tileSize) * tileSize, Mathf.Round(position.z / tileSize) * tileSize);
        hoverObject.transform.position = position;
    }
    public void CheckHover()
    {
        if (hoverObject == null)
        {
            return; //gaf bugs...
            hoverObject = Instantiate(tileObject.prefab, transform);
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

    public void SpawnWall(Vector3 position, Vector3 offset, float rotation = 0)
    {
        if (DetectedFloor(position +offset) != null) { return; }

        //instantiatePrefab
        GameObject prefab_instance = PrefabUtility.InstantiatePrefab(wallPrefab as GameObject) as GameObject;
        prefab_instance.transform.parent = wallParent.transform;
        prefab_instance.transform.position = position + offset;

        prefab_instance.transform.Rotate(new Vector3(0, rotation, 0));
        //add to list
        walls.Add(prefab_instance);
    }

    public void SpawnObject(Vector3 position, GameObject parent, List<GameObject> list)
    {
        if (DetectedFloor(position) != null) { return; }

        //instantiatePrefab
        GameObject prefab_instance = PrefabUtility.InstantiatePrefab(tileObject.prefab as GameObject) as GameObject;
        prefab_instance.transform.parent = parent.transform;
        prefab_instance.transform.position = position;

        //add to list
        list.Add(prefab_instance);

    }


    public void RemoveSurroundingWalls(Vector3 pos)
    {
        RemovePotentialWall(pos + new Vector3(-tileSize / 2f, 0, 0));
        RemovePotentialWall(pos + new Vector3(tileSize / 2f, 0, 0));
        RemovePotentialWall(pos + new Vector3(0, 0, -tileSize / 2f));
        RemovePotentialWall(pos + new Vector3(0, 0, tileSize / 2f));
    }

    public void UpdateWalls(Vector3[] positions)
    {
        foreach (Vector3 pos in positions)
        {
            RemoveSurroundingWalls(pos);
            //left
            if (pos.x == positions[0].x)
            {
                if (DetectedFloor(pos + new Vector3(-tileSize, 0, 0)) == null){ SpawnWall(pos, new Vector3(-tileSize / 2f, 0, 0), 90); }
            }
            //right
            if (pos.x == positions[positions.Length - 1].x)
            {
                if (DetectedFloor(pos + new Vector3(tileSize, 0, 0)) == null){SpawnWall(pos, new Vector3(tileSize / 2f, 0, 0) , -90); }
            }
            //bottom 
            if (pos.z == positions[0].z)
            {
                if (DetectedFloor(pos + new Vector3(0, 0, -tileSize)) == null){ SpawnWall(pos, new Vector3(0, 0, -tileSize / 2f) , 180); }
            }
            //top
            if (pos.z == positions[positions.Length - 1].z)
            {
                if (DetectedFloor(pos + new Vector3(0, 0, tileSize)) == null) {   SpawnWall(pos, new Vector3(0, 0, tileSize / 2f));  }
            }
        }
    }
    public void UpdateWallsWhenRemove(Vector3[] positions)
    {
        foreach (Vector3 pos in positions)
        {
            RemoveSurroundingWalls(pos);

            //left
            if (pos.x == positions[0].x)
            {
                if (DetectedFloor(pos + new Vector3(-tileSize, 0, 0)) != null){ SpawnWall(pos, new Vector3(-tileSize / 2f, 0, 0), 90); }
            }
            //right
            if (pos.x == positions[positions.Length - 1].x)
            {
                if (DetectedFloor(pos + new Vector3(tileSize, 0, 0)) != null){SpawnWall(pos, new Vector3(tileSize / 2f, 0, 0) , -90); }
            }
            //bottom 
            if (pos.z == positions[0].z)
            {
                if (DetectedFloor(pos + new Vector3(0, 0, -tileSize)) != null){ SpawnWall(pos, new Vector3(0, 0, -tileSize / 2f) , 180); }
            }
            //top
            if (pos.z == positions[positions.Length - 1].z)
            {
                if (DetectedFloor(pos + new Vector3(0, 0, tileSize)) != null) {   SpawnWall(pos, new Vector3(0, 0, tileSize / 2f));  }
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

    public void RemoveObject(Vector3 position)
    {
        GameObject selectedTile = DetectedFloor(position);
        if (selectedTile != null)
        {
            DestroyImmediate(selectedTile.gameObject);
            tileObject.list.Remove(selectedTile);
        }
    }
    public void RemovePotentialWall(Vector3 position)
    {
        GameObject selectedWall = DetectedWall(position);
        if (selectedWall != null)
        {
            DestroyImmediate(selectedWall.gameObject);
            walls.Remove(selectedWall);
        }
    }

    public GameObject DetectedFloor(Vector3 position)
    {
        return tileObject.list.Find(x => {
            if (x == null) { return false; }
            return x.transform.position == position;
        }
        );
    }

    public GameObject DetectedWall(Vector3 position)
    {
        return walls.Find(x => {
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
    public GameObject parent;
    public List<GameObject> list = new List<GameObject>();
}
//public class FloorTile
//{
//    public GameObject gameObject;
//    public NeighBourGroup<FloorTile> neighbourTiles;
//    public NeighBourGroup<GameObject> neighbourWalls;

//    public void UpdateWalls()
//    {
//    }
//}

//public class NeighBourGroup<T>
//{
//    public T up;
//    public T down;
//    public T left;
//    public T right;
//}