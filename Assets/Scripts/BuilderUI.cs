using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuilderUI : MonoBehaviour
{
    [SerializeField]
    Button buildingButton;
    [SerializeField]
    Button doorButton;

    [SerializeField]
    Transform buildingMenu;

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
        buildingButton.interactable = false;
        buildingMenu.gameObject.SetActive(true);
    }

    public void StopEditing()
    {
        buildingButton.interactable = true;
        buildingMenu.gameObject.SetActive(false);
    }

    public void StartBuildingDoor()
    {
        doorButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "End Setting Door";
    }

    public void StopBuildingDoor()
    {
        doorButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Start Setting Door";
    }
}
