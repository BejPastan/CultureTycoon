using System;
using UnityEngine;

public class Room : MonoBehaviour
{
    RoomCell[,] cells;
    [SerializeField] RoomBlueprint roomBlueprint;
    [SerializeField] RoomBlueprint lastState = null;
    [SerializeField] Transform elementsParent;
    [SerializeField] RoomUI uiController;
    [SerializeField] FurnitureData[] furnitureData = new FurnitureData[0];

    private void OnDestroy()
    {
        roomBlueprint.RemoveAll();
    }

    /// <summary>
    /// Initialization of room
    /// </summary>
    /// <param name="builder"></param>
    public void OnCreate(RoomBuilder builder, RoomBlueprint blueprint)
    {
        roomBlueprint = blueprint;//this is ebcause I need it to erasing parts of room
        uiController.SetEditButton(builder, this);
    }

    /// <summary>
    /// Confirm editing of room
    /// </summary>
    /// <param name="actualState"></param>
    public void ConfirmRoom(RoomBlueprint actualState)
    {
        roomBlueprint = actualState;
        roomBlueprint.ConfirmBlueprint(out cells, out Vector3 roomCenter, out int newCells);
        uiController.transform.position = new Vector3(roomCenter.x, uiController.transform.position.y, roomCenter.z);
        uiController.EndEditing();
        EnableFurnitureCollider();





        //make new Instance of actualState
        lastState = roomBlueprint.Clone();
    }

    /// <summary>
    /// cancel editing of room
    /// </summary>
    /// <param name="actualState"></param>
    public void CancelEditing(ref RoomBlueprint actualState)
    {







        roomBlueprint.RemoveAll();
        if (lastState == null)
        {
            return;
        }

        roomBlueprint = lastState.Clone();
        roomBlueprint.Rebuild();








        EnableFurnitureCollider();
        uiController.EndEditing();
    }

    /// <summary>
    /// start ediiing room
    /// </summary>
    /// <returns></returns>
    public RoomBlueprint StartEdit()
    {

        uiController.StartEditing();
        roomBlueprint.DisableCollision();
        DisableFurnitureCollider();
        return roomBlueprint;
    }

    /// <summary>
    /// return type of this room
    /// </summary>
    /// <returns></returns>
    public RoomType GetRoomType()
    {
        return roomBlueprint.RoomType;
    }

    /// <summary>
    /// return if this room have part on selected grid
    /// </summary>
    /// <param name="gridId"></param>
    /// <returns></returns>
    public bool IsOnThisGrid(Vector2Int gridId)
    {
        if (roomBlueprint.parts[0].GetCellByGridId(gridId) != null)
        { 
            return true; 
        }
        return false;
    }

    /// <summary>
    /// remove selected area of room
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public void EraseArea(Vector2Int start, Vector2Int end)
    {
        //set smaller value to start and bigger to end
        if(start.x > end.x)
        {
            int temp = start.x;
            start.x = end.x;
            end.x = temp;
        }
        if(start.y > end.y)
        {
            int temp = start.y;
            start.y = end.y;
            end.y = temp;
        }
        
        Vector2Int furniturePos1, furniturePos2;
        foreach(FurnitureData furniture in furnitureData)
        {
            Debug.Log("furniture: " + furniture.name);
            furniturePos1 = furniture.GetStartPos();
            furniturePos2 = furniture.GetEndPos();

            if(furniturePos1.x > end.x)
            {
                Debug.Log("furniturePos1.x > end.x");
            }
            else if(furniturePos1.y > end.y)
            {
                Debug.Log("furniturePos1.y > end.y");
            }
            else if(furniturePos2.x < start.x)
            {
                Debug.Log("furniturePos2.x < start.x");
            }
            else if(furniturePos2.y < start.y)
            {
                Debug.Log("furniturePos2.y < start.y");
            }
            else
            {
                Debug.Log("cannot erase"); return;
            }
        }
        roomBlueprint.EraseArea(start, end);
    }

    /// <summary>
    /// set new furniture to room
    /// </summary>
    /// <param name="newFurniture"></param>
    public void SetNewFurniture(FurnitureData newFurniture)
    {
        Array.Resize(ref furnitureData, furnitureData.Length + 1);
        furnitureData[furnitureData.Length - 1] = newFurniture;
    }

    /// <summary>
    /// remove furniture from room
    /// </summary>
    /// <param name="furniture"></param>
    public void RemoveFurniture(FurnitureData furniture)
    {
        bool find = false;
        for(int i = 0; i < furnitureData.Length; i++)
        {
            if (furnitureData[i] == furniture || find)
            {
                find = true;
                try
                {
                    furnitureData[i] = furnitureData[i + 1];
                }catch(IndexOutOfRangeException)
                {
                    Array.Resize(ref furnitureData, furnitureData.Length - 1);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// disable collider of all furniture in room
    /// </summary>
    private void DisableFurnitureCollider()
    {
        foreach(FurnitureData furniture in furnitureData)
        {
            furniture.DisableCollider();
        }
    }

    /// <summary>
    /// enable collider of all furniture in room
    /// </summary>
    private void EnableFurnitureCollider()
    {
        foreach(FurnitureData furniture in furnitureData)
        {
            furniture.EnableCollider();
        }
    }
}
