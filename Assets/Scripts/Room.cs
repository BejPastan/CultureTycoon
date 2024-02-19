using UnityEngine;

public class Room : MonoBehaviour
{
    Transform[,] walls = new Transform[0,0];
    Transform[,] floors = new Transform[0,0];

    [SerializeField]
    public Transform wallPref;
    [SerializeField]
    public Transform floorPref;

    public void VisualiseBlueprint(Vector3 startPos, Vector3 endPos)
    {
        ClearBlueprint();

        int width = (int)Mathf.Abs(startPos.x - endPos.x)+1;
        int depth = (int)Mathf.Abs(startPos.z - endPos.z)+1;
        
        walls = new Transform[width+2, depth+2];
        floors = new Transform[width, depth];
        Debug.Log("Width: " + width + " Depth: " + depth);


        if(startPos.x > endPos.x)
        {
            float temp = startPos.x;
            startPos.x = endPos.x;
            endPos.x = temp;
        }

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                floors[x, z] = Instantiate(floorPref, new Vector3(startPos.x + x, 0, startPos.z + z), Quaternion.identity);
            }
        }

        for(int x = 1; x <= width; x++)
        {
            walls[x, 0] = Instantiate(wallPref, new Vector3(startPos.x + x - 1, 0, startPos.z), Quaternion.Euler(0, 90, 0));
            walls[x, depth+1] = Instantiate(wallPref, new Vector3(startPos.x + x - 1, 0, endPos.z), Quaternion.Euler(0, -90, 0));
        }
        for(int z = 1; z <= depth; z++)
        {
            walls[0, z] = Instantiate(wallPref, new Vector3(startPos.x, 0, startPos.z + z - 1), Quaternion.Euler(0, 180, 0));
            walls[width+1, z] = Instantiate(wallPref, new Vector3(endPos.x, 0, startPos.z + z - 1), Quaternion.Euler(0, 0, 0));
        }
    }

    public void ClearBlueprint()
    {
        //debug lengths of walls and floors
        Debug.Log("Walls: " + walls.GetLength(0) + " " + walls.GetLength(1));
        //Debug.Log("Floors: " + floors.GetLength(0) + " " + floors.GetLength(1));
        //destroy walls
        for (int i = 0; i < walls.GetLength(0); i++)
        {
            for (int j = 0; j < walls.GetLength(1); j++)
            {
                Debug.Log("i: " + i + " j: " + j);
                if (walls[i,j] != null)
                {
                    Debug.Log(walls[i, j].gameObject);
                    Destroy(walls[i,j].gameObject);
                }
                else
                {
                    Debug.Log("Not destroyed");
                }
            }
        }
        //destroy floors
        for (int i = 0; i < floors.GetLength(0); i++)
        {
            for (int j = 0; j < floors.GetLength(1); j++)
            {
                if (floors[i,j] != null)
                {
                    Destroy(floors[i,j].gameObject);
                }
            }
        }
    }
}
