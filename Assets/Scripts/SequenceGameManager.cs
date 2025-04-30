using System.Collections.Generic;
using UnityEngine;

public class SequenceGameManager : MonoBehaviour
{
    public static SequenceGameManager Instance;

    public UIManager uiManager;

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
    }

    public void StartNewLevel()
    {
        // Generate a new sequence (e.g., start = 5, diff = 4 → 5, 9, 13, 17, 21, 25)
        int start = Random.Range(1, 10);
        int diff = Random.Range(1, 6);
        currentSequence = new Sequence(start, diff);

        // Update UI
        uiManager.GenerateTimeButtons(totalButtons);

        // Highlight first 3 sequence terms only (e.g., 5, 9, 13)
        uiManager.HighlightInitialSequenceNumbers(currentSequence.Terms.GetRange(0, 3));

        // Store the expected next 3 numbers (e.g., 17, 21, 25)
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
            uiManager.ShowRuleInputPanel(true);
        }
        else
        {
            uiManager.SetFeedback("Incorrect. Try again.", false);
        }
    }

    public void ValidateRuleInput()
    {
        var (userStart, userDiff, formula) = uiManager.GetRuleInputs();

        bool isCorrectStart = userStart == currentSequence.Start;
        bool isCorrectDiff = userDiff == currentSequence.Difference;
        string expectedFormula;
        
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
    }
}