using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FurniturePlacer : MonoBehaviour
{
    bool isPlacing = false;
    bool canPlace = false;
    [SerializeField] FurnitureData objectToPlace;
    [SerializeField] Grid grid;
    Vector2Int mouseGridPos;
    [SerializeField] Material defaultMaterial;

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

    }

    public void Place()
    {
        isPlacing = false;
        grid.ToggleGrid();
        objectToPlace.Place();
        objectToPlace = null;
    }

    public void Cancel()
    {
        Destroy(objectToPlace.gameObject);
        isPlacing = false;
        grid.ToggleGrid();
    }

    private void Update()
    {
        if (isPlacing)
        {
            InputHandler();
        }
    }

    private void InputHandler()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(canPlace)
            { Place(); }
            return;
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            Cancel();
            return;
        }
        if (mouseGridPos != MouseGridPos())
        {
            mouseGridPos = MouseGridPos();
            objectToPlace.SetOnGrid(mouseGridPos);
            canPlace = objectToPlace.CheckConditions();
        }
        if (Input.mouseScrollDelta.y != 0)
        {
            objectToPlace.Rotate((int)Input.mouseScrollDelta.y);
            canPlace = objectToPlace.CheckConditions();
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
