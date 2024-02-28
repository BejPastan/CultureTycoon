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
        ClearPart(ref part);
        part.Resize(startPos, endPos);//kinda git
        SetFloors(ref part);//kinda git
        for(int roomIndex = 0; roomIndex < parts.Length; roomIndex++)
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

    private void SetFloors(ref RoomPart part)
    {
        Vector2Int size = part.GetSize();
        Vector2Int gridId;
        for (int x = 1; x < size.x-1; x++)
        {
            for (int z =1; z < size.y-1; z++)
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
                    RoomCell[] walls = room.GetWallCellsFromGridId(gridId);
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
        Vector2Int gridId;
        Vector2Int size = part.GetSize();
        for (int x = 1; x < size.x-1; x++)
        {
            for (int z = 1; z < size.y-1; z++)
            {
                if (part.GetFloorById(x, z) != null)
                {
                    if (part.GetFloorById(x, z).CompareTag("Floor"))
                    {
                        //here i need to check where around this tiles are floors
                    }
                }
            }
        }
    }

    /// <summary>
    /// return true if wall can be build
    /// </summary>
    /// <param name="gridId"></param>
    /// <returns></returns>
    private bool CanBuildWall(Vector2Int gridId)
    {
        foreach (var room in parts)
        {
            //wall cannot be build if in this place is floor, but can be build if in this position is other wall
            if(room.GetFloorByGridId(gridId) != null)
            {
                if(room.GetFloorByGridId(gridId).CompareTag("Floor"))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void CreateWall(ref RoomPart part, Vector2Int id, Vector2Int centerId)
    {
        id = part.GetIdByGridId(id);
        part.CreateWall(id, ref wallPref, centerId);
    }

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

    /// <summary>
    /// Remove all walls that are on this grid cell
    /// </summary>
    /// <param name="roomIndex"></param>
    /// <param name="gridId"></param>
    private void RemoveWallsOnCell(Vector2Int gridId)
    {
        //Debug.Log("RemoveElement at "+ gridId + " from "+roomIndex);
        foreach (var room in parts)
        {
            //remove walls in position
        }
        
    }

    private void ClearPart(ref RoomPart part)
    {
        Vector2Int size = part.GetSize();
        Vector2Int gridId;
        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                gridId = part.GetGridId(new Vector2Int(x, z));
                part.RemoveElement(new Vector2Int(x,z));
                if(part.GetFloorById(x, z) != null)
                    if(part.GetFloorById(x, z).CompareTag("Floor"))
                        grid.ChangeGridState(GridState.free, gridId);
            }
        }
    }
}

public struct RoomPart
{
    public Vector2Int gridShift;
    public Vector2Int gridEnd;
    public Grid grid;
    public RoomCell[,] elements;

    public RoomPart(ref Grid grid)
    {
        gridShift = Vector2Int.zero;
        gridEnd = Vector2Int.zero;
        elements = new RoomCell[0, 0];
        this.grid = grid;
    }

    public void Resize(Vector2Int startPos, Vector2Int endPos)
    {
        //set start as smaller value
        if(startPos.x > endPos.x)
        {
            int temp = startPos.x;
            startPos.x = endPos.x;
            endPos.x = temp;
        }
        if(startPos.y > endPos.y)
        {
            int temp = startPos.y;
            startPos.y = endPos.y;
            endPos.y = temp;
        }

        gridShift = startPos;
        gridEnd = endPos;
        Vector2Int size = endPos - startPos+Vector2Int.one;

        Vector2Int deltaShift = startPos - gridShift;

        RoomCell[,] newElements = new RoomCell[size.x, size.y];
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (x + deltaShift.x >= 0 && x + deltaShift.x < elements.GetLength(0) - 1 && y + deltaShift.y >= 0 && y + deltaShift.y < elements.GetLength(1) - 1)
                {
                    newElements[x, y] = elements[x + deltaShift.x, y + deltaShift.y];
                }
            }
        }
        elements = newElements;
    }

    public void CreateWall(Vector2Int id, ref GameObject pref, Vector2Int cellId)
    {
        Vector3 pos = grid.GetWorldPosition(GetGridId(id + cellId));
        //here goes magic stuff with direction
        Quaternion rotation = Quaternion.Euler(0, (90*cellId.y*(cellId.y+1)) + ((cellId.x*-90)-90), 0);
        //create element

    }

    public void CreateFloor(Vector2Int id, ref GameObject pref)
    {
        Vector3 pos = grid.GetWorldPosition(GetGridId(id));
        //create element
        Transform floor = GameObject.Instantiate(pref, pos, Quaternion.identity).transform;
        elements[id.x, id.y] = new RoomCell(floor, GetGridId(id));
    }

    /// <summary>
    /// This is fore removing whole cell
    /// </summary>
    /// <param name="id"></param>
    public void RemoveElement(Vector2Int id)
    {
        elements[id.x, id.y].RemoveElement();
    }

    public Transform GetFloorById(int x, int y)
    {
        return elements[x, y].GetFloor();
    }

    public Transform GetFloorByGridId(Vector2Int gridId)
    {
        return elements[gridId.x - gridShift.x, gridId.y - gridShift.y].GetFloor();
    }

    public RoomCell[] GetWallCellsFromGridId(Vector2Int gridId)
    {
        RoomCell[] wallsCells = new RoomCell[0];
        Vector2Int startSearch;
        Vector2Int endSearch;
        startSearch = new Vector2Int(gridId.x - 1, gridId.y - 1);
        endSearch = new Vector2Int(gridId.x + 1, gridId.y + 1);
        if(startSearch.x < 0)
        {
            startSearch.x = 0;
        }
        if(startSearch.y < 0)
        {
            startSearch.y = 0;
        }
        if(endSearch.x > elements.GetLength(0))
        {
            endSearch.x = elements.GetLength(0);
        }
        if(endSearch.y > elements.GetLength(1))
        {
            endSearch.y = elements.GetLength(1);
        }
        for (int x = startSearch.x; x < endSearch.x; x++)
        {
            for (int y = startSearch.y; y < endSearch.y; y++)
            {
                if (elements[x, y].GetWall(gridId) != null)
                {
                    wallsCells[0] = elements[x, y];
                }
            }
        }

        return wallsCells;
    }

    public RoomCell GetCellFromId(int x, int y)
    {
        try
        {
            return elements[x, y];
        } catch { }
        return null;
    }

    public RoomCell GetCellByGridId(Vector2Int gridId)
    {
        try
        {
            return elements[gridId.x - gridShift.x, gridId.y - gridShift.y];
        } catch { }
        return null;
    }

    //git
    public Vector2Int GetIdByGridId(int x, int y)
    {
        return new Vector2Int(x - gridShift.x, y - gridShift.y);
    }

    //git
    public Vector2Int GetIdByGridId(Vector2Int gridId)
    {
        return gridId - gridShift;
    }

    //git
    public Vector2Int GetGridId(Vector2Int id)
    {
        return id+gridShift;
    }

    //git
    public Vector2Int GetSize()
    {
        Vector2Int size = new Vector2Int(elements.GetLength(0), elements.GetLength(1));
        return size;
    }
}

public partial class RoomCell
{
    public Transform tile;
    Transform[,] walls;
    Vector2Int gridPos;

    public RoomCell(Transform tile, Vector2Int gridPos)
    {
        this.tile = tile;
        this.gridPos = gridPos;
        this.walls = new Transform[3,3];
    }

    /// <summary>
    /// removing whole RoomCell, if you wato remove only floor u just can't
    /// </summary>
    public void RemoveElement()
    {
        if(tile != null)
        {
            GameObject.Destroy(tile.gameObject);
            tile = null;
        }
        for(int x = 0; x<walls.GetLength(0); x++)
        {
            for(int z = 0; z<walls.GetLength(1); z++)
            {
                if (walls[x,z] != null)
                {
                    GameObject.Destroy(walls[x,z].gameObject);
                    walls[x,z] = null;
                }
            }
        }
    }

    public void RemoveWallByGridId(Vector2Int gridId)
    {
        RemoveWall(gridId - gridPos);
    }

    private void RemoveWall(Vector2Int id)
    {
        if (walls[id.x, id.y] != null)
        {
            GameObject.Destroy(walls[id.x, id.y].gameObject);
            walls[id.x, id.y] = null;
        }
    }

    public Transform GetFloor()
    {
        return tile;
    }

    public Transform GetWall(Vector2Int gridId)
    {
        //+1 because from the center of the cell is 1 unit to the edge to -1 x and -1 z
        return walls[gridId.x-gridPos.x+1, gridId.y-gridPos.x+1];
    }

    private Vector2Int CalcWallId(Quaternion rotation)
    {
        if(rotation.eulerAngles.y < 0)
        {
            rotation.eulerAngles += new Vector3(0, 360, 0);
        }
        //here goes magic stuff with direction
        switch(rotation.eulerAngles.y)
        {
            case 0:
                return new Vector2Int(2, 1);
            case 90:
                return new Vector2Int(1, 0);
            case 180:
                return new Vector2Int(0, 1);
            case 270:
                return new Vector2Int(1, 2);
        }
        return new Vector2Int(1, 1);
    }
}