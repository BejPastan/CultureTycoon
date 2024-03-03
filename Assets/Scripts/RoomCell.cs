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

    /// <summary>
    /// Create wall in this cell in given orientation
    /// </summary>
    /// <param name="gridId"></param>
    /// <param name="pref"></param>
    /// <param name="orientation"></param>
    public void CreateWall(Vector2Int gridId, ref GameObject pref, Vector2Int orientation)
    {
        Vector3 pos = roomPart.grid.GetWorldPosition(gridPos);
        //here goes magic stuff with direction
        Quaternion rotation = Quaternion.Euler(0, (90 * orientation.y * (orientation.y + 1)) + ((orientation.x * - 90) + 90), 0);
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

    /// <summary>
    /// Remove wall by local position in cell
    /// </summary>
    /// <param name="id"></param>
    private void RemoveWall(Vector2Int id)
    {
        if (walls[id.x, id.y] != null)
        {
            GameObject.Destroy(walls[id.x, id.y].gameObject);
            walls[id.x, id.y] = null;
        }
    }

    /// <summary>
    /// Get X and Y of cell in roomPart array
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetFloorPos()
    {
        return gridPos;
    }

    /// <summary>
    /// Get transform of floor in this cell
    /// </summary>
    /// <returns></returns>
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
            return walls[gridId.x - gridPos.x + 1, gridId.y - gridPos.y + 1];
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Get wall by which side of cell it is
    /// </summary>
    /// <param name="localPos">which side of the cell is wall</param>
    /// <returns></returns>
    public Transform GetWallByLocalPos(Vector2Int localPos)
    {
        return walls[localPos.x, localPos.y];
    }

    /// <summary>
    /// Change wall object in cell
    /// </summary>
    /// <param name="wallPref">new wall object</param>
    /// <param name="wallId">local id in cell</param>
    public void ChangeWallObject(GameObject wallPref, Vector2Int wallId)
    {
        GameObject.Destroy(walls[wallId.x, wallId.y].gameObject);
        walls[wallId.x, wallId.y] = GameObject.Instantiate(wallPref, roomPart.grid.GetWorldPosition(gridPos), Quaternion.identity).transform;
    }

    /// <summary>
    /// calculate wall local position in cell by rotation
    /// </summary>
    /// <param name="rotation">rotation of wall</param>
    /// <returns></returns>
    public Vector2Int CalcWallId(Quaternion rotation)
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
