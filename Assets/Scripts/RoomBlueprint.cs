using UnityEngine;

public class RoomBlueprint : MonoBehaviour
{
    Transform[,] tempParts = new Transform[0,0];
    Transform[,] tempFloors = new Transform[0,0];

    Transform[,] parts;

    [SerializeField]
    public Transform wallPref;
    [SerializeField]
    public Transform floorPref;

    public void VisualiseBlueprint(Vector3 startPos, Vector3 endPos)
    {
        ClearBlueprint();

        int width = (int)Mathf.Abs(startPos.x - endPos.x)+1;
        int depth = (int)Mathf.Abs(startPos.z - endPos.z)+1;
        
        tempParts = new Transform[width+2, depth+2];
        //tempFloors = new Transform[width, depth];
        Debug.Log("Width: " + width + " Depth: " + depth);


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

        for (int x = 1; x <= width; x++)
        {
            for (int z = 1; z <= depth; z++)
            {
                tempParts[x, z] = Instantiate(floorPref, new Vector3(startPos.x + x-1, 0, startPos.z + z-1), Quaternion.identity);
            }
        }

        for(int x = 1; x <= width; x++)
        {
            tempParts[x, 0] = Instantiate(wallPref, new Vector3(startPos.x + x - 1, 0, startPos.z), Quaternion.Euler(0, 90, 0));
            tempParts[x, tempParts.GetLength(1)-1] = Instantiate(wallPref, new Vector3(startPos.x + x - 1, 0, endPos.z), Quaternion.Euler(0, -90, 0));
        }
        for (int z = 1; z <= depth; z++)
        {
            tempParts[0, z] = Instantiate(wallPref, new Vector3(startPos.x, 0, startPos.z + z - 1), Quaternion.Euler(0, 180, 0));
            Debug.Log("Z: " + z + " Walls: " + (tempParts.GetLength(1) - 1));
            tempParts[tempParts.GetLength(0) - 1, z] = Instantiate(wallPref, new Vector3(endPos.x, 0, startPos.z + z - 1), Quaternion.Euler(0, 0, 0));
        }
    }

    public void ConfirmBlueprint()
    {
        
    }

    public void ClearBlueprint()
    {
        //destroy walls
        for (int i = 0; i < tempParts.GetLength(0); i++)
        {
            for (int j = 0; j < tempParts.GetLength(1); j++)
            {
                Debug.Log("i: " + i + " j: " + j);
                if (tempParts[i,j] != null)
                {
                    Destroy(tempParts[i,j].gameObject);
                }
                else
                {
                    Debug.Log("Not destroyed");
                }
            }
        }
        //destroy floors
        /*for (int i = 0; i < tempFloors.GetLength(0); i++)
        {
            for (int j = 0; j < tempFloors.GetLength(1); j++)
            {
                if (tempFloors[i,j] != null)
                {
                    Destroy(tempFloors[i,j].gameObject);
                }
            }
        }*/
    }
}
