using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankingPart : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI resultText;

    public void SetData(string name, int result)
    {
        nameText.text = name;
        resultText.text = result.ToString();
    }
}
