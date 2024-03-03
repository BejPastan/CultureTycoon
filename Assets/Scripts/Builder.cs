using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Builder : MonoBehaviour
{
    public bool isBuilding = false;
    public bool isLeftClick = false;
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
            //check if mouse is not above UI
            if(!EventSystem.current.IsPointerOverGameObject())
            {
                //building
                if (Input.GetMouseButtonDown(0))
                {
                    startPos = GetMousePosition();
                    isLeftClick = true;
                    room.CreateNewPart();
                }
                if (isLeftClick)
                {
                    if (endPos != GetMousePosition())
                    {
                        endPos = GetMousePosition();
                        room.PaintPart(startPos, endPos);
                    }
                }

                //removing
                if(!isLeftClick)
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        startPos = GetMousePosition();
                        isRightClick = true;
                    }
                    if (isRightClick)
                    {
                        if (endPos != GetMousePosition())
                        {
                            endPos = GetMousePosition();
                            room.EraseArea(startPos, endPos);
                        }
                    }
                }
            }
            if (isBuilding)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    isLeftClick = false;
                }
                if(Input.GetMouseButtonUp(1))
                {
                    isRightClick = false;
                }
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
        return new Vector2Int(0, 0);
    }

    /// <summary>
    /// Change game mode to editing selected Room
    /// </summary>
    private void StartEditing()
    {
        //create ne game object for room part
        room = new GameObject("Room").AddComponent<RoomBlueprint>();
        room.createNewBlueprint(ref grid, wallPref, floorPref);
    }

    /// <summary>
    /// Exit game from editing mode
    /// </summary>
    private void EndEditing()
    {
        room.ConfirmBlueprint();
        room = null;
    }

    /// <summary>
    /// Change to and from Editing mode
    /// </summary>
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
