using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentMenuUI : MonoBehaviour
{
    [SerializeField] List<GameObject> furnitures;
    [SerializeField] GameObject buttonPref;
    [SerializeField] RoomType roomType;    public RoomType GetRoomType(){ return roomType; }

    public void UnlockNewFurniture(GameObject newFurniture)
    {
        Debug.Log("UnlockNewFurniture");
        for(int i = 0; i<furnitures.Count; i++)
        {
            if (furnitures[i].GetComponent<FurnitureData>().name == newFurniture.GetComponent<FurnitureData>().name)
            {
                break;
            }
        }
        Debug.Log("FurnitureAdded");
        furnitures.Add(newFurniture);
    }

    private void OnEnable()
    {
        Debug.Log("Awake");
        BuildEquipmentMenu();
    }

    private void OnDisable()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void BuildEquipmentMenu()
    {
        FurniturePlacer furniturePlacer = FindObjectOfType<FurniturePlacer>();
        for(int i = 0; i<furnitures.Count; i++)
        {
            GameObject button = Instantiate(buttonPref, transform);
            Vector3 pos = new Vector3(0, i * 40, 0);
            button.GetComponent<RectTransform>().anchoredPosition = pos;
            //add onclick event
            Debug.Log("i = " + i);
            GameObject furnitureToPlace = furnitures[i];
            button.GetComponent<Button>().onClick.AddListener(delegate { furniturePlacer.StartPlacing(furnitureToPlace); });
            button.GetComponentInChildren<TextMeshProUGUI>().text = furnitures[i].name;
            //in future add more info from furniture data
        }
    }
}
