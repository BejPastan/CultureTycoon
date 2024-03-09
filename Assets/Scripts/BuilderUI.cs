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

    public void StartEditingRoom()
    {
        //change text of button
        bottomMenu.gameObject.SetActive(false);
        //I must disconect building menu from building button
        newRoomSelectMenu.gameObject.SetActive(true);
        newRoomBuildingInfo.gameObject.SetActive(true);
    }

    public void EndEditingRoom()
    {
        bottomMenu.gameObject.SetActive(true);
        newRoomSelectMenu.gameObject.SetActive(false);
        newRoomBuildingInfo.gameObject.SetActive(false);
        StopBuildingDoor();
    }

    public void StartBuildingDoor()
    {
        doorButton.transform.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
    }

    public void StopBuildingDoor()
    {
        doorButton.transform.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
    }

    public void StartPlacingEquipment()
    {
        
    }

    public void StopPlacingEquipment()
    {
        
    }
}
