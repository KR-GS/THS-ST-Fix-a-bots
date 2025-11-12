using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using System.Collections;
using Unity.VisualScripting;
using TMPro;
using System.Linq.Expressions;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;
    public GameObject loadingscreenObjects;
    public GameObject loadingscreenMath;
    public GameObject loadingscreenGear;
    public Slider progressBar;
    public TextMeshProUGUI tiptext;
    public TextMeshProUGUI prompt;
    public TextMeshProUGUI equation;
    //public SpriteRenderer loadingScreen;
    //public SpriteRenderer loadingMath;
    public Image vanThink;
    public Sprite vanThonk;
    public Sprite vanNice;
    public Sprite vanNotNice;
    public Image pattspeak;
    public Sprite pattSpoke;
    public Sprite pattNice;
    public Sprite pattNotNice;
    public Button a;
    public Button b;
    public Button c;
    private int number;
    private int number2;
    private int operation;
    private int answer;
    private int locAnswer; //which of the 3 button gets the answer?
    private bool answered;
    private int options1;
    private int options2;


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void SwitchtoSceneGear(int id)
    {
        loadingscreenGear.SetActive(true);
        StartCoroutine(SwitchtoSceneAsync(id));
    }

    public void SwitchtoSceneMath(int id, System.Action onComplete = null)
    {
        ResetButtons();
        vanThink.sprite = vanThonk;
        pattspeak.sprite = pattSpoke;
        prompt.text = "Habang naghihintay, tulungan natin si Van sumagot!";
        loadingscreenMath.SetActive(true);
        progressBar.value = 0;
        answered = false;

        number = Random.Range(1, 51);
        number2 = Random.Range(1, 51);
        operation = Random.Range(0, 3); // 0: addition, 1: subtraction, 2: multiplication

        if(operation == 0)
        {
            answer = number + number2;
        }
        else if (operation == 1)
        {
            answer = number - number2;
        }
        else
        {
            answer = number * number2;
        }

        equation.text = "" + number + " " + GetOperator(operation) + " " + number2;

        locAnswer = Random.Range(0, 3);


        if(locAnswer  == 0)
        {
            a.GetComponentInChildren<TextMeshProUGUI>().text = answer.ToString();

            if (options1 == 0 && options2 == 0)
            {
                do
                {
                    b.GetComponentInChildren<TextMeshProUGUI>().text = (answer + Random.Range(1, 10)).ToString();
                    c.GetComponentInChildren<TextMeshProUGUI>().text = (answer + Random.Range(1, 10)).ToString();
                } while (b.GetComponentInChildren<TextMeshProUGUI>().text == c.GetComponentInChildren<TextMeshProUGUI>().text);

            }
            else if (options1 == 0 && options2 == 1)
            {
                b.GetComponentInChildren<TextMeshProUGUI>().text = (answer + Random.Range(1, 10)).ToString();
                c.GetComponentInChildren<TextMeshProUGUI>().text = (answer - Random.Range(1, 10)).ToString();
            }

            else if (options1 == 1 && options2 == 0)
            {
                b.GetComponentInChildren<TextMeshProUGUI>().text = (answer - Random.Range(1, 10)).ToString();
                c.GetComponentInChildren<TextMeshProUGUI>().text = (answer + Random.Range(1, 10)).ToString();
            }
            else if (options1 == 1 && options2 == 1)
            {
                do
                {
                    b.GetComponentInChildren<TextMeshProUGUI>().text = (answer + Random.Range(1, 10)).ToString();
                    c.GetComponentInChildren<TextMeshProUGUI>().text = (answer + Random.Range(1, 10)).ToString();
                } while (b.GetComponentInChildren<TextMeshProUGUI>().text == c.GetComponentInChildren<TextMeshProUGUI>().text);
            }
            
        }
        else if (locAnswer == 1)
        {
            b.GetComponentInChildren<TextMeshProUGUI>().text = answer.ToString();

            if (options1 == 0 && options2 == 0)
            {
                do
                {
                    a.GetComponentInChildren<TextMeshProUGUI>().text = (answer + Random.Range(1, 10)).ToString();
                    c.GetComponentInChildren<TextMeshProUGUI>().text = (answer + Random.Range(1, 10)).ToString();
                } while (a.GetComponentInChildren<TextMeshProUGUI>().text == c.GetComponentInChildren<TextMeshProUGUI>().text);

            }
            else if (options1 == 0 && options2 == 1)
            {
                a.GetComponentInChildren<TextMeshProUGUI>().text = (answer + Random.Range(1, 10)).ToString();
                c.GetComponentInChildren<TextMeshProUGUI>().text = (answer - Random.Range(1, 10)).ToString();
            }

            else if (options1 == 1 && options2 == 0)
            {
                a.GetComponentInChildren<TextMeshProUGUI>().text = (answer - Random.Range(1, 10)).ToString();
                c.GetComponentInChildren<TextMeshProUGUI>().text = (answer + Random.Range(1, 10)).ToString();
            }
            else if (options1 == 1 && options2 == 1)
            {
                do
                {
                    a.GetComponentInChildren<TextMeshProUGUI>().text = (answer + Random.Range(1, 10)).ToString();
                    c.GetComponentInChildren<TextMeshProUGUI>().text = (answer + Random.Range(1, 10)).ToString();
                } while (a.GetComponentInChildren<TextMeshProUGUI>().text == c.GetComponentInChildren<TextMeshProUGUI>().text);
            }

        }
        else if (locAnswer == 2)
        {
            c.GetComponentInChildren<TextMeshProUGUI>().text = answer.ToString();

            if (options1 == 0 && options2 == 0)
            {
                do
                {
                    b.GetComponentInChildren<TextMeshProUGUI>().text = (answer + Random.Range(1, 10)).ToString();
                    a.GetComponentInChildren<TextMeshProUGUI>().text = (answer + Random.Range(1, 10)).ToString();
                } while (b.GetComponentInChildren<TextMeshProUGUI>().text == a.GetComponentInChildren<TextMeshProUGUI>().text);

            }
            else if (options1 == 0 && options2 == 1)
            {
                b.GetComponentInChildren<TextMeshProUGUI>().text = (answer + Random.Range(1, 10)).ToString();
                a.GetComponentInChildren<TextMeshProUGUI>().text = (answer - Random.Range(1, 10)).ToString();
            }

            else if (options1 == 1 && options2 == 0)
            {
                b.GetComponentInChildren<TextMeshProUGUI>().text = (answer - Random.Range(1, 10)).ToString();
                a.GetComponentInChildren<TextMeshProUGUI>().text = (answer + Random.Range(1, 10)).ToString();
            }
            else if (options1 == 1 && options2 == 1)
            {
                do
                {
                    b.GetComponentInChildren<TextMeshProUGUI>().text = (answer + Random.Range(1, 10)).ToString();
                    a.GetComponentInChildren<TextMeshProUGUI>().text = (answer + Random.Range(1, 10)).ToString();
                } while (b.GetComponentInChildren<TextMeshProUGUI>().text == a.GetComponentInChildren<TextMeshProUGUI>().text);
            }

        }


        StartCoroutine(WaitForAnswerThenLoad(id, onComplete));
    }

    public string GetOperator(int choice)
    {
        if (choice == 0)
        {
            return "+";
        }
        else if (choice == 1)
        {
            return "-";
        }
        else
        {
            return "x";
        }
    }
    IEnumerator WaitForAnswerThenLoad(int id, System.Action onComplete)
    {
        // Wait until the player answers
        while (answered == false)
        {
            yield return null; // This allows Unity to continue running!
        }

        // Once answered, wait for 3 seconds then proceed with the next scene
        yield return new WaitForSeconds(1f);

        onComplete?.Invoke();

        yield return StartCoroutine(SwitchtoSceneAsync(id));
    }

    public void OnAnswerButtonClicked(int buttonIndex)
    {
        if (answered) return; // Prevent multiple clicks

        answered = true;

        bool isCorrect = (buttonIndex == locAnswer);

        // Highlight the buttons
        if (isCorrect)
        {
            HighlightButton(buttonIndex, Color.green); // Correct answer
            vanThink.sprite = vanNice;
            pattspeak.sprite = pattNice;
            prompt.text = "Magaling!";
        }
        else
        {
            HighlightButton(buttonIndex, Color.red);      // Wrong answer clicked
            HighlightButton(locAnswer, Color.green);      // Show correct answer
            vanThink.sprite = vanNotNice;
            pattspeak.sprite = pattNotNice;
            prompt.text = "Hala, sa susnod nalang...";
        }
    }

    void HighlightButton(int buttonIndex, Color color)
    {
        Button targetButton = null;

        if (buttonIndex == 0) targetButton = a;
        else if (buttonIndex == 1) targetButton = b;
        else if (buttonIndex == 2) targetButton = c;

        if (targetButton != null)
        {
            // Change the button's Image color directly
            Image buttonImage = targetButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = color;
            }

            targetButton.interactable = false;
        }
    }

    void ResetButtons()
    {
        ResetSingleButton(a);
        ResetSingleButton(b);
        ResetSingleButton(c);
    }

    void ResetSingleButton(Button button)
    {
        if (button != null)
        {
            
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = Color.white; 
            }

            button.interactable = true;
        }
    }

    public void SwitchtoScene(int id)
    {
        
        loadingscreenObjects.SetActive(true);
        progressBar.value = 0;

        int tipIndex = Random.Range(0, 6);

        switch (tipIndex)
        {
            case 0:
                tiptext.text = "TIP: Sumusunod ang lahat sa pattern, huwag mong kalimutan!";
                break;

            case 1:
                tiptext.text = "TIP: Ang pagbibilang ay ang susi sa game. Focus ka sa pagbibilang!";
                break;

            case 2:
                tiptext.text = "TIP: Ang bagong orders ay hindi makukuha kapag hindi mo natapos ang luma.";
                break;

            case 3:
                tiptext.text = "TIP: Mahilig itago ni P.A.T.T ang iyong toolbox, huwag mong kalimutan tumingin sa ilalim ng sofa!";
                break;

            case 4:
                tiptext.text = "TIP: Pwede mo gamitin ang sticky notes para maalala mo ang mga bagay-bagay!";
                break;

            case 5:
                tiptext.text = "TIP: Huwag mong kalimutan ang iyong skills sa addition, subtraction at multiplication!";
                break;

            case 6:
                tiptext.text = "TIP: Huwag magpauhaw. Uminom ka ng tubig, lalo na maiinit ang mga araw!";
                break;
        }

        StartCoroutine(SwitchtoSceneAsync(id));
    }

    IEnumerator SwitchtoSceneAsync(int id)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(id);
        while (!operation.isDone)
        {
            progressBar.value = operation.progress;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        loadingscreenObjects.SetActive(false);
        loadingscreenMath.SetActive(false);
        loadingscreenGear.SetActive(false);

    }
}
