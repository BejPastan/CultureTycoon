using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class FurniturePlacer : MonoBehaviour
{
    bool isPlacing = false;
    [SerializeField] FurnitureData objectToPlace;
    [SerializeField] Grid grid;
    [SerializeField] BudgetManager budgetManager;
    Vector2Int mouseGridPos;
    [SerializeField] Material defaultMaterial;
    bool relocation = false;
    public bool enabled = true;

    /// <summary>
    /// Create new object to place and start placing it
    /// </summary>
    /// <param name="objectPref"></param>
    public void StartPlacing(GameObject objectPref)
    {
        //Debug.Log("StartPlacing");
        objectToPlace = Instantiate(objectPref).GetComponent<FurnitureData>();
        objectToPlace.StartMoving(defaultMaterial);
        isPlacing = true;
        grid.ToggleGrid();
        relocation = false;

        HideUI();
    }

    /// <summary>
    /// start moving existing object on grid
    /// </summary>
    /// <param name="furniture"></param>
    public void StartPlacing(FurnitureData furniture)
    {
        if(!enabled)
        {
            return;
        }
        //Debug.Log("StartPlacing");
        objectToPlace = furniture;
        objectToPlace.StartMoving(defaultMaterial);
        isPlacing = true;
        grid.ToggleGrid();
        relocation = true;

        HideUI();
    }

    /// <summary>
    /// place object on grid
    /// </summary>
    public void Place()
    {
        if(!relocation)
        {
            if (!budgetManager.canBuild(objectToPlace.buildingCost))
            {
                //Debug.Log("Not enough money");//in future change to make prompt on UI
                return;
            }
            budgetManager.NewExpanse(objectToPlace.name, objectToPlace.buildingCost, 1);
            budgetManager.ChangeMaitenanceCost(objectToPlace.maintenanceCost);
        }
        isPlacing = false;
        grid.ToggleGrid();
        objectToPlace.Place();
        objectToPlace = null;
        relocation = false;

        FindObjectOfType<CameraControler>().EnableZoom();
        foreach (var rooms in FindObjectsOfType<RoomUI>())
        {
            rooms.ShowUI();
        }
    }

    /// <summary>
    /// cancel placing new obejct
    /// </summary>
    public void Cancel()
    {
        Destroy(objectToPlace.gameObject);
        isPlacing = false;
        grid.ToggleGrid();
        relocation = false;
        ShowUI();
    }

    /// <summary>
    /// remove existing object from grid
    /// </summary>
    public void Remove()
    {
        Destroy(objectToPlace.gameObject);
        isPlacing = false;
        grid.ToggleGrid();
        if(relocation)
        {
            budgetManager.NewExpanse(objectToPlace.name, Mathf.FloorToInt(objectToPlace.buildingCost * 0.25f), -1);
            budgetManager.ChangeMaitenanceCost(-objectToPlace.maintenanceCost);
        }
        relocation = false;
        
        ShowUI();
    }

    private void HideUI()
    {
        FindObjectOfType<CameraControler>().DisableZoom();
        foreach (var rooms in FindObjectsOfType<RoomUI>())
        {
            rooms.HideUI();
        }
    }

    private void ShowUI()
    {
        FindObjectOfType<CameraControler>().EnableZoom();
        foreach (var rooms in FindObjectsOfType<RoomUI>())
        {
            rooms.ShowUI();
        }
    }

    private void Update()
    {
        if (isPlacing)
        {
            InputHandler();
        }
    }

    /// <summary>
    /// handel player input
    /// </summary>
    /// <returns></returns>
    private async Task InputHandler()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if(objectToPlace.canBuild)
            { Place(); }
            return;
        }
        if (Input.GetKey(KeyCode.Escape) && !relocation)
        {
            Cancel();
            return;
        }
        if (Input.GetKey(KeyCode.Delete) && relocation)
        {
            Remove();
            return;
        }
        if (mouseGridPos != MouseGridPos())
        {
            mouseGridPos = MouseGridPos();
            objectToPlace.SetOnGrid(mouseGridPos);
            objectToPlace.CheckConditions();
        }
        if (Input.mouseScrollDelta.y != 0)
        {
            objectToPlace.Rotate((int)Input.mouseScrollDelta.y);
            objectToPlace.CheckConditions();
        }
    }

    /// <summary>
    /// get mouse position on grid
    /// </summary>
    /// <returns></returns>
    private Vector2Int MouseGridPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hit;
        hit = Physics.RaycastAll(ray, 100.0f);
        foreach (RaycastHit h in hit)
        {
            if (h.transform.CompareTag("Ground"))
            {
                Vector2Int gridId = grid.GetGridId(h.point);
                return gridId;
                
            }
        }
        return new Vector2Int(0, 0);
    }
}
