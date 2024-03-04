using System;
using UnityEngine;

public class RoomPart
{
    public Vector2Int gridShift;
    public Vector2Int gridEnd;
    public Grid grid;
    public RoomCell[,] elements;

    /// <summary>
    /// Creating new RoomPart instance
    /// </summary>
    /// <param name="grid"></param>
    public RoomPart(ref Grid grid)
    {
        gridShift = Vector2Int.zero;
        gridEnd = Vector2Int.zero;
        elements = new RoomCell[0, 0];
        this.grid = grid;
    }

    /// <summary>
    /// Set new size of this roompart, fill with floor and build walls
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    public void Resize(Vector2Int startPos, Vector2Int endPos)
    {
        Debug.Log("Resizing");
        //set start as smaller value
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

        gridShift = startPos;
        gridEnd = endPos;
        Vector2Int size = endPos - startPos + Vector2Int.one;

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

    public void MergeParts(RoomPart toMerge)
    {
        Vector2Int start = toMerge.gridShift;
        if(start.x < gridShift.x)
        {
            start.x = gridShift.x;
        }
        if (start.y < gridShift.y)
        {
            start.y = gridShift.y;
        }
        Vector2Int end = toMerge.gridEnd;
        if (end.x > gridEnd.x)
        {
            end.x = gridEnd.x;
        }
        if (end.y > gridEnd.y)
        {
            end.y = gridEnd.y;
        }

        Vector2Int size = end - start + Vector2Int.one;

        RoomCell[,] newElements = new RoomCell[size.x, size.y];

        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                Vector2Int gridId = new Vector2Int(x + start.x, z + start.y);
                if (toMerge.GetCellByGridId(gridId)!= null)
                {
                    newElements[x, z] = toMerge.GetCellByGridId(gridId);
                }
                else if(GetCellByGridId(gridId) != null)
                {
                    newElements[x, z] = GetCellByGridId(gridId);
                }
            }
        }
    }

    /// <summary>
    /// create wall in selected cell with selected orientation from prefab
    /// </summary>
    /// <param name="gridId">position in game system grid</param>
    /// <param name="pref">prefab of wall</param>
    /// <param name="orientation">On which side of the cell wall should be</param>
    public void CreateWall(Vector2Int gridId, ref GameObject pref, Vector2Int orientation)
    {
        Vector2Int id = GetIdByGridId(gridId);
        Debug.Log("Creating wall at " + gridId + " at local "+ id);
        elements[id.x, id.y].CreateWall(gridId, ref pref, orientation);
    }

    /// <summary>
    /// Build floor in selected cell from prefab
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pref"></param>
    public void CreateFloor(Vector2Int id, ref GameObject pref)
    {
        Vector3 pos = grid.GetWorldPosition(GetGridId(id));
        //create element
        Transform floor = GameObject.Instantiate(pref, pos, Quaternion.identity).transform;
        //get this RoomPart
        RoomPart refPart = this;
        elements[id.x, id.y] = new RoomCell(floor, GetGridId(id), ref refPart);
    }

    /// <summary>
    /// This is for removing whole cell
    /// </summary>
    /// <param name="id">elementId</param>
    public void RemoveElement(Vector2Int id)
    {
        try
        {
            elements[id.x, id.y].RemoveElement();
            elements[id.x, id.y] = null;
            grid.ChangeGridState(GridState.free, GetGridId(id));
        }
        catch { }
    }

    /// <summary>
    /// Get floor transform fromw RoomPart array position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Transform GetFloorById(int x, int y)
    {
        return elements[x, y].GetFloor();
    }

    /// <summary>
    /// Get floor transform from RoomPart by system grid Id
    /// </summary>
    /// <param name="gridId"></param>
    /// <returns></returns>
    public Transform GetFloorByGridId(Vector2Int gridId)
    {
        try
        {
            return elements[gridId.x - gridShift.x, gridId.y - gridShift.y].GetFloor();
        }
        catch
        {
            return null;
        }
        
    }

    /// <summary>
    /// Get cells that have wall with back to this cell
    /// </summary>
    /// <param name="gridId">cell position</param>
    /// <returns></returns>
    public RoomCell[] GetWallCellsOutsideGridId(Vector2Int gridId)
    {
        RoomCell[] wallsCells = new RoomCell[0];
        Vector2Int startSearch;
        Vector2Int endSearch;
        startSearch = gridId - gridShift -Vector2Int.one;
        endSearch = gridId - gridShift + Vector2Int.one;

        for (int x = startSearch.x; x <= endSearch.x; x++)
        {
            for (int z = startSearch.y; z <= endSearch.y; z++)
            {
                if (x >= 0 && x < elements.GetLength(0) && z >= 0 && z < elements.GetLength(1))
                {
                    if (elements[x, z] != null)
                    {
                        Transform wall = elements[x, z].GetWall(gridId);
                        Debug.Log("Wall at " + (x + gridShift.x) + " " + (z + gridShift.y) + " " + wall);
                        if (wall != null)
                        {
                            Array.Resize(ref wallsCells, wallsCells.Length + 1);
                            wallsCells[wallsCells.Length - 1] = elements[x, z];
                        }
                    }
                }
            }
        }

        return wallsCells;
    }

    /// <summary>
    /// get cell data from RoomPart array position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public RoomCell GetCellFromId(int x, int y)
    {
        try
        {
            return elements[x, y];
        }
        catch { }
        return null;
    }

    /// <summary>
    /// get cell data from RoomPart by system grid Id
    /// </summary>
    /// <param name="gridId"></param>
    /// <returns>if cell not exist return null</returns>
    public RoomCell GetCellByGridId(Vector2Int gridId)
    {
        try
        {
            return elements[gridId.x - gridShift.x, gridId.y - gridShift.y];
        }
        catch { }
        return null;
    }

    public void EnableCollision()
    {
        foreach (RoomCell cell in elements)
        {
            if (cell != null)
            {
                cell.EnableCollision();
            }
        }
    }

    public void DisableCollision()
    {
        foreach (RoomCell cell in elements)
        {
            if (cell != null)
            {
                cell.DisableCollision();
            }
        }
    }

    /// <summary>
    /// get position in RoomPart array by system grid Id
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector2Int GetIdByGridId(int x, int y)
    {
        return new Vector2Int(x - gridShift.x, y - gridShift.y);
    }

    /// <summary>
    /// get position in RoomPart array by system grid Id
    /// </summary>
    /// <param name="gridId"></param>
    /// <returns></returns>
    public Vector2Int GetIdByGridId(Vector2Int gridId)
    {
        return gridId - gridShift;
    }

    /// <summary>
    /// get system grid Id by position in RoomPart array
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Vector2Int GetGridId(Vector2Int id)
    {
        return id + gridShift;
    }

    /// <summary>
    /// get size of this RoomPart
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetSize()
    {
        Vector2Int size = new Vector2Int(elements.GetLength(0), elements.GetLength(1));
        return size;
    }
}