using System;
using UnityEngine;

public class RankingDataHolder : MonoBehaviour
{
    ExpanseData[] expanses = new ExpanseData[0];
    [SerializeField] Competitor[] competitors;


    public void SetNewExpense(string name, int cost, int amount)
    {
        Array.Resize(ref expanses, expanses.Length + 1);
        expanses[expanses.Length - 1] = new ExpanseData(cost, amount, name);
    }

    public void Reset()
    {
        expanses = new ExpanseData[0];
    }

    public ExpanseData[] GetExpenses()
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