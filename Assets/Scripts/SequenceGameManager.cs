using System.Collections.Generic;
using UnityEngine;

public class SequenceGameManager : MonoBehaviour
{
    public static SequenceGameManager Instance;

    public UIManager uiManager;

    public RuleInputFormula ruleInputFormula;

    public int totalButtons = 60;
    public Sequence currentSequence;

    private List<int> correctAnswers = new List<int>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        uiManager.ShowRuleInputPanel(false);
        StartNewLevel();
        uiManager.PlaySequenceIntro(currentSequence.Terms);
    }

    public void StartNewLevel()
    {
        int start = Random.Range(1, 2);
        int diff = Random.Range(2, 3);

        while (start == diff){
            diff = Random.Range(2, 3); //makes sure it's not just 2n for example
        }

        currentSequence = new Sequence(start, diff);

        uiManager.GenerateTimeButtons(totalButtons);
        uiManager.HighlightInitialSequenceNumbers(currentSequence.Terms.GetRange(0, 3));

        correctAnswers = currentSequence.Terms.GetRange(3, 3);
    }

    public void ConfirmAnswer()
    {
        List<int> selected = uiManager.GetSelectedAnswers();

        if (selected.Count != 3)
        {
            uiManager.SetFeedback("Select exactly 3 time periods!", false);
            return;
        }

        selected.Sort();
        correctAnswers.Sort();

        bool correct = true;
        for (int i = 0; i < 3; i++)
        {
            if (selected[i] != correctAnswers[i])
            {
                correct = false;
                break;
            }
        }

        if (correct)
        {
            uiManager.SetFeedback("Correct sequence! Now enter the rule.", true);
            uiManager.showRuleInputButton.gameObject.SetActive(true);
            uiManager.ShowRuleInputPanel(true);
        }
        else
        {
            uiManager.SetFeedback("Incorrect. Try again.", false);
        }
    }

    public void ValidateFormula()
{
    var (userStart, userDiff) = uiManager.GetRuleInputs();
    string expectedFormula;
    if(currentSequence.Start-currentSequence.Difference > 0){
        expectedFormula = $"{currentSequence.Difference}n+{currentSequence.Start - currentSequence.Difference}";
    }
    else{
        expectedFormula = $"{currentSequence.Difference}n-{-(currentSequence.Start - currentSequence.Difference)}";
    }

    Debug.Log($"{expectedFormula}");

    string inputFormula = ruleInputFormula.SubmitRule();

    bool isCorrectStart = userStart == currentSequence.Start;
    bool isCorrectDiff = userDiff == currentSequence.Difference;

        if (isCorrectStart && isCorrectDiff){
            if (inputFormula == expectedFormula){
                uiManager.SetRuleFeedback("Perfect! You found the correct rule!", true);
            }
            else{
                uiManager.SetRuleFeedback("Your start and difference are correct but your formula is wrong!", false);
            }
    }
        else{
            uiManager.SetRuleFeedback("Oh no! Please double check your start and difference!", false);
        }
}

    /*public void ValidateRuleInput()
    {
        var (userStart, userDiff, formula) = uiManager.GetRuleInputs();

        bool isCorrectStart = userStart == currentSequence.Start;
        bool isCorrectDiff = userDiff == currentSequence.Difference;
        bool iscorrectFormula;


        
        if((currentSequence.Start - currentSequence.Difference) < 0)
        {
            expectedFormula = $"{currentSequence.Difference}n-{-(currentSequence.Start - currentSequence.Difference)}";
        }
        else{
            expectedFormula = $"{currentSequence.Difference}n+{currentSequence.Start - currentSequence.Difference}";
        }

        
        bool isCorrectFormula = formula.Replace(" ", "") == expectedFormula.Replace(" ", "");

        if (isCorrectStart && isCorrectDiff && isCorrectFormula)
        {
            uiManager.SetRuleFeedback("Perfect! You found the correct rule!", true);
        }
        else
        {
            uiManager.SetRuleFeedback("Something is off. Double-check your rule.", false);
        }
    }*/
}