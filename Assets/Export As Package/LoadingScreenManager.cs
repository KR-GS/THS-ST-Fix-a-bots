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

    public void SwitchtoSceneMath(int id)
    {
        ResetButtons();
        vanThink.sprite = vanThonk;
        pattspeak.sprite = pattSpoke;
        prompt.text = "While waiting, let's help Van solve Math equations.";
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


        StartCoroutine(WaitForAnswerThenLoad(id));
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
    IEnumerator WaitForAnswerThenLoad(int id)
    {
        // Wait until the player answers
        while (answered == false)
        {
            yield return null; // This allows Unity to continue running!
        }

        // Once answered, wait for 3 seconds then proceed with the next scene
        yield return new WaitForSeconds(1f);

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
            prompt.text = "Well done.";
        }
        else
        {
            HighlightButton(buttonIndex, Color.red);      // Wrong answer clicked
            HighlightButton(locAnswer, Color.green);      // Show correct answer
            vanThink.sprite = vanNotNice;
            pattspeak.sprite = pattNotNice;
            prompt.text = "Try again next time.";
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
                tiptext.text = "TIP: Everything follows a pattern, don't forget that!";
                break;

            case 1:
                tiptext.text = "TIP: Counting is the main key in this game! Be careful in counting.";
                break;

            case 2:
                tiptext.text = "TIP: New orders can't be received if you haven't finish the current one!";
                break;

            case 3:
                tiptext.text = "TIP: P.A.T.T loves hiding your toolbox, make sure to check under the sofa!";
                break;

            case 4:
                tiptext.text = "TIP: You can use the sticky notes to keep track of lots of things!";
                break;

            case 5:
                tiptext.text = "TIP: Don't forget your addition, subtraction and multiplication skills!";
                break;

            case 6:
                tiptext.text = "TIP: Always stay hydrated. Drink water, especially that the days are hot!";
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

    }
}
