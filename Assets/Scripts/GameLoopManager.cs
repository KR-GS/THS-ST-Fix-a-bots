using NUnit.Framework;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoopManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private Camera cam;

    public static GameLoopManager Instance;

    public static DataPersistenceManager dpm;

    //[SerializeField] private String fileName;

    public int level = 0;

    public int money = 0;

    public TextMeshPro dayNumber;

    public TextMeshPro moneyText;

    private TimerScript timer;

    private void Awake()
    {
        //Instance = this;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        OrderManager.Instance.TryCompleteOrder();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called automatically whenever a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (cam != null)
        {
            cam.gameObject.SetActive(scene.name == "LO_Workshop");
        }

        if (scene.name == "LO_Workshop")
        {
            // Re-find and assign the new instance of the text
            GameObject dayTextObject = GameObject.Find("Day_Number");
            if (dayTextObject != null)
            {
                dayNumber = dayTextObject.GetComponent<TextMeshPro>();
                if (dayNumber != null)
                {
                    dayNumber.text = "Day: " + level;
                }
                else
                {
                    Debug.LogWarning("TextMeshPro component not found on DayText object.");
                }
            }
            else
            {
                Debug.LogWarning("DayText object not found in LO_Workshop.");
            }

            GameObject moneyTextObject = GameObject.Find("MoneyText");
            if (moneyTextObject != null)
            {
                moneyText = moneyTextObject.GetComponent<TextMeshPro>();
                if (moneyText != null)
                {
                    moneyText.text = "Money: " + money;
                }
                else
                {
                    Debug.LogWarning("TextMeshPro component not found on DayText object.");
                }
            }
            else
            {
                Debug.LogWarning("MoneyText object not found in LO_Workshop.");
            }

            if (TimerScript.instance != null)
            {
                if (OrderManager.Instance != null && OrderManager.Instance.GetCurrentOrder() != null)
                {
                    TimerScript.instance.StartTimer();
                }
                else
                {
                    Debug.Log("No active order. Timer not restarted.");
                }
            }

        }
    }
    public enum DifficultyLevel
    {
        tutorial,
        easy,
        medium,
        hard
    }

    public void LoadData(GameData data)
    {
        this.level = data.level;
        this.money = data.money;
        if (dayNumber != null)
        {
            dayNumber.text = "Day: " + this.level;
        }

        if (moneyText != null)
        {
            moneyText.text = "Money: " + this.money;
        }
        Debug.Log("Level: " + level);
        Debug.Log("Money: " + money);
    }

    public void SaveData(ref GameData data)
    {
        data.level = this.level;
        data.money = this.money;
    }

    public void StartNewLevel()
    {
        level++;
        money += OrderManager.Instance.GetPrize();
        OrderManager.Instance.SetStatus(false);
        dayNumber.text = "Day: " + this.level;
        moneyText.text = "Money: " + this.money;
        Debug.Log("Starting Level " + level);
        if (TimerScript.instance != null && TimerScript.instance.timer != null)
        {
            TimerScript.instance.timer.gameObject.SetActive(false); // hide
        }
        TimerScript.instance.ResetTimer();
    }

    public void CompleteLevel()
    {
        Debug.Log("Level " + level + " complete!");
        StartNewLevel();
    }

    public DifficultyLevel GetDifficultyLevel()
    {
        if (level <= 5)
            return DifficultyLevel.easy;
        else if (level > 5 && level <= 10)
            return DifficultyLevel.medium;
        else
            return DifficultyLevel.hard;
    }

    /*
    public void OnTimerExpired()
    {
        Debug.Log("Time's up! Game Over or auto-submit.");
        // Call any fail screen or auto-fail logic here
        // Example: Load failure screen or retry?
    }
    */
}
