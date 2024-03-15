using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BudgetManager : MonoBehaviour
{
    [SerializeField]
    private int normalMoney = 100;
    [SerializeField]
    BudgetUI budgetUI;
    [SerializeField]
    RankingDataHolder rankingDataHolder;
    [SerializeField] int playerResult;
    [SerializeField] Competitor player;

    [SerializeField] int moneyToDivide;

    private void Start()
    {
        budgetUI.UpdateMoney(normalMoney);
    }

    public void NewExpanse(string name, int cost, int amount)
    {
        Debug.Log("New expanse: " + name + " " + cost + " " + amount);
        normalMoney -= cost * amount;
        budgetUI.UpdateMoney(normalMoney);
        rankingDataHolder.SetNewExpense(name, cost, amount);
    }

    public bool canBuild(int cost)
    {
        if(cost<= normalMoney)
        {
            return true;
        }
        return false;
    }

    //make this method available for editor
    [ContextMenu("YearEnd")]
    public void YearEnd()
    {
        player.result = playerResult;
        Competitor[] competitors = rankingDataHolder.GetCompetitors();
        Array.Resize(ref competitors, competitors.Length + 1);
        competitors[competitors.Length - 1] = player;
        AssignMoney(ref competitors);
        budgetUI.ShowEndYearUI(ref competitors, normalMoney, rankingDataHolder.GetExpenses());
    }


    public void AssignMoney(ref Competitor[] competitors)
    {
        int resultSum = 0;
        for(int i = 0; i < competitors.Length; i++)
        {
            resultSum += competitors[i].result;
        }

        moneyToDivide /= resultSum;
        normalMoney = moneyToDivide*player.result;
    }

    public void StartNewYear()
    {
        budgetUI.HideEndYearUI();
        moneyToDivide *= Mathf.FloorToInt(moneyToDivide*1.1f);
    }
}
