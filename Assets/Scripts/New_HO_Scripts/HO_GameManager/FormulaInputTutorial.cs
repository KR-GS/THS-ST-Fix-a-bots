using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.VisualScripting;


public class FormulaInputTutorial : MonoBehaviour
{

    public GameObject constBubble, coefBubble, tutorialPanel;
    public Button tutorialEndButton;

    public void Start()
    {
        tutorialEndButton.onClick.AddListener(() => tutorialPanel.SetActive(false));
        ShowTutorial(StaticData.tutorialType);
    }

    public void ShowTutorial(int tutorialNum)
    {
        Debug.Log("Showing Tutorial: " + tutorialNum);
        switch (tutorialNum)
        {
            case 0:
                tutorialPanel.SetActive(false);
                break;
            case 1:
                tutorialPanel.SetActive(true);
                coefBubble.SetActive(true);
                break;
            case 2:
                tutorialPanel.SetActive(true);
                constBubble.SetActive(true);
                break;
            case 3:
                tutorialPanel.SetActive(true);
                constBubble.SetActive(true);
                coefBubble.SetActive(true);
                break;
        }

    }

}