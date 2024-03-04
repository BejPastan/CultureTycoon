using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    RoomCell[,] cells = new RoomCell[0,0];
    RoomBlueprint roomBlueprint;
    [SerializeField]
    Transform elementsParent;
    [SerializeField]
    RoomUI uiController;

    public void OnCreate(Builder builder)
    {
        uiController.SetEditButton(builder, this);
    }

    public void ConfirmRoom(RoomBlueprint actualState)
    {
        roomBlueprint = actualState;
        actualState.ConfirmBlueprint(out cells);
        //change parent of all cells elements to elementsParent
        foreach (RoomCell cell in cells)
        {
            cell.GetFloor().transform.SetParent(elementsParent);
            for(int x = 0; x < 3; x++)
            {
                for(int y = 0; y < 3; y++)
                {
                    try
                    {
                        cell.GetWallByLocalPos(new Vector2Int(x, y)).transform.SetParent(elementsParent);
                    }
                    catch { }
                }
            }
        }

        uiController.ShowUI();
    }

    public void CancelEditing(ref RoomBlueprint actualState)
    {
        actualState.Cancel(ref roomBlueprint);
        roomBlueprint = actualState;
    }
}
