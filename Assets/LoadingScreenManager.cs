using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using System.Collections;
using Unity.VisualScripting;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;
    public GameObject loadingscreenObjects;
    public Slider progressBar;

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
