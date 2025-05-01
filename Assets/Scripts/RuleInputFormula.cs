using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RuleInputFormula : MonoBehaviour
{
    public TextMeshProUGUI coefficientText;
    public TextMeshProUGUI signText;
    public TextMeshProUGUI constantText;
    public Button upCoefficientButton, downCoefficientButton;
    public Button toggleSignButton;
    public Button upConstantButton, downConstantButton;

    private int coefficient = 1;
    private int constant = 0;
    private bool isAddition = true;

    private void Start()
    {
        UpdateDisplay();

        upCoefficientButton.onClick.AddListener(() => { coefficient++; UpdateDisplay(); });
        downCoefficientButton.onClick.AddListener(() => { coefficient = Mathf.Max(1, coefficient - 1); UpdateDisplay(); });

        toggleSignButton.onClick.AddListener(() => { isAddition = !isAddition; UpdateDisplay(); });

        upConstantButton.onClick.AddListener(() => { constant++; UpdateDisplay(); });
        downConstantButton.onClick.AddListener(() => { constant--; UpdateDisplay(); });

        //submitFormulaButton.onClick.AddListener(SubmitRule);
    }

    private void UpdateDisplay()
    {
        coefficientText.text = coefficient.ToString();
        signText.text = isAddition ? "+" : "-";
        constantText.text = Mathf.Abs(constant).ToString(); 
    }

    public string SubmitRule()
    {
        string formula = $"{coefficient}n{(isAddition ? "+" : "-")}{Mathf.Abs(constant)}";
        Debug.Log($"User submitted formula: {formula}");

        return formula;

        //FindFirstObjectByType<SequenceGameManager>().ValidateFormula(coefficient, adjustedConstant);
    }
}