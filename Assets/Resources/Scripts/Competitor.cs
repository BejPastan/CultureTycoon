
using UnityEngine;

[CreateAssetMenu(fileName = "Competitors", menuName = "Competitors", order = 1)]
public class Competitor : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] int avarageResult;
    [SerializeField] int fluctuations;
    public int result;

    public void GetValue()
    {
        result = avarageResult + UnityEngine.Random.Range(-fluctuations / 2, fluctuations / 2);
    }
}