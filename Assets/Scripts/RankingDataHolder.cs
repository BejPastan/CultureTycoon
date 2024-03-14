using System;
using UnityEngine;

public class RankingDataHolder : MonoBehaviour
{
    ExpenseData[] expanses = new ExpenseData[0];
    [SerializeField] Competitor[] competitors;


    public void SetNewExpense(string name, int cost, int amount)
    {
        Array.Resize(ref expanses, expanses.Length + 1);
        expanses[expanses.Length - 1] = new ExpenseData(cost, amount, name);
    }

    public void Reset()
    {
        expanses = new ExpenseData[0];
    }

    public ExpenseData[] GetExpenses()
    {
        return expanses;
    }

    public Competitor[] GetCompetitors()
    {
        foreach (Competitor competitor in competitors)
        {
            competitor.GetValue();
        }
        return competitors;
    }
}