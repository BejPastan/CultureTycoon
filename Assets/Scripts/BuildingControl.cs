using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingControl : MonoBehaviour
{
    public bool isBuilding = false;
    public bool isRightClick = false;
    
    [SerializeField]
    Grid grid;
    Vector2Int startPos;
    Vector2Int endPos;

    [SerializeField]
    Room room;

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
                startPos = GetPosition();
                isRightClick = true;
            }
            if(isRightClick)
            {
                if(endPos != GetPosition())
                {
                    endPos = GetPosition();
                    room.VisualiseBlueprint(grid.GetWorldPosition(startPos), grid.GetWorldPosition(endPos));
                }
            }
            if(Input.GetMouseButtonUp(1))
            {
                isRightClick = false;
            }
            
        }
    }

    public Vector2Int GetPosition()
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
        room = new GameObject("Room").AddComponent<Room>();
        room.floorPref = floorPref.transform;
        room.wallPref = wallPref.transform;
    }


}
