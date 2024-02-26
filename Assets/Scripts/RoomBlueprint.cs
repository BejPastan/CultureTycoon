using System;
using UnityEngine;

public class RoomBlueprint : MonoBehaviour
{
    [SerializeField]
    public GameObject wallPref;
    [SerializeField]
    public GameObject floorPref;
    public RoomPart[] parts = new RoomPart[0];
    public Grid grid;
    
    /// <summary>
    /// Creating New Blueprint
    /// </summary> 
    public void createNewBlueprint(ref Grid grid, GameObject wallPref, GameObject floorPref)
    {
        this.grid = grid;
        this.wallPref = wallPref;
        this.floorPref = floorPref;
    }

    /// <summary>
    /// Creating new room part
    /// </summary>
    public void CreateNewPart()
    {
        Array.Resize(ref parts, parts.Length + 1);
        parts[parts.Length - 1] = new RoomPart();
    }

    /// <summary>
    /// this change bounds of one room part
    /// </summary>
    public void ChangeSize(Vector2Int startPos, Vector2Int endPos)
    {

    }

    private void SetFloors(ref RoomPart part, int id)
    {
        Vector2Int size = part.GetSize();
        Vector2Int gridId;
        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                gridId = grid.GetGridId(part.gridShift, new Vector2Int(x, z));
                foreach (RoomPart room in parts)
                {
                    Vector2Int id = room.GetIdByGridId(gridId.x, gridId.y);
                }
                part.elements[x, z] = CreateFloor(gridId);
            }
        }
    }

    private Transform CreateFloor(Vector3 pos)
    {
        Vector2Int gridId = grid.GetGridId(pos);
        grid.gridStates[gridId.x, gridId.y] = GridState.blueprint;

        return Instantiate(floorPref, pos, Quaternion.identity).transform;
    }

    private Transform CreateFloor(Vector2Int gridId)
    {
        Vector3 pos = grid.GetWorldPosition(gridId);
        grid.gridStates[gridId.x, gridId.y] = GridState.blueprint;
        return Instantiate(floorPref, pos, Quaternion.identity).transform;
    }
}

public struct RoomPart
{
    public Transform[,] elements;
    public Vector2Int gridShift;
    public Vector2Int gridEnd;
    public Grid grid;

    public RoomPart(Vector2Int gridShift, Vector2Int gridEnd, ref Grid grid)
    {
        this.gridShift = gridShift;
        this.gridEnd = gridEnd;
        elements = new Transform[0, 0];
        this.grid = grid;
    }



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

    public Vector2Int GetGridId(Vector2Int id)
    {
        return id+gridShift;
    }

    public Vector2Int GetSize()
    {
        Vector2Int size = new Vector2Int(gridEnd.x - gridShift.x, gridEnd.y - gridShift.y);
        return size;
    }
}
