using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BudgetUI : MonoBehaviour
{
    [SerializeField] Transform endYearUI;
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] GameObject rankingPartPref;
    [SerializeField] GameObject exensesPref;


    [SerializeField] Transform onTopParent;
    [SerializeField] Transform notOnTop;

    Transform[] rankingParts = new Transform[0];
    Transform[] expensesParts = new Transform[0];

    public void UpdateMoney(int money)
    {
        moneyText.text ="Money: "+money.ToString();
    }

    public void ShowEndYearUI(ref Competitor[] competitors, int assignedMoney, ExpenseData[] expanses)
    {
        endYearUI.gameObject.SetActive(true);
        BuildRanking(competitors);
        BuildExpensesSheet(expanses);
    }

    public void HideEndYearUI()
    {
        DestroyArray(ref rankingParts);
        DestroyArray(ref expensesParts);
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

        Transform competitorsUI = GameObject.Find("Competitors").transform;


        //change size of the competitorsUI to fit all the elements
        RectTransform rect = competitorsUI.GetComponent<RectTransform>();
        if (competitors.Length * 60 > 500)
        {
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, competitors.Length * 60 + 60);
        }
        else
        {
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, 500);
        }

        for (int i = 0; i < competitors.Length; i++)
        {
            Vector3 position = new Vector2(0, (i+1)*-60);
            GameObject rankingPart = Instantiate(rankingPartPref, competitorsUI);
            rankingPart.GetComponent<RectTransform>().anchoredPosition = position;
            rankingPart.GetComponent<RankingPart>().SetData(competitors[i].name, competitors[i].result);
            Array.Resize(ref rankingParts, rankingParts.Length + 1);
            rankingParts[i] = rankingPart.transform;
        }
    }

    private void BuildExpensesSheet(ExpenseData[] expenses)
    {
        Transform competitorsUI = GameObject.Find("Expenses").transform;

        //change size of the competitorsUI to fit all the elements
        RectTransform rect = competitorsUI.GetComponent<RectTransform>();
        if (expenses.Length * 60 > 500)
        {
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, expenses.Length * 60 + 60);
        }
        else
        {
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, 500);
        }

        for (int i = 0; i < expenses.Length; i++)
        {
            Vector2 position = new Vector2(0, (i+1) * -60);
            GameObject expansePart = Instantiate(exensesPref, competitorsUI);
            expansePart.GetComponent<RectTransform>().anchoredPosition = position;
            expansePart.GetComponent<ExpensePart>().ShowData(expenses[i].name, expenses[i].amount, expenses[i].cost);
            Array.Resize(ref expensesParts, expensesParts.Length + 1);
            expensesParts[i] = expansePart.transform;
        }
    }

    private void DestroyArray(ref Transform[] arrayToDestroy)
    {
        for (int i = 0; i < arrayToDestroy.Length; i++)
        {
            Destroy(arrayToDestroy[i].gameObject);
        }
        arrayToDestroy = new Transform[0];
    }

    public void BringToFront(Transform transformToTop)
    {
        //remove all children from parent
        foreach (Transform child in onTopParent)
        {
            child.SetParent(notOnTop);
        }
        transformToTop.SetParent(onTopParent);
        transformToTop.localPosition = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        if(Input.mouseScrollDelta.y != 0)
        {
            Scroll();
        }
    }

    private void Scroll()
    {
        RectTransform child = onTopParent.GetChild(0).GetComponent<RectTransform>();
        //nie myślę już, ogarnę to jutro
        if(Input.mouseScrollDelta.y > 0)//scroll up
        {
            if(child.anchoredPosition.y < 0)
            {
                child.transform.localPosition += new Vector3(0, 10, 0);
                Debug.Log("scroll up " + child.anchoredPosition);
            }
        }
        else if (Input.mouseScrollDelta.y < 0)//scroll down
        {
            if(child.anchoredPosition.y + child.sizeDelta.y - onTopParent.GetComponent<RectTransform>().sizeDelta.y > 0)
            {
                child.transform.localPosition += new Vector3(0, -10, 0);
                Debug.Log("scroll down " + child.anchoredPosition);
            }
        }
    }
}
