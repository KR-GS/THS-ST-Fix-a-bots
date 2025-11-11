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
    private Button done_Btn;

    [SerializeField]
    private Button left_Btn;

    [SerializeField]
    private Button right_Btn;

    [SerializeField]
    private Tutorial[] tutorials;

    private int currentSlide = 0;

    private bool tutorialComplete = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void OpenTutorial()
    {
        done_Btn.gameObject.SetActive(false);

        tutorialUI.enabled = true;

        patt_sprite.sprite = tutorials[currentSlide].patt_sprite;

        description_TMP.text = tutorials[currentSlide].description;

        tutorial_display.texture = tutorials[currentSlide].display;

        right_Btn.gameObject.SetActive(true);
        left_Btn.gameObject.SetActive(false);

        Time.timeScale = 0;
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

        if(currentSlide >= tutorials.Length - 1)
        {
            done_Btn.gameObject.SetActive(true);
            right_Btn.gameObject.SetActive(false);
            left_Btn.gameObject.SetActive(true);
        }
        else
        {
            right_Btn.gameObject.SetActive(true);
            left_Btn.gameObject.SetActive(true);
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

        if (currentSlide <= 0)
        {
            right_Btn.gameObject.SetActive(true);
            left_Btn.gameObject.SetActive(false);
        }
        else
        {
            right_Btn.gameObject.SetActive(true);
            left_Btn.gameObject.SetActive(true);
        }
    }

    public void CloseTutorial()
    {
        tutorialComplete = true;

        currentSlide = 0;

        tutorialUI.enabled = false;

        Time.timeScale = 1;
    }
}
