using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour, IDataPersistence
{
    public static OrderManager Instance;
    [SerializeField] private GameObject orderCompletePanel;
    [SerializeField] private RaycastInteractor raycastInteractor;
    private Button button;
    private int prize;
    private bool isFinished = false;

    public List<Order> orderList = new List<Order>();
    public int currentOrderIndex = -1;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        HideOrderCompletePanel();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LO_WS2D")
        {
            Debug.Log("Returned to WorkshopScene. Checking for completed orders...");
            StartCoroutine(HandleWorkshopSceneLoad());
        }

    }

    private System.Collections.IEnumerator HandleWorkshopSceneLoad()
    {
        yield return null;

        if (isFinished)
        {
            Debug.Log("isFinished was true. Showing complete panel...");
            ShowOrderCompletePanel();
            raycastInteractor.enabled = false;
        }
        else
        {
            TryCompleteOrder(); 
        }
    }

    public Order CreateNewOrder()
    {
        if (TimerScript.instance != null)
        {
            TimerScript.instance.timer.gameObject.SetActive(true);  // show
        }
  
        Order newOrder = new Order
        {
            needsTool = Random.value > 0.5f,
            needsPaint = Random.value > 0.5f,
            needsWire = Random.value > 0.5f
        };

        // Ensure at least one requirement
        if (!newOrder.needsTool && !newOrder.needsPaint && !newOrder.needsWire)
            newOrder.needsTool = true;

        orderList.Add(newOrder);
        currentOrderIndex = orderList.Count - 1;

        Debug.Log("New Order Created!");

        if (TimerScript.instance != null)
        {
            TimerScript.instance.StartTimer();
        }

        return newOrder;
    }

    public void TryCompleteOrder()
    {
        if (GetCurrentOrder()?.IsComplete() ?? false)
        {
            isFinished = true;
            Debug.Log("Order Complete!");
            if (TimerScript.instance != null)
            {
                TimerScript.instance.StopTimer();
                if (TimerScript.instance.timeLft >= 240f)
                {
                    prize = 5;
                }
                else if (TimerScript.instance.timeLft >= 180f && TimerScript.instance.timeLft < 240f)
                {
                    prize = 4;
                }
                else if (TimerScript.instance.timeLft >= 120f && TimerScript.instance.timeLft < 180f)
                {
                    prize = 3;
                }
                else if (TimerScript.instance.timeLft >= 60f && TimerScript.instance.timeLft < 120f)
                {
                    prize = 2;
                }
                else if (TimerScript.instance.timeLft >= 1f && TimerScript.instance.timeLft < 60f)
                {
                    prize = 1;
                }
                else
                {
                    prize = 0;
                }
            }
            ShowOrderCompletePanel();
            raycastInteractor.enabled = false;
            orderList.RemoveAt(currentOrderIndex);
            currentOrderIndex = Mathf.Clamp(currentOrderIndex - 1, 0, orderList.Count - 1);
        }
    }

    public Order GetCurrentOrder()
    {
        if (currentOrderIndex >= 0 && currentOrderIndex < orderList.Count)
            return orderList[currentOrderIndex];

        return null;
    }

    public void ShowOrderCompletePanel()
    {
        Transform prizeTextFind = orderCompletePanel.transform.Find("PrizeText");
        if (prizeTextFind != null)
        {
            TextMeshProUGUI prizeTexts = prizeTextFind.GetComponent<TextMeshProUGUI>();
            if (prize == 5)
            {
                prizeTexts.text = "Excellent in all marks! Earned 5 currency!";
            }
            else if (prize == 4)
            {
                prizeTexts.text = "Very good job! Earned 4 currency!";
            }
            else if (prize == 3)
            {
                prizeTexts.text = "Good job! Earned 3 currency!";
            }
            else if (prize == 2)
            {
                prizeTexts.text = "Nice! Earned 2 currency!";
            }
            else if (prize == 1)
            {
                prizeTexts.text = "You took too long... but it's acceptable. Earned 1 currency.";
            }
            else
            {
                prizeTexts.text = "That was disappointing... Earned nothing.";
            }
        }
        orderCompletePanel.SetActive(true);
    }

    public void HideOrderCompletePanel()
    {
        orderCompletePanel.SetActive(false);
    }

    public void OnButtonClick()
    {
        HideOrderCompletePanel();
        GameLoopManager.Instance.CompleteLevel();
        raycastInteractor.enabled = true;
    }

    public void LoadData(GameData data)
    {
        this.orderList = data.savedOrders ?? new List<Order>();
        this.currentOrderIndex = data.currentOrderIndex;
        this.isFinished = data.finished;
        this.prize = data.prize;

        if (TimerScript.instance != null && GetCurrentOrder() != null)
        {
            TimerScript.instance.StartTimer();
            Debug.Log("Timer started from OrderManager.LoadData");
        }
    }

    public void SaveData(ref GameData data)
    {
        data.savedOrders = this.orderList;
        data.currentOrderIndex = this.currentOrderIndex;
        data.finished = this.isFinished;
        data.prize = this.prize;
    }

    public int GetPrize()
    {
        return prize;
    }

    public bool GetStatus()
    {
        return isFinished;
    }

    public void SetStatus(bool s)
    {
        isFinished = s;
    }
}