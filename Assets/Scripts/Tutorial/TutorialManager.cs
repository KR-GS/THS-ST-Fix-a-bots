using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private SoundEffectsManager soundEffectsManager;

    [SerializeField]
    private Canvas tutorialUI;

    [SerializeField]
    private TextMeshProUGUI description_TMP;

    [SerializeField]
    private Image patt_sprite;

    [SerializeField]
    private RawImage tutorial_display;

    [SerializeField]
    private VideoPlayer tutorial_video;

    [SerializeField]
    private Button done_Btn;

    [SerializeField]
    private Button left_Btn;

    [SerializeField]
    private Button right_Btn;

    [SerializeField]
    private RenderTexture video_texture;

    [SerializeField]
    private Tutorial[] tutorials;

    private int currentSlide = 0;

    private bool tutorialComplete = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        GameObject SEM = GameObject.Find("Sound_Effect_Manager");
        soundEffectsManager = SEM.GetComponent<SoundEffectsManager>();
    }

    public void OpenTutorial()
    {
        soundEffectsManager.playHitSound();
        done_Btn.gameObject.SetActive(false);

        tutorialUI.enabled = true;

        patt_sprite.sprite = tutorials[currentSlide].patt_sprite;

        description_TMP.text = tutorials[currentSlide].description;

        if(tutorials[currentSlide].display != null)
        {
            Debug.Log("Hello from picture display");
            //tutorial_video.clip = null;

            tutorial_display.texture = tutorials[currentSlide].display;

        }
        else
        {
            Debug.Log("Hello from video display");
            tutorial_video.clip = tutorials[currentSlide].display_video;

            tutorial_display.texture = video_texture;
        }
            

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

            if (tutorials[currentSlide].display != null)
            {
                tutorial_display.texture = tutorials[currentSlide].display;
            }
            else
            {
                tutorial_video.clip = tutorials[currentSlide].display_video;

                tutorial_display.texture = video_texture;
            }
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

            if (tutorials[currentSlide].display != null)
            {
                tutorial_display.texture = tutorials[currentSlide].display;
            }
            else
            {
                tutorial_video.clip = tutorials[currentSlide].display_video;

                tutorial_display.texture = video_texture;
            }
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
