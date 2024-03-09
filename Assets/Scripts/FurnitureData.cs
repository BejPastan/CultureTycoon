using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FurnitureData : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] int buildingCost;
    [SerializeField] float quality;
    [SerializeField] RoomType destinationType;
    [SerializeField] Transform[] usersLocations;
    [SerializeField] int usagePrice;//actual not active
    [SerializeField] int maintenanceCost;//actual not active
    [SerializeField] int rotationNumber;
    [SerializeField] Grid grid;
    [SerializeField] Vector2Int startPos;
    [SerializeField] Vector2Int endPos;

    private void Start()
    {
        grid = FindObjectOfType<Grid>();
    }

    public void Rotate(int rotationNumber)
    {
        this.rotationNumber += rotationNumber;
        transform.Rotate(0, 90 * rotationNumber, 0);

        if(this.rotationNumber == 3)
        {
            this.rotationNumber = 4;
        }
        else if (this.rotationNumber < 0)
        {
            this.rotationNumber += 3;
        }

        SetBounds(transform.position);
    }

    public void SetOnGrid(Vector2Int gridId)
    {
        Vector3 newPos = grid.GetWorldPosition(gridId);

        SetBounds(newPos);
        //check if start and end pos are inside the grid
        if (startPos.x < 0 || startPos.y < 0 || endPos.x >= grid.width || endPos.y >= grid.depth)
        {
            Debug.Log("Out of bounds");
            SetBounds(transform.position);
            return;
        }
        transform.position = newPos;
    }

    private void SetBounds(Vector3 position)
    {
        Collider collider = GetComponent<Collider>();
        startPos = grid.GetRealGridId(position - collider.bounds.extents);
        endPos = grid.GetRealGridId(position + collider.bounds.extents);
    }
}