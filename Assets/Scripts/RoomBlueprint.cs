using System;
using UnityEditor;
using UnityEngine;

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
    Vector2Int tempShift;

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

        tempShift = CalcShift(startPos-Vector3.one);

        for (int x = 1; x <= width; x++)
        {
            for (int z = 1; z <= depth; z++)
            {
                //check if in this position is empty
                Debug.Log("Checking: " + (tempShift.x + x) + " " + (tempShift.y + z));
                Debug.Log("tempShift: " + tempShift.x + " " + tempShift.y);
                if(grid.gridStates[tempShift.x + x, tempShift.y + z] == GridState.free)
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
        parts = newParts;
        partsShift = tempShift;
    }

    private Transform CreateFloor(Vector3 pos)
    {
        Vector2Int gridID = grid.GetGridId(pos);
        grid.gridStates[gridID.x, gridID.y] = GridState.blueprint;
        return Instantiate(floorPref, pos, Quaternion.identity).transform;
    }

    //this must also handle walls in part array
    private void SetWalls()
    {
        //iterate through parts array
        for (int i = 0; i < newParts.GetLength(0); i++)
        {
            for (int j = 0; j < newParts.GetLength(1); j++)
            {
                Vector2Int gridId = grid.GetGridId(new Vector3(i + tempShift.x + grid.origin.x, 0, j + tempShift.y + grid.origin.z));

                if (grid.gridStates[gridId.x, gridId.y] == GridState.blueprint)
                {
                    Vector2Int floorPos = new Vector2Int(Mathf.RoundToInt(newParts[i,j].position.x), Mathf.RoundToInt(newParts[i, j].position.z));
                    if (newParts[i, j + 1] == null)
                    {
                        newParts[i, j + 1] = CreateWall(floorPos, new Vector2Int(0, -1));
                    }
                    if (newParts[i, j - 1] == null)
                    {
                        newParts[i, j - 1] = CreateWall(floorPos, new Vector2Int(0, 1));
                    }
                    if (newParts[i + 1, j] == null)
                    {
                        newParts[i + 1, j] = CreateWall(floorPos, new Vector2Int(-1, 0));
                    }
                    if (newParts[i - 1, j] == null)
                    {
                        newParts[i - 1, j] = CreateWall(floorPos, new Vector2Int(1, 0));
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
        Debug.Log("CalcShift: " + pos);
        Debug.Log("grid.origin: " + grid.origin);
        return new Vector2Int(Mathf.RoundToInt(pos.x - grid.origin.x), Mathf.RoundToInt(pos.z - grid.origin.z));
    }
}
