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

        int tipIndex = Random.Range(0, 5);

        switch (tipIndex)
        {
            case 0:
                tiptext.text = "TIP: Mitochondria is the powerhouse of the whole cell.";
                break;
            case 1:
                tiptext.text = "TIP: Counting saves lives. Do not be lazy to count!";
                break;
            case 2:
                tiptext.text = "TIP: Everything is just addition and subtraction, do not overthink it!";
                break;
            case 3:
                tiptext.text = "Tip: Brave men to not fear if scheduler turns RR at time slice 7.";
                break;
            case 4:
                tiptext.text = "Tip: Always stay hydrated. Drink lots of water, especially on hot days!";
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
