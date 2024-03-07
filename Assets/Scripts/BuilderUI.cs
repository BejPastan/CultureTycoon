using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuilderUI : MonoBehaviour
{
    [SerializeField]
    Transform buildingButton;
    [SerializeField]
    Button doorButton;


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


    public void StartEditing()
    {
        //change text of button
        buildingButton.gameObject.SetActive(false);
        //I must disconect building menu from building button
        newRoomSelectMenu.gameObject.SetActive(true);
        newRoomBuildingInfo.gameObject.SetActive(true);
    }

    public void EndEditing()
    {
        buildingButton.gameObject.SetActive(true);
        newRoomSelectMenu.gameObject.SetActive(false);
        newRoomBuildingInfo.gameObject.SetActive(false);
    }

    public void StartBuildingDoor()
    {
        doorButton.transform.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
    }

    public void StopBuildingDoor()
    {
        doorButton.transform.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
    }
}
