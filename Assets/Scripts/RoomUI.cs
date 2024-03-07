using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    [SerializeField]
    Transform editButton;

    public void StartEditing()
    {
        HideUI();
    }

    public void EndEditing()
    {
        ShowUI();
    }

    public void SetEditButton(Builder builder, Room room)
    {
        EndEditing();
        Debug.Log(editButton.GetComponent<Button>());
        editButton.GetComponent<Button>().onClick.AddListener( delegate { builder.StartEditing(room); } );
        StartEditing();
    }

    private void HideUI()
    {
        editButton.gameObject.SetActive(false);
    }

    private void ShowUI()
    {
        editButton.gameObject.SetActive(true);
    }
}
