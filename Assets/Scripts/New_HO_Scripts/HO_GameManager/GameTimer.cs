using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    private float elapsedTime;
    private bool isRunning;

    public void StartTimer()
    {
        elapsedTime = 0f;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public float getElapsedTime()
    {
        return elapsedTime;
    }

    private void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime);
            timerText.text = timeSpan.ToString(@"mm\:ss\.ff");
        }
    }
}
