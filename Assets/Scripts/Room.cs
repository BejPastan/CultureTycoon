using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    RoomCell[,] cells = new RoomCell[0,0];
    RoomBlueprint roomBlueprint;

    public void ConfirmRoom(RoomBlueprint actualState)
    {
        roomBlueprint = actualState;
        actualState.ConfirmBlueprint(out cells);
    }

    public void CancelEditing(ref RoomBlueprint actualState)
    {
        actualState.Cancel(ref roomBlueprint);
        roomBlueprint
    }
}
