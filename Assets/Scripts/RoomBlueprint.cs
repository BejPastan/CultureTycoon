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
                if(grid.gridStates[newShift.x + x, newShift.y + z] == GridState.free)
                {
                    newParts[x, z] = CreateFloor(new Vector3(startPos.x + x-1, 0, startPos.z + z-1));
                }               
            }
        }
        SetWalls(ref newParts, ref newShift);
    }

    //this is only temporary, this shit need any logic
    public void ConfirmPart()
    {
        Vector2Int maxShift = newShift;
        if (maxShift.x < partsShift.x)
            maxShift.x = partsShift.x;
        if (maxShift.y < partsShift.y)
            maxShift.y = partsShift.y;

        Transform[,] temp = new Transform[parts.GetLength(0) + newParts.GetLength(0) - (partsShift.x - newShift.x), parts.GetLength(1) + newParts.GetLength(1) - (partsShift.y - newShift.y)];
        Vector2Int id;
        for (int x = 0; x < temp.GetLength(0); x++)
        {
            for (int z = 0; z < temp.GetLength(1); z++)
            {
                id = new Vector2Int((partsShift.x - maxShift.x) + x, (partsShift.y - maxShift.y) + z);
                if (id.x >= 0 && id.y >= 0 && id.x < parts.GetLength(0) && id.y < parts.GetLength(1))
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
        if (grid.gridStates[gridID.x, gridID.y] == GridState.free)
        {
            grid.gridStates[gridID.x, gridID.y] = GridState.blueprint;
            return Instantiate(floorPref, pos, Quaternion.identity, transform).transform;
        }
        else
        {
            return null;
        }
    }

    private void SetWalls(ref Transform[,] refParts, ref Vector2Int shift)
    {
        /*/iterate through newParts array
        for (int i = 0; i < newParts.GetLength(0); i++)
        {
            for (int j = 0; j < newParts.GetLength(1); j++)
            {
                Vector2Int gridId = grid.GetGridId(new Vector3(i + newShift.x + grid.origin.x, 0, j + newShift.y + grid.origin.z));

                if (grid.gridStates[gridId.x, gridId.y] == GridState.blueprint)
                {
                    //debug i and j
                    Debug.Log("i: " + i + " j: " + j + " grid id: " + gridId);

                    Vector2Int floorPos = new Vector2Int(i, j) - newShift;

                    //Vector2Int floorPos = new Vector2Int(Mathf.RoundToInt(newParts[i,j].position.x), Mathf.RoundToInt(newParts[i, j].position.z));
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
        }*/

        int width = refParts.GetLength(0);
        int depth = refParts.GetLength(1);

        for (int x = 1; x < width-1; x++)
        {
            for(int z = 1; z<depth-1; z++)
            {
                //get grid id
                Vector3 gridPos = new Vector3(x + shift.x + grid.origin.x, 0, z + shift.y + grid.origin.z);
                Vector2Int gridId = grid.GetGridId(gridPos);
                
                if (grid.gridStates[gridId.x, gridId.y] == GridState.blueprint )
                {
                    Debug.Log("gridId: " + gridId + "parts Array: "+x+", "+z);
                    
                    //check if there is a wall
                    Vector2Int floorPos = new Vector2Int(Mathf.RoundToInt(gridPos.x), Mathf.RoundToInt(gridPos.z));
                    if ( /*refParts[x, z + 1] == null &&*/ grid.gridStates[gridId.x, gridId.y+1] == GridState.free)
                    {
                        refParts[x, z + 1] = CreateWall(floorPos, new Vector2Int(0, -1));
                    }
                    if ( /*refParts[x, z - 1] == null &&*/ grid.gridStates[gridId.x, gridId.y-1] == GridState.free)
                    {
                        refParts[x, z - 1] = CreateWall(floorPos, new Vector2Int(0, 1));
                    }
                    if ( /*refParts[x + 1, z] == null &&*/ grid.gridStates[gridId.x+1, gridId.y] == GridState.free)
                    {
                        refParts[x + 1, z] = CreateWall(floorPos, new Vector2Int(-1, 0));
                    }
                    if ( /*refParts[x - 1, z] == null &&*/ grid.gridStates[gridId.x-1, gridId.y] == GridState.free)
                    {
                        refParts[x - 1, z] = CreateWall(floorPos, new Vector2Int(1, 0));
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
        return Instantiate(wallPref, new Vector3(wallPos.x, 0, wallPos.y), rotation, transform).transform;
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
                    Debug.Log("Destroying: " + newParts[i, j].position + " name: " + newParts[i,j]);
                    Vector2Int gridID = grid.GetGridId(newParts[i, j].position);
                    if (newParts[i, j].gameObject.CompareTag("Floor"))
                    {
                        grid.gridStates[gridID.x, gridID.y] = GridState.free;
                    }
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
