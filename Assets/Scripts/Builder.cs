using System;
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
    RoomBlueprint room;

    [SerializeField]
    GameObject floorPref;
    [SerializeField]
    GameObject wallPref;

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
                    room.VisualiseBlueprint(grid.GetWorldPosition(startPos), grid.GetWorldPosition(endPos));
                }
            }
            if(Input.GetMouseButtonUp(1))
            {
                isRightClick = false;
                room.ConfirmPart();
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

    public void CreateNewRoomPart()
    {
        room = new GameObject("Room").AddComponent<RoomBlueprint>();
        room.InitializeRoom(ref grid);
        //this is temporary
        room.floorPref = floorPref;
        room.wallPref = wallPref;
    }

    public void ToggleBuilding()
    {
        if(isBuilding)
        {
            isBuilding = false;
            room.InstanceRoom();
            room = null;
        }
        else
        {
            isBuilding = true;
            CreateNewRoomPart();
        }
        grid.ToggleGrid();
    }
}
