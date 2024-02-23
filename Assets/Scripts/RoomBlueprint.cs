using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class RoomBlueprint : MonoBehaviour
{
    Transform[,] newParts = new Transform[0,0];
    Transform[,] parts = new Transform[0,0];

    [SerializeField]
    public GameObject wallPref;
    [SerializeField]
    public GameObject floorPref;

    Grid grid;

    Vector2Int partsShift;
    Vector2Int newShift;

    public void InitializeRoom(ref Grid grid)
    {
        this.grid = grid;
    }
    
    public void InstanceRoom()
    {
        grid.ConfirmRoom();
    }

    public void VisualiseBlueprint(Vector3 startPos, Vector3 endPos)
    {
        ClearBlueprint();
        if (parts.GetLength(0) == 0)
        {
            parts = newParts;
            partsShift = newShift;
        }

        int width = (int)Mathf.Abs(startPos.x - endPos.x)+1;
        int depth = (int)Mathf.Abs(startPos.z - endPos.z)+1;        
        newParts = new Transform[width+2, depth+2];


        //swich start and end pos if needed
        if(startPos.x > endPos.x)
        {
            float temp = startPos.x;
            startPos.x = endPos.x;
            endPos.x = temp;
        }

        if(startPos.z > endPos.z)
        {
            float temp = startPos.z;
            startPos.z = endPos.z;
            endPos.z = temp;
        }

        newShift = CalcShift(startPos-Vector3.one);

        for (int x = 1; x <= width; x++)
        {
            for (int z = 1; z <= depth; z++)
            {
                //check if in this position is empty
                //Debug.Log("Checking: " + (newShift.x + x) + " " + (newShift.y + z));
                //Debug.Log("tempShift: " + newShift.x + " " + newShift.y);
                if(grid.gridStates[newShift.x + x, newShift.y + z] == GridState.free)
                {
                    newParts[x, z] = CreateFloor(new Vector3(startPos.x + x-1, 0, startPos.z + z-1));
                    //debug
                }               
            }
        }
        SetWalls();
    }

    //this is only temporary, this shit need any logic
    public void ConfirmPart()
    {
            Vector2Int maxShift = newShift;
            if(maxShift.x < partsShift.x)
                maxShift.x = partsShift.x;
            if( maxShift.y < partsShift.y)
                maxShift.y = partsShift.y;

            Transform[,] temp = new Transform[parts.GetLength(0) + newParts.GetLength(0) - (partsShift.x - newShift.x), parts.GetLength(1) + newParts.GetLength(1) - (partsShift.y - newShift.y)];
            Vector2Int id;
            for(int x = 0; x < temp.GetLength(0); x++)
            {
                for(int z = 0; z < temp.GetLength(1); z++)
                {
                    id = new Vector2Int((partsShift.x - maxShift.x) + x, (partsShift.y - maxShift.y) + z);
                    if (id.x >= 0 && id.y >= 0 && id.x <parts.GetLength(0) && id.y<parts.GetLength(1))
                    {
                        temp[x, z] = parts[id.x, id.y];
                    }

                    id = new Vector2Int((newShift.x - maxShift.x) + x, (newShift.y - maxShift.y) + z);
                    if (id.x >= 0 && id.y >= 0 && id.x < newParts.GetLength(0) && id.y < newParts.GetLength(1))
                    {
                        temp[x, z] = newParts[id.x, id.y];
                    }
                }
            }

            parts = temp;
        newParts = new Transform[0, 0];
    }

    private Transform CreateFloor(Vector3 pos)
    {
        Vector2Int gridID = grid.GetGridId(pos);
        grid.gridStates[gridID.x, gridID.y] = GridState.blueprint;
        return Instantiate(floorPref, pos, Quaternion.identity).transform;
    }

    //this must also handle walls in part array

    /* soooooo
     * this shit below must be remake to
     * 1. create walls around floors in newPart array
     * 2. NOT create walls if in this place is already room from part array
     * 3. create walls around part array
     */
    private void SetWalls()
    {
        ////iterate through newParts array
        //for (int i = 0; i < newParts.GetLength(0); i++)
        //{
        //    for (int j = 0; j < newParts.GetLength(1); j++)
        //    {
        //        Vector2Int gridId = grid.GetGridId(new Vector3(i + newShift.x + grid.origin.x, 0, j + newShift.y + grid.origin.z));

        //        if (grid.gridStates[gridId.x, gridId.y] == GridState.blueprint)
        //        {
        //            //debug i and j
        //            Debug.Log("i: " + i + " j: " + j + " grid id: " + gridId);

        //            Vector2Int floorPos = new Vector2Int(i, j) - newShift;

        //            //Vector2Int floorPos = new Vector2Int(Mathf.RoundToInt(newParts[i,j].position.x), Mathf.RoundToInt(newParts[i, j].position.z));
        //            if (newParts[i, j + 1] == null)
        //            {
        //                newParts[i, j + 1] = CreateWall(floorPos, new Vector2Int(0, -1));
        //            }
        //            if (newParts[i, j - 1] == null)
        //            {
        //                newParts[i, j - 1] = CreateWall(floorPos, new Vector2Int(0, 1));
        //            }
        //            if (newParts[i + 1, j] == null)
        //            {
        //                newParts[i + 1, j] = CreateWall(floorPos, new Vector2Int(-1, 0));
        //            }
        //            if (newParts[i - 1, j] == null)
        //            {
        //                newParts[i - 1, j] = CreateWall(floorPos, new Vector2Int(1, 0));
        //            }
        //        }
        //    }
        //}

        Vector2Int maxShift = newShift;
        if (maxShift.x < partsShift.x)
            maxShift.x = partsShift.x;
        if (maxShift.y < partsShift.y)
            maxShift.y = partsShift.y;

        int width = parts.GetLength(0) + newParts.GetLength(0) - (partsShift.x - newShift.x);
        int depth = parts.GetLength(1) + newParts.GetLength(1) - (partsShift.y - newShift.y);

        for (int x = 0; x < width; x++)
        {
            for(int z = 0; z<depth; z++)
            {
                //chack if in this position is floor(by checking if this position is marked as blueprint in grid)
                Vector2Int gridId = grid.GetGridId(new Vector3(x + maxShift.x + grid.origin.x, 0, z + maxShift.y + grid.origin.z));
                if (grid.gridStates[gridId.x, gridId.y] == GridState.blueprint)
                {
                    //now we need to check if around this position is floor
                    Vector2Int floorPos = new Vector2Int(x, z) - newShift;
                    Vector2Int id1 = new Vector2Int((partsShift.x - maxShift.x) + x, (partsShift.y - maxShift.y) + z);
                    Vector2Int id2 = new Vector2Int((newShift.x - maxShift.x) + x, (newShift.y - maxShift.y) + z);


                    if (newParts[id1.x, id1.y + 1] == null )
                    {
                        newParts[id1.x, id1.y + 1] = CreateWall(floorPos, new Vector2Int(0, -1));
                    }
                }
            }
        }

    }

    private Transform CreateWall(Vector2Int wallPos, Vector2Int faceDir)
    {
        //calc rotation in direction of faceDir
        Quaternion rotation = Quaternion.Euler(0, faceDir.x * 90+Mathf.Abs(faceDir.x*90) + faceDir.y * 90, 0);
        /*
         * so this is simplifyied we can have only 4 possibel rotations, and only X or Z can be 1 or -1. 
         * So we can just multiply the X and Z to get the rotation, +90 in x is to make suer that rotation is 0 or 180
         */
        //instantiate the wall
        //Debug.Log("Wall at: " + wallPos);
        return Instantiate(wallPref, new Vector3(wallPos.x, 0, wallPos.y), rotation).transform;
    }

    private void ClearBlueprint()
    {
        //destroy walls
        for (int i = 0; i < newParts.GetLength(0); i++)
        {
            for (int j = 0; j < newParts.GetLength(1); j++)
            {
                if (newParts[i,j] != null)
                {
                    Vector2Int gridID = grid.GetGridId(newParts[i, j].position);
                    grid.gridStates[gridID.x, gridID.y] = GridState.free;
                    Destroy(newParts[i,j].gameObject);
                }
            }
        }
    }

    private Vector2Int CalcShift(Vector3 pos)
    {
        //Debug.Log("CalcShift: " + pos);
        //Debug.Log("grid.origin: " + grid.origin);
        return new Vector2Int(Mathf.RoundToInt(pos.x - grid.origin.x), Mathf.RoundToInt(pos.z - grid.origin.z));
    }
}
