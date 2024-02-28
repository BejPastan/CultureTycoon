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
            if (Input.GetMouseButtonDown(0))
            {
                startPos = GetMousePosition();
                isRightClick = true;
                room.CreateNewPart();
            }
            if(isRightClick)
            {
                if(endPos != GetMousePosition())
                {
                    endPos = GetMousePosition();
                    room.ChangeSize(startPos, endPos);
                }
            }
            if(Input.GetMouseButtonUp(0))
            {
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
            if(hit.transform.CompareTag("Ground"))
            {
                Vector3 hitPoint = hit.point;
                return grid.GetGridId(hitPoint);
            }
        }
        return new Vector2Int(-1, -1);
    }

    private void StartEditing()
    {
        //create ne game object for room part
        room = new GameObject("Room").AddComponent<RoomBlueprint>();
        room.createNewBlueprint(ref grid, wallPref, floorPref);
    }

    private void EndEditing()
    {
        room.ConfirmBlueprint();
        room = null;
    }

    public void ToggleBuilding()
    {
        if(isBuilding)
        {
            isBuilding = false;
            EndEditing();
        }
        else
        {
            isBuilding = true;
            StartEditing();
        }
        grid.ToggleGrid();
    }
}
