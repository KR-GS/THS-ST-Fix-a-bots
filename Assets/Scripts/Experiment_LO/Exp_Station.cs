using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class Exp_Station : MonoBehaviour
{
    public Button ToolHard;
    public Button ToolEasy;
    public Button StickHard;
    public Button StickEasy;

    void Start()
    {
        // Add listeners to each button
        ToolHard.onClick.AddListener(OnToolHardPressed);
        ToolEasy.onClick.AddListener(OnToolEasyPressed);
        StickHard.onClick.AddListener(OnStickerHardPressed);
        StickEasy.onClick.AddListener(OnStickerEasyPressed);
    }

    void OnToolHardPressed()
    {
        // Multiple actions
        SceneManager.LoadScene("LO_Tool_GUI");
        // currentOrder.toolDone = true;
        StaticData.diffInt = 2;
        Debug.Log("Hard tool station activated!");
    }

    void OnToolEasyPressed()
    {
        SceneManager.LoadScene("LO_Tool_GUI");
        // currentOrder.toolDone = true;
        StaticData.diffInt = 0;
        Debug.Log("Easy tool station activated!");
    }

    void OnStickerHardPressed()
    {
        SceneManager.LoadScene("LO_Paint_GUI");
        // currentOrder.toolDone = true;
        StaticData.diffInt = 2;
        Debug.Log("Hard paint station activated!");
    }

    void OnStickerEasyPressed()
    {
        SceneManager.LoadScene("LO_Paint_GUI");
        // currentOrder.toolDone = true;
        StaticData.diffInt = 0;
        Debug.Log("Easy paint station activated!");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse clicked");
        }
    }

}
