using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

//using System;
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

    public TextMeshPro remainingOrders;

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
            cam.gameObject.SetActive(scene.name == "LO_WS2D"); //LO_Workshop
        }

        if (scene.name == "LO_WS2D")
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
                    Debug.LogWarning("TextMeshPro component not found on MoneyText object.");
                }
            }
            else
            {
                Debug.LogWarning("MoneyText object not found in LO_Workshop.");
            }

            GameObject remainingOrderObj = GameObject.Find("RemainingOrders");
            if (remainingOrderObj != null)
            {
                remainingOrders = remainingOrderObj.GetComponent<TextMeshPro>();
                if (remainingOrders != null)
                {
                    UpdateRemainingOrders(); 
                }
                else
                {
                    Debug.LogWarning("TextMeshPro component not found on RemainingOrders object.");
                }
            }
            else
            {
                Debug.LogWarning("RemainingOrders object not found in LO_Workshop.");
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

        StaticData.dayNo = this.level;

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

    public void UpdateMoneyText()
    {
        if (moneyText != null)
        {
            moneyText.text = "Money: " + money;
        }
    }

    public void UpdateRemainingOrders()
    {
        if (remainingOrders != null && OrderManager.Instance != null)
        {
            remainingOrders.text = "Remaining Orders: " + OrderManager.Instance.orderList.Count;
        }
    }

    public void StartNewLevel()
    {
        OrderManager.Instance.SetStatus(false);
        level++;
        StaticData.dayNo = level;
        OrderManager.Instance.orderReceived = false; // Reset order received status
        //OrderManager.Instance.SetStatus(false);
        RaycastInteractor ri = Object.FindFirstObjectByType<RaycastInteractor>();
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

    /*
    public void OnTimerExpired()
    {
        Debug.Log("Time's up! Game Over or auto-submit.");
        // Call any fail screen or auto-fail logic here
        // Example: Load failure screen or retry?
    }
    */
}
