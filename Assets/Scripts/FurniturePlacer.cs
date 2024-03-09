using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FurniturePlacer : MonoBehaviour
{
    [SerializeField] bool isPlacing = false;
    [SerializeField] FurnitureData objectToPlace;
    [SerializeField] Grid grid;
    Vector2Int mouseGridPos;

    //rotating object

    //moving object

    //snapping to grid

    //placing object

    public void StartPlacing(GameObject objectPref)
    {
        Debug.Log("StartPlacing");
        objectToPlace = Instantiate(objectPref).GetComponent<FurnitureData>();
        isPlacing = true;
        grid.ToggleGrid();
    }

    public void Place()
    {
        objectToPlace = null;
        isPlacing = false;
        grid.ToggleGrid();
    }

    public void Cancel()
    {
        Destroy(objectToPlace.gameObject);
        isPlacing = false;
        grid.ToggleGrid();
    }

    private void Update()
    {
        if(isPlacing)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Place();
                return;
            }
            if(Input.GetKey(KeyCode.Escape))
            {
                Cancel();
                return;
            }
            if(mouseGridPos != MouseGridPos())
            {
                mouseGridPos = MouseGridPos();
                objectToPlace.SetOnGrid(mouseGridPos);
            }
            if(Input.mouseScrollDelta.y != 0)
            {
                objectToPlace.Rotate((int)Input.mouseScrollDelta.y);
            }
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
                return grid.GetGridId(h.point);
                
            }
        }
        return new Vector2Int(0, 0);
    }
}
