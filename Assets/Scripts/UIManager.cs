using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    [Header("Time Buttons")]
    public GameObject timePeriodButtonPrefab;
    public Transform buttonContainer;
    public Button confirmPatternButton;
    public Button backButton;
    public Button nextButton;

    [Header("Feedback and Rule Input")]
    public TextMeshProUGUI feedbackText;
    public GameObject ruleInputPanel;
    public TMP_InputField startInputField;
    public TMP_InputField diffInputField;
    public Button submitRuleButton;
    public TextMeshProUGUI ruleFeedbackText;

    private List<GameObject> allButtons = new List<GameObject>();
    private List<int> correctAnswers = new List<int>();
    private int currentPage = 0;
    private const int pageSize = 10;


    public void GenerateTimeButtons(int total = 60)
    {
        allButtons.Clear();
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 1; i <= total; i++)
        {
            GameObject btn = Instantiate(timePeriodButtonPrefab, buttonContainer);
            btn.GetComponentInChildren<TMP_Text>().text = i.ToString();
            btn.GetComponent<Button>().onClick.AddListener(() => OnTimeButtonClicked(btn));
            btn.SetActive(false);
            allButtons.Add(btn);
        }

        currentPage = 0;
        UpdatePage();
    }

    public void UpdatePage()
    {
        for (int i = 0; i < allButtons.Count; i++)
        {
            allButtons[i].SetActive(false);
        }

        int start = currentPage * pageSize;
        int end = Mathf.Min(start + pageSize, allButtons.Count);

        for (int i = start; i < end; i++)
        {
            allButtons[i].SetActive(true);
        }

        backButton.interactable = currentPage > 0;
        nextButton.interactable = end < allButtons.Count;
    }

    private void OnTimeButtonClicked(GameObject button)
    {
        Image img = button.GetComponent<Image>();
        bool selected = img.color == Color.yellow;
        img.color = selected ? Color.white : Color.yellow;
    }

    public void HighlightInitialSequenceNumbers(List<int> sequence)
    {
        foreach (GameObject btn in allButtons)
        {
            int btnTime = int.Parse(btn.GetComponentInChildren<TMP_Text>().text);
            if (sequence.Contains(btnTime))
            {
                btn.GetComponent<Image>().color = Color.green; 
                btn.GetComponent<Button>().interactable = false; 
            }
        }
    }

    public List<int> GetSelectedAnswers()
    {
        List<int> selected = new List<int>();
        foreach (GameObject btn in allButtons)
        {
            Image img = btn.GetComponent<Image>();
            if (img.color == Color.yellow)
            {
                int val = int.Parse(btn.GetComponentInChildren<TMP_Text>().text);
                selected.Add(val);
            }
        }
        return selected;
    }

    public void SetFeedback(string message, bool correct)
    {
        feedbackText.text = message;
        feedbackText.color = correct ? Color.green : Color.red;
    }

    public void ShowRuleInputPanel(bool show)
    {
        ruleInputPanel.SetActive(show);
    }

    public (int start, int diff) GetRuleInputs()
    {
        int.TryParse(startInputField.text, out int start);
        int.TryParse(diffInputField.text, out int diff);
        return (start, diff);
    }

    public void SetRuleFeedback(string message, bool correct)
    {
        ruleFeedbackText.text = message;
        ruleFeedbackText.color = correct ? Color.green : Color.red;
    }

    private void Start()
    {
        backButton.onClick.AddListener(() =>
        {
            if (currentPage > 0)
            {
                currentPage--;
                UpdatePage();
            }
        });

        nextButton.onClick.AddListener(() =>
        {
            if ((currentPage + 1) * pageSize < allButtons.Count)
            {
                currentPage++;
                UpdatePage();
            }
        });

        if (confirmPatternButton != null)
    {
        confirmPatternButton.onClick.AddListener(() =>
        {
            SequenceGameManager.Instance.ConfirmAnswer();
        });
    }

        if (submitRuleButton != null)
        {
            submitRuleButton.onClick.AddListener(() =>
            {
                Debug.Log($"Pressed Button");
                var (start, diff) = GetRuleInputs();
                Debug.Log($"Rule submitted: start={start}, diff={diff}");
                SequenceGameManager.Instance.ValidateFormula();
            });
        }
    }
}