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
    public bool buildingDoor = false;
    
    [SerializeField]
    Grid grid;
    Vector2Int startPos;
    Vector2Int endPos;

    [SerializeField]
    RoomBlueprint roomBP;
    [SerializeField]
    Room room;

    [SerializeField]
    GameObject roomPref;

    [SerializeField]
    GameObject floorPref;
    [SerializeField]
    GameObject wallPref;
    [SerializeField]
    GameObject doorPref;

    private void Update()
    {
        Building();
    }

    private void Building()
    {
        if (isBuilding)
        {
            if (buildingDoor)
            {
                Debug.Log("Building door");
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("Building door");
                    Transform wall = GetObjectUnderMouse();
                    Debug.Log(wall);
                    if (wall != null && wall.CompareTag("Wall"))
                    {
                        Debug.Log("Building door");
                        roomBP.SetDoors(GetMousePosition(), wall.rotation);
                        EndEditing();
                        return;
                    }
                }
            }
            else
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    //building
                    if (Input.GetMouseButtonDown(0))
                    {
                        startPos = GetMousePosition();
                        isLeftClick = true;
                        roomBP.CreateNewPart();
                    }
                    if (isLeftClick)
                    {
                        if (endPos != GetMousePosition())
                        {
                            endPos = GetMousePosition();
                            roomBP.PaintPart(startPos, endPos);
                        }
                    }

                    //removing
                    if (!isLeftClick)
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
                                roomBP.EraseArea(startPos, endPos);
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
                    if (Input.GetMouseButtonUp(1))
                    {
                        isRightClick = false;
                    }
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
        RaycastHit[] hit;
        //get multiple hits
        hit = Physics.RaycastAll(ray);
        if (hit.Length > 0)
        {
            foreach (RaycastHit h in hit)
            {
                if (h.transform.CompareTag("Ground"))
                {
                    Vector3 hitPoint = h.point;
                    return grid.GetGridId(hitPoint);
                }
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
        isBuilding = true;
        room = Instantiate(roomPref, Vector3.zero, Quaternion.identity).GetComponent<Room>();
        room.OnCreate(this);
        roomBP = room.gameObject.GetComponent<RoomBlueprint>();
        roomBP.createNewBlueprint(ref grid, wallPref, floorPref, doorPref);
        grid.ToggleGrid();
        roomBP.DisableCollision();
    }

    public void StartEditing(Room room)
    {
        isBuilding = true;
        this.room = room;
        roomBP = room.GetComponent<RoomBlueprint>();
        grid.ToggleGrid();
        roomBP.DisableCollision();
    }

    /// <summary>
    /// Exit game from editing mode
    /// </summary>
    private void EndEditing()
    {
        buildingDoor = false;
        isBuilding = false;
        room.ConfirmRoom(roomBP);
        roomBP = null;
        grid.ToggleGrid();
        room = null;
    }

    public void CancelEditing()
    {
        buildingDoor = false;
        isBuilding = false;
        room.CancelEditing(ref roomBP);
    }

    private void StartBuildingDoor()
    {
        buildingDoor = true;
        roomBP.EnableCollision();
    }

    private Transform GetObjectUnderMouse()
    {
        Debug.Log("Getting object under mouse");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.transform;
        }
        return null;
    }

    /// <summary>
    /// Change to and from Editing mode
    /// </summary>
    public void ToggleBuilding()
    {
        if(isBuilding)
        {
            StartBuildingDoor();            
        }
        else
        {
            StartEditing();
        }
    }
}
