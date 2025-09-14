using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using System.Collections;
using Unity.VisualScripting;
using TMPro;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;
    public GameObject loadingscreenObjects;
    public Slider progressBar;
    public TextMeshProUGUI tiptext;


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
    
    public void SwitchtoScene(int id)
    {
        loadingscreenObjects.SetActive(true);
        progressBar.value = 0;

        int tipIndex = Random.Range(0, 6);

        switch (tipIndex)
        {
            case 0:
                tiptext.text = "TIP: The mitochondria is the powerhouse of the whole cell!";
                break;

            case 1:
                tiptext.text = "TIP: Counting is the main key in this game! Be careful in counting.";
                break;

            case 2:
                tiptext.text = "TIP: New orders can't be received if you don't finish the current one!";
                break;

            case 3:
                tiptext.text = "TIP: Brave men do not fear if the scheduler turns RR at time slice 7!";
                break;

            case 4:
                tiptext.text = "TIP: You can always restart the level if you make a mistake!";
                break;

            case 5:
                tiptext.text = "TIP: Don't forget your addition and subtraction, you will need it!";
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

    }
}
