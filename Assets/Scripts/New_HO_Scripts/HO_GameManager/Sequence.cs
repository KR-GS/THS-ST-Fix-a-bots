using System.Collections.Generic;
using UnityEngine;

public class Sequence : MonoBehaviour
{
    public int MaxNumber { get; private set; }
    public int Coefficient { get; private set; }
    public int Constant { get; private set; }
    public string FormulaString => $"{Coefficient}n{(Constant >= 0 ? "+" : "")}{Constant}";
    public List<int> Numbers { get; private set; }

    public Sequence(int maxNumber = 25)
    {
        MaxNumber = maxNumber;
        GenerateRandomFormula();
        GenerateSequence();
    }

    public void GenerateRandomFormula()
    {
        Coefficient = Random.Range(2, 6);
        Constant = Random.Range(-Coefficient + 1, 6);
    }

    public void GenerateSequence()
    {
        Numbers = new List<int>();
        for (int n = 1; ; n++)
        {
            int val = Coefficient * n + Constant;
            if (val > MaxNumber) break;
            if (val >= 1)
                Numbers.Add(val);
        }
    }

    public bool Matches(Sequence other)
    {
        return other != null &&
               this.Coefficient == other.Coefficient &&
               this.Constant == other.Constant;
    }

}