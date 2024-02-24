using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class RoomBlueprint : MonoBehaviour
{
    [SerializeField]
    public GameObject wallPref;
    [SerializeField]
    public GameObject floorPref;
    public RoomPart[] parts = new RoomPart[0];
    
    public void createNewBlueprint()
    {
        CreateNewPart();
    }

    private void CreateNewPart()
    {
        Array.Resize(ref parts, parts.Length + 1);
        parts[parts.Length - 1] = new RoomPart();
    }
}

public struct RoomPart
{
    public Transform[,] elements;
    public Vector2Int gridShift;

    public Transform GetObjectFromId(int x, int y)
    {
        return elements[x, y];
    }

    public Transform GetObjectByGridId(int x, int y)
    {
        return elements[x - gridShift.x, y - gridShift.y];
    }

    public Vector2Int GetIdByGridId(int x, int y)
    {
        return new Vector2Int(x - gridShift.x, y - gridShift.y);
    }
}
