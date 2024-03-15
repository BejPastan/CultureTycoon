using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeText;

    public void UpdateTime(int day, string month, int year)
    {
        timeText.text = year+ " " + month + " " + day;
    }
}
