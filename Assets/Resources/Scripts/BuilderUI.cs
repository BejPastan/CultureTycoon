using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuilderUI : MonoBehaviour
{
    [SerializeField]
    Transform bottomMenu;
    [SerializeField]
    Button doorButton;

    [SerializeField]
    Transform equipmentMenu;

    [SerializeField]
    Transform newRoomSelectMenu;
    [SerializeField]
    Transform newRoomBuildingInfo;

    public void DisableUI()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
    }

    public void EnableUI()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = true;
        }
    }

    /// <summary>
    /// show apripriate menu for funrnitures or rooms
    /// </summary>
    /// <param name="menu"></param>
    public void ToggleBuildMenu(Transform menu)
    {
        if(menu.gameObject.activeSelf)
        {
            menu.gameObject.SetActive(false);
        }
        else
        {
            menu.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Change UI to room editing mode
    /// </summary>
    public void StartEditingRoom()
    {
        //change text of button
        bottomMenu.gameObject.SetActive(false);
        //I must disconect building menu from building button
        newRoomSelectMenu.gameObject.SetActive(true);
        newRoomBuildingInfo.gameObject.SetActive(true);
    }

    /// <summary>
    /// Change UI to normal mode
    /// </summary>
    public void EndEditingRoom()
    {
        bottomMenu.gameObject.SetActive(true);
        newRoomSelectMenu.gameObject.SetActive(false);
        newRoomBuildingInfo.gameObject.SetActive(false);
        StopBuildingDoor();
    }

    /// <summary>
    /// Change UI to setting door mode
    /// </summary>
    public void StartBuildingDoor()
    {
        doorButton.transform.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
    }

    /// <summary>
    /// Return building door UI to normal mode
    /// </summary>
    public void StopBuildingDoor()
    {
        doorButton.transform.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
    }
}