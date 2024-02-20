using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    public bool isBuilding = false;
    public bool isRightClick = false;
    
    [SerializeField]
    Grid grid;
    Vector2Int startPos;
    Vector2Int endPos;

    [SerializeField]
    RoomBlueprint newRoom;

    [SerializeField]
    Transform floorPref;
    [SerializeField]
    Transform wallPref;

    private void Update()
    {
        if(isBuilding)
        {
            //on Right click
            if (Input.GetMouseButtonDown(1))
            {
                startPos = GetMousePosition();
                isRightClick = true;
            }
            if(isRightClick)
            {
                if(endPos != GetMousePosition())
                {
                    endPos = GetMousePosition();
                    newRoom.VisualiseBlueprint(grid.GetWorldPosition(startPos), grid.GetWorldPosition(endPos));
                }
            }
            if(Input.GetMouseButtonUp(1))
            {
                newRoom.ConfirmBlueprint();
                isRightClick = false;
            }
            
        }
    }

    /// <summary>
    /// Get position of mouse on grid
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetMousePosition()
    {
        //make ray from camera to mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //if ray hit something
        if (Physics.Raycast(ray, out hit))
        {
            if(hit.transform.CompareTag("Grid"))
            {
                Vector3 hitPoint = hit.point;
                return grid.GetGridId(hitPoint);
            }
        }
        return new Vector2Int(-1, -1);
    }

    public void StartBuilding()
    {
        isBuilding = true;
        grid.ToggleGrid();
        newRoom = new GameObject("Room").AddComponent<RoomBlueprint>();
        newRoom.floorPref = floorPref.transform;
        newRoom.wallPref = wallPref.transform;
    }
}
