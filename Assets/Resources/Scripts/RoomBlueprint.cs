using System;
using UnityEngine;

//make this available in editor
[CreateAssetMenu(fileName = "RoomBlueprint", menuName = "RoomBlueprint", order = 1)]
public class RoomBlueprint : ScriptableObject
{
    [SerializeField]
    public GameObject wallPref;
    [SerializeField]
    public GameObject floorPref;
    [SerializeField]
    public GameObject doorPref;
    [SerializeField] public int cellCost;
    public RoomPart[] parts = new RoomPart[0];
    public Grid grid;
    public Transform doorObj;
    public Transform roomObj;
    [SerializeField] public RoomType roomType;
    public RoomType RoomType { get => roomType; }

    [SerializeField] public int minSurface;
    
    /// <summary>
    /// Creating New Blueprint
    /// </summary> 
    public void CreateNewBlueprint(ref Grid grid, Transform roomObj)
    {
        this.grid = grid;
        this.roomObj = roomObj;
    }

    public void Cancel()
    {
        for(int roomIndex = 1; roomIndex < parts.Length; roomIndex++)
        {
            ClearPart(ref parts[roomIndex]);
        }
        SetWalls(ref parts[0], 0);
    }

    /// <summary>
    /// Creating new room part
    /// </summary>
    public void CreateNewPart()
    {
        Array.Resize(ref parts, parts.Length + 1);
        parts[parts.Length - 1] = new RoomPart(ref grid, ref roomObj);
        Debug.Log("New part created");
    }

    /// <summary>
    /// this change bounds of one room part
    /// </summary>
    public void PaintPart(Vector2Int startPos, Vector2Int endPos)
    {
        ChangeSize(startPos, endPos, ref parts[parts.Length - 1]);
    }

    /// <summary>
    /// Remove all cells from area
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    public void EraseArea(Vector2Int startPos, Vector2Int endPos)
    {
        //set startPos as smaller value
        if (startPos.x > endPos.x)
        {
            int temp = startPos.x;
            startPos.x = endPos.x;
            endPos.x = temp;
        }
        if (startPos.y > endPos.y)
        {
            int temp = startPos.y;
            startPos.y = endPos.y;
            endPos.y = temp;
        }
        //Remove all Cells from area
        ClearArea(startPos, endPos);
        //Set again walls //i guess this should work
        for (int roomIndex = 0; roomIndex < parts.Length; roomIndex++)
        {
            SetWalls(ref parts[roomIndex], roomIndex);
        }
        //optionaly check if there are any parts that are empty and remove them
    }

    /// <summary>
    /// remove whole room and all its parts
    /// </summary>
    public void RemoveAll()
    {
        for(int roomIndex = 0; roomIndex < parts.Length; roomIndex++)
        {
            ClearPart(ref parts[roomIndex]);
        }
    }


    public bool PassRequirements(out bool noCells)
    {
        //count all cells in all parts
        noCells = false;
        int surface = 0;
        foreach (RoomPart part in parts)
        {
            for(int x = 0; x < part.GetSize().x; x++)
            {
                for(int z = 0; z < part.GetSize().y; z++)
                {
                    if(part.GetFloorByGridId(part.GetGridId(new Vector2Int(x, z))) != null)
                    {
                        surface++;
                    }
                }
            }
        }

        if(surface == 0)
        {
            noCells = true;
            return false;
        }               

        if(surface < minSurface || doorObj == null)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Resize selected room part and fill with floor and build walls around
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="part"></param>
    private void ChangeSize(Vector2Int startPos, Vector2Int endPos, ref RoomPart part)
    {
        ClearPart(ref part);
        part.Resize(startPos, endPos);
        SetFloors(ref part);
        for (int roomIndex = 0; roomIndex < parts.Length; roomIndex++)
        {
            SetWalls(ref parts[roomIndex], roomIndex);
        }
    }

    /// <summary>
    /// Ending editing and returnign all cells
    /// </summary>
    public void ConfirmBlueprint(out RoomCell[,] cells, out Vector3 roomCenter, out int newCells)
    {
        newCells = CountNewCells();
        for(int roomIndex = 1; roomIndex < parts.Length; roomIndex++)
        {
            parts[0].MergeParts(parts[roomIndex]);
        }
        parts[0].EndEdit();
        Array.Resize(ref parts, 1);
        grid.ChangeGridState(GridState.blueprint, GridState.room, parts[0].gridShift, parts[0].gridEnd);
        cells = parts[0].elements;
        //calculate room center
        roomCenter = grid.GetWorldPosition(parts[0].GetGridId(parts[0].GetSize()/2)) ;
    }

    /// <summary>
    /// count cells created in last part
    /// </summary>
    /// <returns></returns>
    public int CountNewCells()
    {
        int count = 0;
        foreach (RoomPart part in parts)
        {
            count += part.newCells;
        }
        return count;
    }

    /// <summary>
    /// enable collision on all cells
    /// </summary>
    public void EnableCollision()
    {
        foreach (RoomPart part in parts)
        {
            part.EnableCollision();
        }
    }

    /// <summary>
    /// disable collision on all cells
    /// </summary>
    public void DisableCollision()
    {
        foreach (RoomPart part in parts)
        {
            part.DisableCollision();
        }
    }

    /// <summary>
    /// build doors on selected place
    /// </summary>
    /// <param name="position">grid id to buuld door</param>
    /// <param name="wallRotation">orientation of wall on which build door</param>
    public void SetDoors(Vector2Int position,Quaternion wallRotation)
    {
        RoomCell cell;
        if(doorObj != null)
        {
            cell = GetCell(grid.GetGridId(doorObj.transform.position));
            if(cell != null)
            {
                cell.ChangeWallObject(wallPref, cell.CalcWallId(doorObj.rotation));
            }
        }
        cell = GetCell(position);
        if(cell != null)
        {
            cell.ChangeWallObject(doorPref, cell.CalcWallId(wallRotation));
            doorObj = cell.GetWallByLocalPos(cell.CalcWallId(wallRotation)).transform;
        }
   
    }

    /// <summary>
    /// Build floor in selected room part
    /// </summary>
    /// <param name="part"></param>
    private void SetFloors(ref RoomPart part)
    {
        Vector2Int size = part.GetSize();
        Vector2Int gridId;
        for (int x = 0; x < size.x; x++)
        {
            for (int z =0; z < size.y; z++)
            {
                gridId = part.GetGridId(new Vector2Int(x, z));
                bool isFloor = false;
                int roomIndex = 0;
                if (grid.gridStates[gridId.x, gridId.y] ==GridState.free)
                {
                    foreach (RoomPart room in parts)
                    {
                        //here i need to check if in this place is floor or walls
                        if (room.GetFloorByGridId(gridId) != null)
                        {
                            isFloor = true;
                        }
                        RoomCell[] walls = room.GetWallCellsOutsideGridId(gridId);
                        foreach (RoomCell wall in walls)
                        {
                            //under this something is wrong
                            wall.RemoveWallByGridId(gridId);
                        }
                        roomIndex++;

                    }
                    if (!isFloor)
                    {
                        CreateFloor(gridId, ref part, ref floorPref);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Build walls around selected roomPart
    /// </summary>
    /// <param name="part"></param>
    /// <param name="roomPart"></param>
    private void SetWalls(ref RoomPart part, int roomPart)
    {
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

    /// <summary>
    /// check if can build wall in selected place in selected orientation
    /// </summary>
    /// <param name="centerGridId">cell in which wall will be build</param>
    /// <param name="orientation">On which side of cell wall will be build</param>
    /// <param name="roomPart">in which roompart</param>
    /// <returns></returns>
    private bool CanBuildWall(Vector2Int centerGridId, Vector2Int orientation, int roomPart)
    {
        bool isFloor = false;
        foreach (RoomPart room in parts)
        {
            if (room.GetFloorByGridId(centerGridId + orientation) != null)
            {
                isFloor = true;
                break;
            }
        }
        if(isFloor)
        {
            return false;
        }

        //check if there is walls in this place
        RoomCell cell = parts[roomPart].GetCellByGridId(centerGridId);
        if (cell != null)
        {
            if(cell.GetWallByLocalPos(orientation + Vector2Int.one) != null)
                return false;
        }
        else
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Build wall in selected place in selected orientation in selected room part
    /// </summary>
    /// <param name="part"></param>
    /// <param name="gridId"></param>
    /// <param name="orientation"></param>
    private void CreateWall(ref RoomPart part, Vector2Int gridId, Vector2Int orientation)
    {
        
        part.CreateWall(gridId, ref wallPref, orientation);
    }

    /// <summary>
    /// Build floor in selected place in selected room part
    /// </summary>
    /// <param name="gridId"></param>
    /// <param name="room"></param>
    /// <param name="pref"></param>
    private void CreateFloor(Vector2Int gridId, ref RoomPart room, ref GameObject pref)
    {
        //test if prefab hase tag floor
        if(pref.CompareTag("Floor"))
        {
            grid.ChangeGridState(GridState.blueprint, gridId);
        }
        room.CreateFloor(room.GetIdByGridId(gridId), ref pref);
    }

    /// <summary>
    /// Remove all cells from selected part
    /// </summary>
    /// <param name="part"></param>
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
                if(part.GetCellFromId(x, z) != null)
                {
                    grid.ChangeGridState(GridState.free, gridId);
                }
                
            }
        }
    }

    /// <summary>
    /// Remove all cells from area
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    private void ClearArea(Vector2Int startPos, Vector2Int endPos)
    {
        Vector2Int gridId;
        Debug.Log("Clearing area from " + startPos + " to "+ endPos);
        for (int x = startPos.x; x <= endPos.x; x++)
        {
            for (int z = startPos.y; z <= endPos.y; z++)
            {
                gridId = new Vector2Int(x, z);
                Vector2Int id;
                foreach (RoomPart part in parts)
                {
                    id = part.GetIdByGridId(gridId);
                    part.RemoveElement(id);
                }
            }
        }
    }

    /// <summary>
    /// return cell on selected gridId
    /// </summary>
    /// <param name="gridId"></param>
    /// <returns></returns>
    private RoomCell GetCell(Vector2Int gridId)
    {
        foreach (RoomPart part in parts)
        {
            if(part.GetCellByGridId(gridId) != null)
            {
                return part.GetCellByGridId(gridId);
            }
        }
        return null;
    }
}
