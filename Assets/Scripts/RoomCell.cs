using UnityEngine;

public partial class RoomCell
{
    public Transform tile;
    Transform[,] walls;
    Vector2Int gridPos;
    RoomPart roomPart;

    public RoomCell(Transform tile, Vector2Int gridPos, ref RoomPart roomPart)
    {
        this.tile = tile;
        this.gridPos = gridPos;
        walls = new Transform[3, 3];
        this.roomPart = roomPart;
    }

    /// <summary>
    /// removing whole RoomCell, if you want to remove only floor u just can't
    /// </summary>
    public void RemoveElement()
    {
        if (tile != null)
        {
            GameObject.Destroy(tile.gameObject);
            tile = null;
        }
        for (int x = 0; x < walls.GetLength(0); x++)
        {
            for (int z = 0; z < walls.GetLength(1); z++)
            {
                if (walls[x, z] != null)
                {
                    GameObject.Destroy(walls[x, z].gameObject);
                    walls[x, z] = null;
                }
            }
        }
    }

    public void CreateWall(Vector2Int gridId, ref GameObject pref, Vector2Int orientation)
    {
        Vector3 pos = roomPart.grid.GetWorldPosition(gridPos);
        //here goes magic stuff with direction
        Quaternion rotation = Quaternion.Euler(0, (90 * orientation.y * (orientation.y - 1)) + ((orientation.x * +90) - 90), 0);
        //create element
        orientation += Vector2Int.one;
        walls[orientation.x, orientation.y] = GameObject.Instantiate(pref, pos, rotation).transform;
    }

    /// <summary>
    /// Remove wall by gridId of cell that is behind that wall
    /// </summary>
    /// <param name="gridId"></param>
    public void RemoveWallByGridId(Vector2Int gridId)
    {
        Debug.Log("RemoveWall at "+gridId);
        RemoveWall(gridId - gridPos+Vector2Int.one);
    }

    private void RemoveWall(Vector2Int id)
    {
        if (walls[id.x, id.y] != null)
        {
            GameObject.Destroy(walls[id.x, id.y].gameObject);
            walls[id.x, id.y] = null;
        }
    }

    public Vector2Int GetFloorPos()
    {
        return gridPos;
    }

    public Transform GetFloor()
    {
        if(tile==null)
        {
            return null;
        }
        return tile;
    }

    /// <summary>
    /// get wall by gridId of cell that is behind wall
    /// </summary>
    /// <param name="gridId"></param>
    /// <returns></returns>
    public Transform GetWall(Vector2Int gridId)
    {
        //+1 because from the center of the cell is 1 unit to the edge to -1 x and -1 z
        try
        {
            return walls[gridId.x - gridPos.x + 1, gridId.y - gridPos.x + 1];
        }
        catch
        {
            return null;
        }
    }

    private Vector2Int CalcWallId(Quaternion rotation)
    {
        if (rotation.eulerAngles.y < 0)
        {
            rotation.eulerAngles += new Vector3(0, 360, 0);
        }
        //here goes magic stuff with direction
        switch (rotation.eulerAngles.y)
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
