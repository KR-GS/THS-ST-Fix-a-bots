using TMPro;
using UnityEngine;

public class GameLoopManager : MonoBehaviour
{
    public static GameLoopManager Instance;

    public int level = 0;

    public TextMeshPro dayNumber;


    private void Awake()
    {
        Instance = this;
        dayNumber.text = "Day: " + level;
    }

    public void StartNewLevel()
    {
        level++;
        dayNumber.text = "Day: " + level;
        Debug.Log("Starting Level " + level);
    }

    public void CompleteLevel()
    {
        Debug.Log("Level " + level + " complete!");
        StartNewLevel();
    }
}
