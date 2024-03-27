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
    [SerializeField] public string newName;
    public int buildingCost;
    [SerializeField] int level;
    [SerializeField] public float quality;
    [SerializeField] public RoomType destinationType;
    [SerializeField] Transform[] usersLocations;
    private bool[] occupaiedLocationsID;
    [SerializeField] int usagePrice;//actual not active
    [SerializeField] public int maintenanceCost;//actual not active
    [SerializeField] public float usageTime;
    [SerializeField] AnimatorController animationForNPC;
    [SerializeField] Animator furnitureAnimator;

    [Header("BUILDIGNB")]
    int rotationNumber;
    [SerializeField] Grid grid;
    [SerializeField] Vector2Int roomCenter;
    [SerializeField] Vector2Int startPos;
    public Vector2Int GetStartPos() { return startPos+roomCenter; }
    [SerializeField] Vector2Int endPos;
    public Vector2Int GetEndPos() { return endPos+roomCenter; }
    Room roomToPlace = null;
    Material[] materials;
    [SerializeField] MeshRenderer meshRenderer;
    private bool building = true;
    public bool canBuild;

    private bool used = false;

    private void Start()
    {
        occupaiedLocationsID = new bool[usersLocations.Length];
        for(int i = 0; i < usersLocations.Length; i++)
        {
            occupaiedLocationsID[i] = false;
        }
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
        //create list of materials with materials length and set all materials to temporaryMaterial
        Material[] newMaterials = new Material[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            newMaterials[i] = temporaryMaterial;
        }
        meshRenderer.materials = newMaterials;
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
            foreach (Material material in meshRenderer.materials)
            {
                material.SetColor("_AccessColor", Color.red);
            }
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
                    foreach (Material material in meshRenderer.materials)
                    {
                        material.SetColor("_AccessColor", Color.red);
                    }
                    canBuild = false;
                    return;
                }
            }
            //here i must chack if colider collide with any other object
            foreach (Material material in meshRenderer.materials)
            {
                material.SetColor("_AccessColor", Color.green);
            }
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
        if(!building && !used)
        {
            FindObjectOfType<FurniturePlacer>().StartPlacing(this);
        }
    }

    /// <summary>
    /// Check if this furniture have empty space for new NPC and it's not building now
    /// </summary>
    /// <returns></returns>
    public bool CanBeUsed(out Vector3 slotPos, out int slotId)
    {
        slotPos = Vector3.zero;
        slotId = -1;
        if(!building)
        {
            for(int i = 0; i< usersLocations.Length; i++)
            {
                if (!occupaiedLocationsID[i])
                {
                    slotPos = usersLocations[i].position;
                    slotId = i;
                    occupaiedLocationsID[i] = true;
                    return true;
                }
            }
        }
        return false;
    }

    public void UseFurniture(out AnimatorController animator, out RoomType furnitureType,out float quality)
    {
        used = true;
        furnitureAnimator.SetBool("painting", true);
        animator = animationForNPC;
        furnitureType = destinationType;
        quality = this.quality;
    }

    public void LeaveFurniture(int slot)
    {
        used = false;
        furnitureAnimator.SetBool("painting", false);
        occupaiedLocationsID[slot] = false;
    }
}