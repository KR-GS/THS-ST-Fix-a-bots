using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FormulaInputPanel : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelRoot;
    public TMP_Text coefficientText;
    public TMP_Text constantText;
    public TMP_Text feedbackText;
    public TextMeshProUGUI signText;

    public Button coefUpButton;
    public Button coefDownButton;

    public Button constUpButton;
    public Button constDownButton;

    public Button submitButton;

    [Header("Lock Settings")]
    public bool lockCoefficient = false;
    public bool lockConstant = false;

    private int currentCoef = 0;
    private int currentConst = 0;

    private Sequence targetSequence;
    private GameTimer gameTimer;

    public void ShowPanel(Sequence sequence, GameTimer gameTimer1)
    {
        gameTimer = gameTimer1;
        targetSequence = sequence;

        currentCoef = 0;
        currentConst = 0;

        ApplyLockSettings();
        UpdateFormulaText();
        feedbackText.text = "Adjust the formula and submit.";
        panelRoot.SetActive(true);
    }

    public void HidePanel()
    {
        panelRoot.SetActive(false);
    }

    private void ApplyLockSettings()
    {
        if (lockCoefficient)
        {
            coefUpButton.transform.localScale = new Vector3(0, 0, 0);
            coefDownButton.transform.localScale = new Vector3(0, 0, 0);
            currentCoef = targetSequence.Coefficient;
        }
        if (lockConstant)
        {
            constUpButton.transform.localScale = new Vector3(0, 0, 0);
            constDownButton.transform.localScale = new Vector3(0, 0, 0);
            currentConst = targetSequence.Constant;
        }

    }

    private void Start()
    {
        coefUpButton.onClick.AddListener(() => { currentCoef += 1; UpdateFormulaText(); });
        coefDownButton.onClick.AddListener(() => { currentCoef -= 1; UpdateFormulaText(); });

        constUpButton.onClick.AddListener(() => { currentConst += 1; UpdateFormulaText(); });
        constDownButton.onClick.AddListener(() => { currentConst -= 1; UpdateFormulaText(); });

        submitButton.onClick.AddListener(ValidateFormula);
    }

    private void UpdateFormulaText()
    {
        coefficientText.text = $"{currentCoef}";
        signText.text = currentConst >= 0 ? "+" : "-";
        constantText.text = currentConst >= 0 ? $"{currentConst}" : $"{-currentConst}";
    }

    private void ValidateFormula()
    {
        if (currentCoef != targetSequence.Coefficient && currentConst != targetSequence.Constant)
        {
            feedbackText.text = "Both are wrong";
        }
        else if (currentCoef != targetSequence.Coefficient && currentConst == targetSequence.Constant)
        {
            feedbackText.text = "Coefficient is wrong";
        }
        else if (currentCoef == targetSequence.Coefficient && currentConst != targetSequence.Constant)
        {
            feedbackText.text = "Constant is wrong";
        }
        else if (currentCoef == targetSequence.Coefficient && currentConst == targetSequence.Constant)
        {
            gameTimer.StopTimer();
            feedbackText.text = "Both are right";
        }
    }
}