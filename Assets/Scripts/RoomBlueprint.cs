using System;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
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
        parts[parts.Length - 1] = new RoomPart(ref grid);
        Debug.Log("New part created");
    }

    /// <summary>
    /// this change bounds of one room part
    /// </summary>
    public void ChangeSize(Vector2Int startPos, Vector2Int endPos)
    {
        ChangeSize(startPos, endPos, ref parts[parts.Length - 1]);
    }

    private void ChangeSize(Vector2Int startPos, Vector2Int endPos, ref RoomPart part)
    {
        ClearPart(ref part);//kinda git
        part.Resize(startPos, endPos);//kinda git
        SetFloors(ref part);//kinda git
        for (int roomIndex = 0; roomIndex < parts.Length; roomIndex++)
        {
            SetWalls(ref parts[roomIndex], roomIndex);
        }
    }

    public void ConfirmBlueprint()
    {
        foreach(RoomPart part in parts)
        {
            grid.ChangeGridState(GridState.blueprint, GridState.room, part.gridShift, part.gridEnd);
        }
    }

    //probably git
    private void SetFloors(ref RoomPart part)
    {
        Debug.Log("Setting floors");
        Vector2Int size = part.GetSize();
        Debug.Log("Size: " + size);
        Vector2Int gridId;
        for (int x = 0; x < size.x; x++)
        {
            for (int z =0; z < size.y; z++)
            {
                gridId = part.GetGridId(new Vector2Int(x, z));
                bool isFloor = false;
                int roomIndex = 0;
                foreach (RoomPart room in parts)
                {
                    //here i need to check if in this place is floor or walls
                    if(room.GetFloorByGridId(gridId) != null)
                    {
                        isFloor = true;
                    }
                    RoomCell[] walls = room.GetWallCellsAroundGridId(gridId);
                    foreach (RoomCell wall in walls)
                    {
                        wall.RemoveWallByGridId(gridId);
                    }
                    roomIndex++;
                    
                }
                if(!isFloor)
                {
                    CreateFloor(gridId, ref part, ref floorPref);
                }
            }
        }
    }

    private void SetWalls(ref RoomPart part, int roomPart)
    {
        Debug.Log("Setting walls");
        Vector2Int gridId;
        Vector2Int size = part.GetSize();
        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                gridId = part.GetGridId(new Vector2Int(x, z));

                Vector2Int location = new Vector2Int(-1, 0);
                if(CanBuildWall(gridId, location, roomPart))
                {
                    CreateWall(ref part, gridId, location);
                }
                location = new Vector2Int(0, 1);
                if(CanBuildWall(gridId, location, roomPart))
                {
                    CreateWall(ref part, gridId, location);
                }
                location = new Vector2Int(1, 0);
                if(CanBuildWall(gridId, location, roomPart))
                {
                    CreateWall(ref part, gridId, location);
                }
                location = new Vector2Int(0, -1);
                if(CanBuildWall(gridId, location, roomPart))
                {
                    CreateWall(ref part, gridId, location);
                }
            }
        }
    }

    //probably git
    private bool CanBuildWall(Vector2Int centerGridId, Vector2Int orientation, int roomPart)
    {
        bool isFloor = false;
        foreach (RoomPart room in parts)
        {
            if (room.GetFloorByGridId(centerGridId+orientation) != null)
            {
                isFloor = true;
                break;
            }
        }
        if(isFloor)
        {
            return false;
        }
        RoomCell[] cells = parts[roomPart].GetWallCellsAroundGridId(centerGridId);
        Debug.Log("Cells: " + cells.Length);
        if (cells.Length > 0)
        { 
            //so here i have all walls that are around this cell and I need only this which have centerGridId as wall
            foreach (RoomCell cell in cells)
            {
                if (cell.GetFloorPos() == centerGridId)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void CreateWall(ref RoomPart part, Vector2Int gridId, Vector2Int orientation)
    {
        part.CreateWall(gridId, ref wallPref, orientation);
    }

    //probably git
    private void CreateFloor(Vector2Int gridId, ref RoomPart room, ref GameObject pref)
    {
        //test if prefab hase tag floor
        if(pref.CompareTag("Floor"))
        {
            Debug.Log("CreateFloor at "+ gridId);
            grid.ChangeGridState(GridState.blueprint, gridId);
        }
        room.CreateFloor(room.GetIdByGridId(gridId), ref pref);
    }

    //probably git
    private void ClearPart(ref RoomPart part)
    {
        Debug.Log("Clearing part");
        Vector2Int size = part.GetSize();
        Vector2Int gridId;
        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                gridId = part.GetGridId(new Vector2Int(x, z));
                part.RemoveElement(new Vector2Int(x,z));
                grid.ChangeGridState(GridState.free, gridId);
            }
        }
    }
}