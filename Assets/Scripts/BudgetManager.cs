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

    private void Start()
    {
        budgetUI.UpdateMoney(normalMoney);
    }

    public void NewExpanse(string name, int cost, int amount)
    {
        Debug.Log("New expanse: " + name + " " + cost + " " + amount);
        normalMoney -= cost * amount;
        budgetUI.UpdateMoney(normalMoney);
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
        budgetUI.ShowEndYearUI(ref competitors);
    }

    public void StartNewYear()
    {
        budgetUI.HideEndYearUI();
    }
}
