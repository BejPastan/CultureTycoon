using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuilderUI : MonoBehaviour
{
    [SerializeField]
    Button buildingButton;
    [SerializeField]
    Button doorButton;

    public void StartEditing()
    {
        //change text of button
        buildingButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Exit Building mode";
        doorButton.enabled = true;
    }

    public void StopEditing()
    {
        buildingButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Enter Building mode";
        doorButton.enabled = false;
    }

    public void StartBuildingDoor()
    {
        buildingButton.enabled = false;
        doorButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "End Setting Door";
    }

    public void StopBuildingDoor()
    {
        doorButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Start Setting Door";
        buildingButton.enabled = true;
    }
}
