using UnityEngine;
using TMPro;

public class TimerScript : MonoBehaviour, IDataPersistence {
    //public static TimerScript instance;

    public float timeAmt = 900f; //300 seconds -> 5 minutes
    public float timeLft; //how much time you have left

    public TextMeshProUGUI timer; 

    private bool isRunning = false;
    private void Awake()
    {
        /*
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        //DontDestroyOnLoad(this.gameObject);
        */
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
        timeLft = 900f;
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
                timer = textObj.GetComponent<TextMeshProUGUI>();
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
        //StaticData.timeSpent = data.timeSpent;

        Debug.Log("I'm debugging to check time: " + timeLft.ToString() + " seconds left.");
        Debug.Log("Checking what time is it!: " + StaticData.timeSpent.ToString() + " seconds spent.");

        if (StaticData.timeSpent > 0)
        {
            if (timeLft == 0)
            {
                Debug.Log("Wait, why is the timer at 0s when applying time deduction?");
            }

            Debug.Log($"[TIMER] Applying pending deduction: {StaticData.timeSpent:F2}s");
            Debug.Log($"[TIMER] Before: {timeLft:F2}s");

            timeLft -= StaticData.timeSpent;

            Debug.Log($"[TIMER] After: {timeLft:F2}s");

            Debug.Log("Changing the time!");
            UpdateTimerDisp();
            Debug.Log("All done!");

        }

    }

    public void SaveData(ref GameData data)
    {
        data.time = timeLft;
        data.timeSpent = StaticData.timeSpent;

        Debug.Log("Did the time save: " + data.time.ToString() + " seconds left.");
        Debug.Log("Static data for timeSpent: " + StaticData.timeSpent.ToString() + " seconds spent.");
        Debug.Log("You spent your free time here: " + data.timeSpent.ToString() + " seconds spent.");
    }

    /*
    public void TimerFinished()
    {
        Debug.Log("Time's up! You failed the level.");
        if (glm != null)
        {
            glm.OnTimerExpired();
        }
    }
    */

}
