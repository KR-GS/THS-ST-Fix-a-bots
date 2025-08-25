/*
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FormulaAttempt
{
    public int coefficient;
    public int constant;
    public string formulaString;
    public float timeStamp;
    public bool wasCorrect;

    public FormulaAttempt(int coef, int consta)
    {
        coefficient = coef;
        constant = consta;
        formulaString = FormatFormulaString(coef, consta);
        timeStamp = Time.time;
        wasCorrect = false;
    }
    
    public FormulaAttempt(int coef, int consta, bool correct)
    {
        coefficient = coef;
        constant = consta;
        formulaString = FormatFormulaString(coef, consta);
        timeStamp = Time.time;
        wasCorrect = correct;
    }
    
    private string FormatFormulaString(int coef, int consta)
    {
        string sign = consta >= 0 ? "+" : "-";
        int absConst = Mathf.Abs(consta);
        return $"{coef}n {sign} {absConst}";
    }
    
    public override string ToString()
    {
        return formulaString;
    }
}

[System.Serializable] 
public class StageFormulaData
{
    public int stageNumber;
    public List<FormulaAttempt> attempts;
    public int correctCoefficient;
    public int correctConstant;
    public bool isCompleted;
    public float totalTime;
    
    public StageFormulaData(int stage)
    {
        stageNumber = stage;
        attempts = new List<FormulaAttempt>();
        isCompleted = false;
        totalTime = 0f;
    }
    
    public void AddAttempt(FormulaAttempt attempt)
    {
        attempts.Add(attempt);
    }
    
    public void SetCorrectAnswer(int coef, int consta)
    {
        correctCoefficient = coef;
        correctConstant = consta;
    }
    
    public void CompleteStage(float time)
    {
        isCompleted = true;
        totalTime = time;
        
        // Mark the last attempt as correct if stage is completed
        if (attempts.Count > 0)
        {
            attempts[attempts.Count - 1].wasCorrect = true;
        }
    }
    
    public int GetAttemptCount()
    {
        return attempts.Count;
    }
    
    public string GetAttemptsString()
    {
        if (attempts.Count == 0) return "";
        
        List<string> attemptStrings = new List<string>();
        foreach (FormulaAttempt attempt in attempts)
        {
            attemptStrings.Add(attempt.ToString());
        }
        
        return string.Join(", ", attemptStrings);
    }
}
*/