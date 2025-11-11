using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private Canvas tutorialUI;

    [SerializeField]
    private TextMeshProUGUI description_TMP;

    [SerializeField]
    private Image patt_sprite;

    [SerializeField]
    private RawImage tutorial_display;

    [SerializeField]
    private Tutorial[] tutorials;

    private int currentSlide = 0;

    private bool tutorialComplete = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void OpenTutorial()
    {
        tutorialUI.enabled = true;

        patt_sprite.sprite = tutorials[currentSlide].patt_sprite;

        description_TMP.text = tutorials[currentSlide].description;

        tutorial_display.texture = tutorials[currentSlide].display;
    }

    public void PlayNextTutorial()
    {
        if (currentSlide < tutorials.Length-1)
        {
            currentSlide++;

            patt_sprite.sprite = tutorials[currentSlide].patt_sprite;

            description_TMP.text = tutorials[currentSlide].description;

            tutorial_display.texture = tutorials[currentSlide].display;
        }
    }

    public void PlayPrevTutorial()
    {
        if (currentSlide > 0)
        {
            currentSlide--;

            patt_sprite.sprite = tutorials[currentSlide].patt_sprite;

            description_TMP.text = tutorials[currentSlide].description;

            tutorial_display.texture = tutorials[currentSlide].display;
        }
    }

    public void CloseTutorial()
    {
        tutorialComplete = true;

        currentSlide = 0;

        tutorialUI.enabled = false;
    }
}
