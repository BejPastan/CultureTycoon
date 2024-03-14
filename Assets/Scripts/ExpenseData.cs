public struct ExpenseData
{
    public int cost;
    public int amount;
    public string name;

    public ExpenseData(int cost, int amount, string name)
    {
        this.cost = cost * amount;
        this.amount = amount;
        this.name = name;
    }
}
