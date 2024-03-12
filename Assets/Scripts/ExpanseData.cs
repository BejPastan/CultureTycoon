using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpanseData : MonoBehaviour
{
    int cost;
    int amount;
    string name;

    public ExpanseData(int cost, int amount, string name)
    {
        this.cost = cost;
        this.amount = amount;
        this.name = name;
    }
}
