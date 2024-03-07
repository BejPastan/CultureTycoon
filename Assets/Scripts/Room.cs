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
        actualState.ConfirmBlueprint(out cells, out Vector3 roomCenter);
        uiController.transform.position = new Vector3(roomCenter.x, uiController.transform.position.y, roomCenter.z);        
        uiController.EndEditing();
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
    }

    public RoomBlueprint StartEdit()
    {
        uiController.StartEditing();
        roomBlueprint.DisableCollision();
        return roomBlueprint;
    }
}
