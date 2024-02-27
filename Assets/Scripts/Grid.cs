using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField]
    bool isShowing = true;
    [SerializeField]
    int width = 10;
    [SerializeField]
    int depth = 10;
    [SerializeField]
    public GridState[,] gridStates;
    [SerializeField]
    float cellSize = 1;
    [SerializeField]
    GameObject cellPref;
    [SerializeField]
    GameObject[,] cellsVisual;
    [SerializeField]
    public Vector3 origin;

    private void Start()
    {
        CreateGrid();
    }

    public void CreateGrid()
    {
        gridStates = new GridState[width, depth];
        cellsVisual = new GameObject[width, depth];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                gridStates[i, j] = GridState.free;
            }
        }
    }

    public void ToggleGrid()
    {
        if(isShowing)
        {
            isShowing = false;
            DisableGrid();
        }
        else
        {
            isShowing = true;
            ShowGrid();
        }
    }

    private void ShowGrid()
    {
        cellsVisual = new GameObject[width, depth];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                if (gridStates[i, j] == GridState.free)
                    cellsVisual[i, j] = Instantiate(cellPref, new Vector3(i * cellSize, 0, j * cellSize) + origin, Quaternion.identity);
            }
        }
    }

    private void DisableGrid()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                if (cellsVisual[i, j] != null)
                    Destroy(cellsVisual[i, j]);
            }
        }
        cellsVisual = new GameObject[0, 0];
    }

    public Vector2Int GetGridId(Vector3 position)
    {
        position -= origin;
        Vector2Int gridId = new Vector2Int(Mathf.RoundToInt(position.x / cellSize), Mathf.RoundToInt(position.z / cellSize));
        if (gridId.x < 0)
            gridId.x = 0;
        if (gridId.x >= width)
            gridId.x = width - 1;
        if (gridId.y < 0)
            gridId.y = 0;
        if(gridId.y >= depth)
            gridId.y = depth - 1;
        return gridId;
    }

    public Vector3 GetWorldPosition(Vector2Int id)
    {
        Vector3 pos = new Vector3(id.x * cellSize, 0, id.y * cellSize)+origin;
        return pos;
    }

    public void ChangeGridState(GridState changeThisState, GridState toThisState, Vector2Int rangeStart, Vector2Int rangeEnd)
    {
        //change all blueprint to room
        for (int x = rangeStart.x; x < rangeEnd.x; x++)
        {
            for (int z = rangeStart.y; z < rangeEnd.y; z++)
            {
                if (x >= 0 && x < width && z >= 0 && z < depth)
                    Debug.Log("Grid state at " + x + " " + z + " is " + gridStates[x, z]);
                    if (gridStates[x, z] == changeThisState)
                    {
                        gridStates[x, z] = toThisState;
                        Debug.Log("Changed grid state at " + x + " " + z + " to " + toThisState);
                    }  
            }
        }
    }

    public void ChangeGridState(GridState changeThisState, GridState toThisState)
    {
        ChangeGridState(changeThisState, toThisState, new Vector2Int(0, 0), new Vector2Int(width, depth));
    }    

    public void ChangeGridState(GridState toThisState, Vector2Int rangeStart, Vector2Int rangeEnd)
    {
        for (int x = rangeStart.x; x < rangeEnd.x; x++)
        {
            for (int z = rangeStart.y; z < rangeEnd.y; z++)
            {
                gridStates[x, z] = toThisState;
            }
        }
    }

    public void ChangeGridState(GridState toThisState, Vector2Int id)
    {
        gridStates[id.x, id.y] = toThisState;
    }
}

public enum GridState
{
    free,
    room,
    blueprint,
    outside
}