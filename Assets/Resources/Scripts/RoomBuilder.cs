using UnityEngine;
using UnityEngine.EventSystems;

public class RoomBuilder : MonoBehaviour
{
    public bool isBuilding = false;
    public bool isLeftClick = false;
    public bool isRightClick = false;
    public bool buildingDoor = false;
    
    [SerializeField]
    Grid grid;
    Vector2Int startPos;
    Vector2Int endPos;

    [SerializeField] BuilderUI uiControl;
    [SerializeField] BudgetManager budgetManager;

    [SerializeField]
    RoomBlueprint roomBP;
    [SerializeField]
    Room room;

    [SerializeField]
    GameObject roomPref;

    //[SerializeField]
    //GameObject floorPref;
    //[SerializeField]
    //GameObject wallPref;
    //[SerializeField]
    //GameObject doorPref;

    private void Update()
    {
        Building();
    }

    /// <summary>
    /// handle player input
    /// </summary>
    private void Building()
    {
        if (isBuilding)
        {
            if (buildingDoor)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Transform wall = GetObjectUnderMouse();
                    Debug.Log(wall);
                    if (wall != null && wall.CompareTag("Wall"))
                    {
                        roomBP.SetDoors(GetMousePosition(), wall.rotation);
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
    /// Change game mode to building a new room
    /// </summary>
    /// <param name="blueprint">Room type blueprint</param>
    public void StartEditing(RoomBlueprint roomBlueprint)
    {
        isBuilding = true;
        room = Instantiate(roomPref, Vector3.zero, Quaternion.identity).GetComponent<Room>();
        roomBP = Instantiate(roomBlueprint);
        roomBP.CreateNewBlueprint(ref grid, room.transform);
        room.OnCreate(this, roomBP);
        grid.ToggleGrid();
        roomBP.DisableCollision();
        uiControl.StartEditingRoom();
    }

    /// <summary>
    /// Change game mode to editing the selected room
    /// </summary>
    public void StartEditing(Room selectedRoom)
    {
        Debug.Log("start Editing");
        if (isBuilding)
            EndEditing();
        isBuilding = true;
        room = selectedRoom;
        grid.ToggleGrid();
        roomBP = room.StartEdit();
        uiControl.StartEditingRoom();
    }

    /// <summary>
    /// Exit game from editing mode
    /// </summary>
    public void EndEditing()
    {
        if(!budgetManager.canBuild(roomBP.cellCost * roomBP.CountNewCells()))
        {
            Debug.Log("NOT ENOUGH MONEY");
            return;
        }
        if(!roomBP.PassRequirements(out bool noCells))
        {
            if(noCells)
            {
                RemoveRoom();
            }
            return;
        }
        buildingDoor = false;
        isBuilding = false;
        budgetManager.NewExpanse(roomBP.name, roomBP.cellCost, roomBP.CountNewCells());
        room.ConfirmRoom(roomBP);
        roomBP = null;
        grid.ToggleGrid();
        room = null;
        uiControl.EndEditingRoom();
    }

    /// <summary>
    /// cancel last edit
    /// </summary>
    public void CancelEditing()
    {
        if(!roomBP.PassRequirements(out bool noCells))
        {
            RemoveRoom();
        }
        buildingDoor = false;
        isBuilding = false;
        room.CancelEditing(ref roomBP);
        uiControl.EndEditingRoom();
        grid.ToggleGrid();
        Debug.Log("CancelEditing");
    }

    /// <summary>
    /// enter building door mode
    /// </summary>
    private void StartBuildingDoor()
    {
        buildingDoor = true;
        roomBP.EnableCollision();
        uiControl.StartBuildingDoor();
    }

    /// <summary>
    /// exit building door mode
    /// </summary>
    private void EndBuildingDoor()
    {
        buildingDoor = false;
        roomBP.DisableCollision();
        uiControl.StopBuildingDoor();
    }

    /// <summary>
    /// get object that is now under mouse
    /// </summary>
    /// <returns></returns>
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
    /// remove whole room
    /// </summary>
    public void RemoveRoom()
    {
        grid.ToggleGrid();
        isBuilding = false;
        uiControl.EndEditingRoom();
        //here i need to change state of grid to free
        Destroy(room.gameObject);
    }

    /// <summary>
    /// change mode to building door or exit this mode
    /// </summary>
    public void ToggleBuildingDoor()
    {
        if(isBuilding)
        {
            if (buildingDoor)
            {
                EndBuildingDoor();
            }
            else
            {
                StartBuildingDoor();
            }
        }
    }
}
