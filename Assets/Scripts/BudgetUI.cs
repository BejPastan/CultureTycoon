using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BudgetUI : MonoBehaviour
{
    [SerializeField] Transform endYearUI;
    [SerializeField] GameObject rankingPartPref;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] GameObject exensesPref;

    Transform[] rankingParts = new Transform[0];
    Transform[] expensesParts = new Transform[0];

    public void UpdateMoney(int money)
    {
        moneyText.text ="Money: "+money.ToString();
    }

    public void ShowEndYearUI(ref Competitor[] competitors)
    {
        endYearUI.gameObject.SetActive(true);
        BuildRanking(competitors);
    }

    public void HideEndYearUI()
    {
        DestroyRanking();
        endYearUI.gameObject.SetActive(false);
    }

    private void BuildRanking(Competitor[] competitors)
    {
        //sort competitors by value
        for (int i = 0; i < competitors.Length; i++)
        {
            for (int j = i + 1; j < competitors.Length; j++)
            {
                if (competitors[i].result > competitors[j].result)
                {
                    Competitor temp = competitors[i];
                    competitors[i] = competitors[j];
                    competitors[j] = temp;
                }
            }
        }

        for(int i = 0; i < competitors.Length; i++)
        {
            Vector3 position = new Vector3(0, i*60, 0);
            GameObject rankingPart = Instantiate(rankingPartPref, endYearUI);
            rankingPart.transform.localPosition = position;
            rankingPart.GetComponent<RankingPart>().SetData(competitors[i].name, competitors[i].result);
            Array.Resize(ref rankingParts, rankingParts.Length + 1);
            rankingParts[i] = rankingPart.transform;
        }
    }

    private void BuildExpensesSheet(ExpanseData[] expenses)
    {

    }

    private void DestroyExpenses()
    {

    }

    private void DestroyRanking()
    {
        foreach(Transform rankingPart in rankingParts)
        {
            Destroy(rankingPart.gameObject);
        }
        rankingParts = new Transform[0];
    }
}
