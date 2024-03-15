using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExpensePart : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] TextMeshProUGUI amountText;
    [SerializeField] TextMeshProUGUI nameText;

    public void ShowData(string name, int amount, int cost)
    {
        gameObject.GetComponentsInChildren<TextMeshProUGUI>()[0].text = name;
        gameObject.GetComponentsInChildren<TextMeshProUGUI>()[1].text = amount.ToString();
        gameObject.GetComponentsInChildren<TextMeshProUGUI>()[2].text = cost.ToString() + "$";
    }
}
