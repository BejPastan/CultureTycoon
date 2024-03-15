using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private int day;
    [SerializeField] private month month;
    [SerializeField] private int year;
    [SerializeField] TimerUI timerUI;
    [SerializeField] BudgetManager budgetManager;

    private void Start()
    {
        timerUI.UpdateTime(day, month.ToString(), year);
        StartCoroutine(TimePass());
    }

    IEnumerator TimePass()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.5f);
            day++;
            if (day > 30)
            {
                day = 1;
                month++;
            }
            if((int)month > 11)
            {
                month = 0;
                year++;
                EndYear();
            }
            timerUI.UpdateTime(day, month.ToString(), year);
        }
    }

    private void EndYear()
    {
        //change timespeed to 0
        budgetManager.YearEnd();
        Time.timeScale = 0;
    }

    public void StartNewYear()
    {
        Time.timeScale = 1;
    }
}

public enum month
{
    January,
    February,
    March,
    April,
    May,
    June,
    July,
    August,
    September,
    October,
    November,
    December
}