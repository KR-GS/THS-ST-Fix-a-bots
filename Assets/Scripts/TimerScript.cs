using UnityEngine;
using TMPro;

public class TimerScript : MonoBehaviour, IDataPersistence {
    public static TimerScript instance;

    public float timeAmt = 300f; //300 seconds -> 5 minutes
    public float timeLft; //how much time you have left

    public TextMeshPro timer; 

    private bool isRunning = false;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void StartTimer() {
        if (!isRunning && timeLft <= 0)
        {
            timeLft = timeAmt;
        }

        isRunning = true;
        UpdateTimerDisp();
    }

    public void StopTimer()
    {
        isRunning = false;
        Debug.Log("TIMER PAUSED");
    }

    public void ResetTimer()
    {
        timeLft = 300f;
    }
    public void Update()
    {
        if (!isRunning) return;

        timeLft -= Time.deltaTime;

        if(timeLft <= 0)
        {
            timeLft = 0;
            //isRunning = false;
            //TimerFinished();
        }

        UpdateTimerDisp();
    }

    public void UpdateTimerDisp()
    {
        if(timer == null)
        {
            GameObject textObj = GameObject.Find("Time_Text");
            if (textObj != null)
            {
                timer = textObj.GetComponent<TextMeshPro>();
            }
        }
        else if (timer != null)
        {
            int minutes = Mathf.FloorToInt(timeLft / 60);
            int seconds = Mathf.FloorToInt(timeLft % 60);
            timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    public void LoadData(GameData data)
    {
        timeLft = data.time;
        UpdateTimerDisp();
    }

    public void SaveData(ref GameData data)
    {
        data.time = timeLft;
    }

    /*
    public void TimerFinished()
    {
        Debug.Log("Time's up! You failed the level.");
        if (GameLoopManager.Instance != null)
        {
            GameLoopManager.Instance.OnTimerExpired();
        }
    }
    */

}
