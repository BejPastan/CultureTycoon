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
    Vector2Int mouseGridPos;
    [SerializeField] Material defaultMaterial;
    bool relocation = false;

    //rotating object

    //moving object

    //snapping to grid

    //placing object

    public void StartPlacing(GameObject objectPref)
    {
        Debug.Log("StartPlacing");
        objectToPlace = Instantiate(objectPref).GetComponent<FurnitureData>();
        objectToPlace.StartMoving(defaultMaterial);
        isPlacing = true;
        grid.ToggleGrid();
        relocation = false;
    }

    public void StartPlacing(FurnitureData furniture)
    {
        Debug.Log("StartPlacing");
        objectToPlace = furniture;
        objectToPlace.StartMoving(defaultMaterial);
        isPlacing = true;
        grid.ToggleGrid();
        relocation = true;
    }

    public void Place()
    {
        isPlacing = false;
        grid.ToggleGrid();
        objectToPlace.Place();
        objectToPlace = null;
        relocation = false;
    }

    public void Cancel()
    {
        Destroy(objectToPlace.gameObject);
        isPlacing = false;
        grid.ToggleGrid();
        relocation = false;
    }

    public void Remove()
    {
        Destroy(objectToPlace.gameObject);
        isPlacing = false;
        grid.ToggleGrid();
        relocation = false;
    }

    private void Update()
    {
        if (isPlacing)
        {
            InputHandler();
        }
    }

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
