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
        ChangeSize(startPos, endPos, ref parts[parts.Length - 1]);
    }

    private void ChangeSize(Vector2Int startPos, Vector2Int endPos, ref RoomPart part)
    {
        part.RemoveAllElements();
        part.Resize(startPos, endPos);
        SetFloors(ref part);
    }

    private void SetFloors(ref RoomPart part)
    {
        Vector2Int size = part.GetSize();
        Vector2Int gridId;
        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                gridId = grid.GetGridId(part.gridShift, new Vector2Int(x, z));
                bool isFloor = false;
                foreach (RoomPart room in parts)
                {
                    if(room.GetObjectByGridId(gridId).CompareTag("Floor"))
                    {
                        isFloor = true;
                    }
                    else
                    {
                        //removing other objects i will do it later
                    }
                }
                if(!isFloor)
                {
                    CreateElement(gridId, ref part, ref floorPref);
                }
            }
        }
    }

    private void CreateElement(Vector2Int gridId, ref RoomPart room, ref GameObject pref)
    {
        Vector3 pos = grid.GetWorldPosition(gridId);
        //test if prefab hase tag floor
        if(pref.CompareTag("Floor"))
        {
            grid.ChangeGridState(GridState.blueprint, gridId, gridId);
        }       
        room.CreateElement(room.GetIdByGridId(gridId), ref pref);
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
        startPos-= Vector2Int.one;
        endPos+= Vector2Int.one;
        Vector2Int size = endPos - startPos+Vector2Int.one;

        Vector2Int deltaShift = startPos - gridShift;

        Transform[,] newElements = new Transform[size.x, size.y];
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                if (x + deltaShift.x >= 0 && x + deltaShift.x < size.x - 1 && y + deltaShift.y >= 0 && y + deltaShift.y < size.y - 1)
                {
                    newElements[x, y] = this.elements[x + deltaShift.x, y + deltaShift.y];
                }
            }
        }
        elements = newElements;
    }

    public void CreateElement(Vector2Int id, ref GameObject pref)
    {
        Vector3 pos = grid.GetWorldPosition(id);
        //create element
        elements[id.x, id.y] = GameObject.Instantiate(pref, pos, Quaternion.identity).transform;
    }

    public void RemoveElement(Vector2Int id)
    {
        if (elements[id.x, id.y] != null)
        {
            GameObject.Destroy(elements[id.x, id.y].gameObject);
            elements[id.x, id.y] = null;
        }
    }

    public void RemoveAllElements()
    {
        for (int x = 0; x < elements.GetLength(0); x++)
        {
            for (int y = 0; y < elements.GetLength(1); y++)
            {
                RemoveElement(new Vector2Int(x, y));
            }
        }
    }

    public Transform GetObjectFromId(int x, int y)
    {
        return elements[x, y];
    }

    public Transform GetObjectByGridId(int x, int y)
    {
        return elements[x - gridShift.x, y - gridShift.y];
    }

    public Transform GetObjectByGridId(Vector2Int gridId)
    {
        return elements[gridId.x - gridShift.x, gridId.y - gridShift.y];
    }

    public Vector2Int GetIdByGridId(int x, int y)
    {
        return new Vector2Int(x - gridShift.x, y - gridShift.y);
    }

    public Vector2Int GetIdByGridId(Vector2Int gridId)
    {
        return gridId - gridShift;
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
