using System;
using UnityEngine;

public class RoomPart
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

    //I guess this is git
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

    //this is NOT COMPLETE
    public void CreateWall(Vector2Int gridId, ref GameObject pref, Vector2Int orientation)
    {
        Vector3 pos = grid.GetWorldPosition(gridId);
        //create element
        Vector2Int id = GetIdByGridId(gridId);
        elements[id.x, id.y].CreateWall(gridId, ref pref, orientation);
    }

    //probably git
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
    /// This is fore removing whole cell
    /// </summary>
    /// <param name="id"></param>
    //git
    public void RemoveElement(Vector2Int id)
    {
        elements[id.x, id.y].RemoveElement();
    }

    //git
    public Transform GetFloorById(int x, int y)
    {
        return elements[x, y].GetFloor();
    }

    //git
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

    //git
    /// <summary>
    /// Get cells that have wall with cell on gridId directly behind
    /// </summary>
    /// <param name="gridId">cell position</param>
    /// <returns></returns>
    public RoomCell[] GetWallCellsAroundGridId(Vector2Int gridId)
    {
        RoomCell[] wallsCells = new RoomCell[0];
        Vector2Int startSearch;
        Vector2Int endSearch;
        startSearch = new Vector2Int(gridId.x - 1, gridId.y - 1);
        endSearch = new Vector2Int(gridId.x + 1, gridId.y + 1);

        if (startSearch.x < 0)
        {
            startSearch.x = 0;
        }
        if (startSearch.y < 0)
        {
            startSearch.y = 0;
        }
        if (endSearch.x > elements.GetLength(0))
        {
            endSearch.x = elements.GetLength(0);
        }
        if (endSearch.y > elements.GetLength(1))
        {
            endSearch.y = elements.GetLength(1);
        }

        for (int x = startSearch.x; x < endSearch.x; x++)
        {
            for (int y = startSearch.y; y < endSearch.y; y++)
            {
                if (elements[x, y] != null)
                    if (elements[x, y].GetWall(gridId) != null)
                    {
                        Array.Resize(ref wallsCells, wallsCells.Length + 1);
                        wallsCells[wallsCells.Length-1] = elements[x, y];
                    }
            }
        }
        return wallsCells;
    }

    //git
    public RoomCell GetCellFromId(int x, int y)
    {
        try
        {
            return elements[x, y];
        }
        catch { }
        return null;
    }

    //git
    public RoomCell GetCellByGridId(Vector2Int gridId)
    {
        try
        {
            return elements[gridId.x - gridShift.x, gridId.y - gridShift.y];
        }
        catch { }
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
        return id + gridShift;
    }

    //git
    public Vector2Int GetSize()
    {
        Vector2Int size = new Vector2Int(elements.GetLength(0), elements.GetLength(1));
        return size;
    }
}

