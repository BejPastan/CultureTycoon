using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    [SerializeField]
    Transform editButton;


    private void Start()
    {
        CameraControler.onCameraRotation += RotateToCamera;
    }

    private void OnDestroy()
    {
        CameraControler.onCameraRotation -= RotateToCamera;
    }

    private void RotateToCamera(Vector3 rotation)
    {
        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);
    }

    /// <summary>
    /// Change room UI to editing mode
    /// </summary>
    public void StartEditing()
    {
        HideUI();
    }

    /// <summary>
    /// Change room UI to normal mode
    /// </summary>
    public void EndEditing()
    {
        ShowUI();
    }

    /// <summary>
    /// Setting values for edit button and methods to call
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="room"></param>
    public void SetEditButton(RoomBuilder builder, Room room)
    {
        EndEditing();
        //Debug.Log(editButton.GetComponent<Button>());
        editButton.GetComponent<Button>().onClick.AddListener( delegate { builder.StartEditing(room); } );
        StartEditing();
    }

    /// <summary>
    /// hide room UI
    /// </summary>
    public void HideUI()
    {
        editButton.gameObject.SetActive(false);
    }


    /// <summary>
    /// Show room UI
    /// </summary>
    public void ShowUI()
    {
        editButton.gameObject.SetActive(true);
    }
}
