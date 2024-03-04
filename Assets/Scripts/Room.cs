using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    RoomCell[,] cells = new RoomCell[0,0];

    public void ConfirmRoom(RoomBlueprint actualState)
    {
        actualState.ConfirmBlueprint(out cells);
    }
}
