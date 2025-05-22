using TMPro;
using UnityEngine;

public class GameLoopManager : MonoBehaviour
{
    public static GameLoopManager Instance;

    public int level = 1;

    public TextMeshPro dayNumber;

    private void Awake()
    {
        Instance = this;
    }

    public void StartNewLevel()
    {
        level++;
        dayNumber.text = "Day: " + level;
        Debug.Log("Starting Level " + level);
        OrderManager.Instance.CreateNewOrder();
    }

    public void CompleteLevel()
    {
        Debug.Log("Level " + level + " complete!");
        // Add difficulty logic here if needed
        StartNewLevel();
    }
}
