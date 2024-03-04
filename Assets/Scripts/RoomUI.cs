using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    [SerializeField]
    Transform editButton;

    public void HideUI()
    {
        editButton.gameObject.SetActive(false);
    }

    public void ShowUI()
    {
        editButton.gameObject.SetActive(true);
    }

    public void SetEditButton(Builder builder, Room room)
    {
        Debug.Log("Setting edit button");
        ShowUI();
        Debug.Log(editButton.GetComponent<Button>());
        editButton.GetComponent<Button>().onClick.AddListener( delegate { builder.StartEditing(room); } );
        HideUI();
    }
}
