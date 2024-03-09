using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class FurnitureData : MonoBehaviour
{
    [SerializeField] string newName;
    [SerializeField] int buildingCost;
    [SerializeField] float quality;
    [SerializeField] RoomType destinationType;
    [SerializeField] Transform[] usersLocations;
    [SerializeField] int usagePrice;//actual not active
    [SerializeField] int maintenanceCost;//actual not active
    [SerializeField] int rotationNumber;
    [SerializeField] Grid grid;
    [SerializeField] Vector2Int roomCenter;
    [SerializeField] Vector2Int startPos;
    public Vector2Int GetStartPos() { return startPos+roomCenter; }
    [SerializeField] Vector2Int endPos;
    public Vector2Int GetEndPos() { return endPos+roomCenter; }
    Room roomToPlace = null;
    Material[] materials;
    [SerializeField] MeshRenderer meshRenderer;

    private void Start()
    {
        grid = FindObjectOfType<Grid>();
    }

    public void StartMoving(Material temporaryMaterial)
    {
        materials = meshRenderer.materials;
        Debug.Log("temporaryMaterial: " + temporaryMaterial.name);
        meshRenderer.materials = new Material[1] { temporaryMaterial };
    }

    public Room Place()
    {
        meshRenderer.materials = materials;
        roomToPlace.SetNewFurniture(this);
        return roomToPlace;
    }

    public async Task Rotate(int rotationNumber)
    {
        this.rotationNumber += rotationNumber;

        if (this.rotationNumber > 4)
        {
            this.rotationNumber -= 4;
        }
        else if (this.rotationNumber < 0)
        {
            this.rotationNumber += 4;
        }
        transform.Rotate(0, 90 * rotationNumber, 0);
        await Task.Delay(50);//yeeee this work but I'm not happy with this solution. Why this is here? Well... I need to wait for until colider rotate too.
        SetBounds(transform.position);
    }

    public void SetOnGrid(Vector2Int gridId)
    {
        Vector3 newPos = grid.GetWorldPosition(gridId);
        SetBounds(newPos);
        //check if start and end pos are inside the grid
        if (startPos.x < 0 || startPos.y < 0 || endPos.x >= grid.width || endPos.y >= grid.depth)
        {
            SetBounds(transform.position);
            return;
        }
        transform.position = newPos;
        //here i must check if there is a apropate type of room
    }

    public bool CheckConditions()
    {
        Vector2Int gridId = grid.GetGridId(transform.position);
        Room[] rooms = FindObjectsOfType<Room>();
        roomToPlace = null;
        foreach (Room room in rooms)
        {
            if (room.IsOnThisGrid(gridId))
            {
                roomToPlace = room;
            }
        }
        if(roomToPlace == null || roomToPlace.GetRoomType() != destinationType)
        {
            //get shader and change color to red
            meshRenderer.material.SetColor("_AccessColor", Color.red);
            return false;
        }
        else
        {
            //here i must chack if colider collide with any other object
            meshRenderer.material.SetColor("_AccessColor", Color.green);
            return true;
        }
    }

    private void SetBounds(Vector3 position)
    {
        Collider collider = GetComponent<Collider>();
        Vector3 newCenter = position - collider.transform.position;

        newCenter += collider.bounds.center;
        startPos = grid.GetRealGridId(newCenter - collider.bounds.extents);
        endPos = grid.GetRealGridId(newCenter + collider.bounds.extents);
    }
}