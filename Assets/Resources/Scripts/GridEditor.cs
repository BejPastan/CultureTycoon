using UnityEngine;
using Newtonsoft.Json;

[ExecuteInEditMode]
public class GridEditor : MonoBehaviour
{
    [SerializeField] Grid grid;
    [SerializeField] bool run = false;
    bool enterEditMode = false;

    [SerializeField] Vector2Int startPos;
    [SerializeField] Vector2Int endPos;
    [SerializeField] GridState state;
    [SerializeField] bool acceptChanges = false;

    //private void OnValidate()
    //{
    //    Debug.Log("before delay");
    //    UnityEditor.EditorApplication.delayCall += () =>
    //    {
    //        Debug.Log("in delay");
    //        if (run)
    //        {
    //            if (enterEditMode)
    //            {
    //                if (acceptChanges)
    //                {
    //                    UpdateGrid();
    //                    return;
    //                }
    //                return;
    //            }
    //            else
    //            {
    //                grid.LoadGrid();
    //                grid.ToggleGrid();
    //                enterEditMode = true;
    //            }
    //        }
    //        if (!run)
    //        {
    //            if (enterEditMode)
    //            {
    //                grid.ToggleGrid();
    //                enterEditMode = false;
    //                grid.SaveGrid();
    //            }
    //        }
    //    };
    //    Debug.Log("afterDelay");
    //}

    private void UpdateGrid()
    {
        grid.ChangeGridState(state, startPos, endPos);
        grid.ToggleGrid();
        grid.ToggleGrid();
        startPos = new Vector2Int(0, 0);
        endPos = new Vector2Int(0, 0);
        acceptChanges = false;
    }
}

[ExecuteAlways]
public static class GridSaver
{
    
    public static void Save(GridState[,] gridState)
    {
        //get path to save
        string path = Application.dataPath + "/Resources/Grids";
        //seriaize grid to json
        string json = JsonConvert.SerializeObject(gridState);
        //serialize grid to json using Newtonsoft.Json
        //save json to file named grid.json
        System.IO.File.WriteAllText(path + "/grid.json", json);
    }

    public static GridState[,] Load()
    {
        //get path to load
        string path = Application.dataPath + "/Resources/Grids";
        //load json from file named grid.json
        //check if file exists
        if(!System.IO.File.Exists(path + "/grid.json"))
        {
            return null;
        }
        string json = System.IO.File.ReadAllText(path + "/grid.json");
        //deserialize json to grid
        GridState[,] gridState = JsonConvert.DeserializeObject<GridState[,]>(json);
        return gridState;


    }
}
