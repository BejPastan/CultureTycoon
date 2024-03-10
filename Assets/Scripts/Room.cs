using System;
using UnityEngine;

public class Room : MonoBehaviour
{
    RoomCell[,] cells;
    RoomBlueprint roomBlueprint;
    [SerializeField] Transform elementsParent;
    [SerializeField] RoomUI uiController;
    [SerializeField] FurnitureData[] furnitureData = new FurnitureData[0];

    public void OnCreate(RoomBuilder builder)
    {
        uiController.SetEditButton(builder, this);
    }

    public void ConfirmRoom(RoomBlueprint actualState)
    {
        roomBlueprint = actualState;
        actualState.ConfirmBlueprint(out cells, out Vector3 roomCenter);
        uiController.transform.position = new Vector3(roomCenter.x, uiController.transform.position.y, roomCenter.z);
        uiController.EndEditing();
        EnableFurnitureCollider();
    }

    public void CancelEditing(ref RoomBlueprint actualState)
    {
        if(roomBlueprint == null)
        {
            actualState.RemoveAll();
            return;
        }
        actualState.Cancel();
        roomBlueprint = actualState;
        EnableFurnitureCollider();
    }

    public RoomBlueprint StartEdit()
    {
        uiController.StartEditing();
        roomBlueprint.DisableCollision();
        DisableFurnitureCollider();
        return roomBlueprint;
    }

    public RoomType GetRoomType()
    {
        return roomBlueprint.RoomType;
    }

    public bool IsOnThisGrid(Vector2Int gridId)
    {
        if (roomBlueprint.parts[0].GetCellByGridId(gridId) != null)
        { 
            return true; 
        }
        return false;
    }

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

    public void SetNewFurniture(FurnitureData newFurniture)
    {
        Array.Resize(ref furnitureData, furnitureData.Length + 1);
        furnitureData[furnitureData.Length - 1] = newFurniture;
    }

    public void RemoveFurniture(ref FurnitureData furniture)
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

    private void DisableFurnitureCollider()
    {
        foreach(FurnitureData furniture in furnitureData)
        {
            furniture.DisableCollider();
        }
    }

    private void EnableFurnitureCollider()
    {
        foreach(FurnitureData furniture in furnitureData)
        {
            furniture.EnableCollider();
        }
    }
}
