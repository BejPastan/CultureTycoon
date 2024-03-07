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

    /// <summary>
    /// Creating
    /// </summary>
    private void CreateGrid()
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

    /// <summary>
    /// change grid from showing to not showing and vice versa
    /// </summary>
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

    /// <summary>
    /// visualization of grid cells on the scene
    /// </summary>
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

    /// <summary>
    /// hide visualization of grid cells on the scene
    /// </summary>
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

    /// <summary>
    /// Get X and Y position of cell in selected position on world. If there is no grid return closest cell
    /// </summary>
    /// <param name="position">position of cell in the world</param>
    /// <returns></returns>
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

    /// <summary>
    /// Get positon of cell in the world by its X nad Y position in grid array
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Vector3 GetWorldPosition(Vector2Int id)
    {
        Vector3 pos = new Vector3(id.x * cellSize, 0, id.y * cellSize)+origin;
        return pos;
    }

    /// <summary>
    /// Change grid state in selected range, from one state to another
    /// </summary>
    /// <param name="changeThisState">cells with this state will be changed</param>
    /// <param name="toThisState">targeted state of cells</param>
    /// <param name="rangeStart">range beetwen which cells will be changed</param>
    /// <param name="rangeEnd">range beetwen which cells will be changed</param>
    public void ChangeGridState(GridState changeThisState, GridState toThisState, Vector2Int rangeStart, Vector2Int rangeEnd)
    {
        //change all blueprint to room
        for (int x = rangeStart.x; x < rangeEnd.x; x++)
        {
            for (int z = rangeStart.y; z < rangeEnd.y; z++)
            {
                if (x >= 0 && x < width && z >= 0 && z < depth)
                    if (gridStates[x, z] == changeThisState)
                    {
                        gridStates[x, z] = toThisState;
                    }  
            }
        }
    }

    /// <summary>
    /// Change state of all cells with selected state to another state
    /// </summary>
    /// <param name="changeThisState">selected state</param>
    /// <param name="toThisState">targeted state</param>
    public void ChangeGridState(GridState changeThisState, GridState toThisState)
    {
        ChangeGridState(changeThisState, toThisState, new Vector2Int(0, 0), new Vector2Int(width, depth));
    }    

    /// <summary>
    /// change state of all cells in selected range
    /// </summary>
    /// <param name="toThisState"></param>
    /// <param name="rangeStart"></param>
    /// <param name="rangeEnd"></param>
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

    /// <summary>
    /// change state of selected cell
    /// </summary>
    /// <param name="toThisState"></param>
    /// <param name="id"></param>
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