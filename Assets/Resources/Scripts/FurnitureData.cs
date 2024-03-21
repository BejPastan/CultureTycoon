using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class FurnitureData : MonoBehaviour
{
    [SerializeField] string newName;
    public int buildingCost;
    [SerializeField] float quality;
    [SerializeField] public RoomType destinationType;
    [SerializeField] Transform[] usersLocations;
    private int[] occupaiedLocationsID;
    [SerializeField] int usagePrice;//actual not active
    [SerializeField] int maintenanceCost;//actual not active
    [SerializeField] public float usageTime;
    [SerializeField] int rotationNumber;
    [SerializeField] Grid grid;
    [SerializeField] Vector2Int roomCenter;
    [SerializeField] Vector2Int startPos;
    [SerializeField] AnimatorController animationForNPC;
    public Vector2Int GetStartPos() { return startPos+roomCenter; }
    [SerializeField] Vector2Int endPos;
    public Vector2Int GetEndPos() { return endPos+roomCenter; }
    Room roomToPlace = null;
    Material[] materials;
    [SerializeField] MeshRenderer meshRenderer;
    private bool building = true;

    public bool canBuild;

    private void Start()
    {
        occupaiedLocationsID = new int[0];
        grid = FindObjectOfType<Grid>();
    }

    /// <summary>
    /// start moving object on grid
    /// </summary>
    /// <param name="temporaryMaterial"></param>
    public void StartMoving(Material temporaryMaterial)
    {
        building = true;
        if(roomToPlace != null)
        {
            roomToPlace.RemoveFurniture(this);
        }
        materials = meshRenderer.materials;
        Debug.Log("temporaryMaterial: " + temporaryMaterial.name);
        meshRenderer.materials = new Material[1] { temporaryMaterial };
    }

    /// <summary>
    /// place objct on selected place on grid
    /// </summary>
    /// <returns></returns>
    public Room Place()
    {
        meshRenderer.materials = materials;
        roomToPlace.SetNewFurniture(this);
        building = false;
        return roomToPlace;
    }

    /// <summary>
    /// rotate object
    /// </summary>
    /// <param name="rotationNumber">number of rotation of 90 degres</param>
    public void Rotate(int rotationNumber)
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
        SetBounds(transform.position);
    }

    /// <summary>
    /// move object to given grid id
    /// </summary>
    /// <param name="gridId">new position to move</param>
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

    /// <summary>
    /// check if object can be placed on selected position
    /// </summary>
    /// <returns></returns>
    public async Task CheckConditions()
    {
        canBuild = false;
        await Task.Delay(25);//yeeee this work but I'm not happy with this solution. Why this is here? Well... I need to wait for until colider rotate too.
        //getting room inside which is now
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
        
        //chech if is in right room
        if(roomToPlace == null || roomToPlace.GetRoomType() != destinationType)
        {
            //get shader and change color to red
            meshRenderer.material.SetColor("_AccessColor", Color.red);
            canBuild = false;
            return;
        }
        else
        {
            //get colider and check if is not overlapping with any other object
            Collider[] overlapedColliders = Physics.OverlapBox(GetComponent<Collider>().bounds.center, GetComponent<Collider>().bounds.extents);
            foreach (Collider colider in overlapedColliders)
            {
                if ((colider.CompareTag("Furniture") && colider.transform != this.transform) || colider.CompareTag("Wall"))
                {
                    Debug.Log(colider.name);
                    meshRenderer.material.SetColor("_AccessColor", Color.red);
                    canBuild = false;
                    return;
                }
            }
            //here i must chack if colider collide with any other object
            meshRenderer.material.SetColor("_AccessColor", Color.green);
            canBuild = true;
            return;
        }      
    }

    /// <summary>
    /// set local position of bounds of object
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private async Task SetBounds(Vector3 position)
    {
        await Task.Delay(25);//yeeee this work but I'm not happy with this solution. Why this is here? Well... I need to wait for until colider rotate too.
        Collider collider = GetComponent<Collider>();
        Vector3 newCenter = position - collider.transform.position;

        newCenter += collider.bounds.center;
        startPos = grid.GetRealGridId(newCenter - collider.bounds.extents);
        endPos = grid.GetRealGridId(newCenter + collider.bounds.extents);
    }

    /// <summary>
    /// disable colider of object
    /// </summary>
    public void DisableCollider()
    {
        GetComponent<Collider>().enabled = false;
    }

    /// <summary>
    /// enable colider of object
    /// </summary>
    public void EnableCollider()
    {
        GetComponent<Collider>().enabled = true;
    }

    /// <summary>
    /// Initialize moving object
    /// </summary>
    private void OnMouseDown()
    {
        if(!building)
        {
            FindObjectOfType<FurniturePlacer>().StartPlacing(this);
        }
    }

    /// <summary>
    /// Check if this furniture have empty space for new NPC and it's not building now
    /// </summary>
    /// <returns></returns>
    public bool CanBeUsed(out Vector3 slotPos)
    {
        bool freeslot;
        slotPos = Vector3.zero;
        if(!building)
        {
            for(int i = 0; i<usersLocations.Length; i++)
            {
                freeslot = true;
                for(int j = 0; j<occupaiedLocationsID.Length; j++)
                {
                    if(i == occupaiedLocationsID[j])
                    {
                        freeslot = false;
                        break;
                    }
                }
                if(freeslot)
                {
                    slotPos = usersLocations[i].position;
                    return true;
                }

            }
        }
        return false;
    }

    public bool UseFurniture(out AnimatorController animator, out RoomType furnitureType,out float quality, out Vector3 usingSlot)
    {
        Debug.Log("Anything?");
        //check is there any empty space for NPC
        Debug.Log("ocuppied space: " + (occupaiedLocationsID.Length));
        if (occupaiedLocationsID.Length < usersLocations.Length)
        {
            Debug.Log("Furniture is not occupied");
            animator = animationForNPC;
            furnitureType = destinationType;
            quality = this.quality;

            //in future i need to change this to get empty place by id
            usingSlot = usersLocations[occupaiedLocationsID.Length].position;
            Array.Resize(ref occupaiedLocationsID, occupaiedLocationsID.Length + 1);

            //here i need to start using animation
            return true;

        }
        else
        {
            Debug.Log("Furniture is occupied");
            animator = null;
            furnitureType = RoomType.none;
            quality = 0;
            usingSlot = Vector3.zero;
            return false;
        }
    }

    public void LeaveFurniture(Vector3 slotPos)
    {
        //find id of slot
        int id = 0;
        for(int i = 0; i < usersLocations.Length; i++)
        {
            if (usersLocations[i].position == slotPos)
            {
                id = i;
                break;
            }
        }
        //remove id from list
        for(int i = id; i < occupaiedLocationsID.Length-1; i++)
        {
            occupaiedLocationsID[i] = occupaiedLocationsID[i + 1];
        }
    }
}