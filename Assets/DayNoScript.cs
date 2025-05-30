using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DayNoScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private int dayNumber = 0;
    private TextMeshProUGUI dayNumText;

    public void Awake()
    {
        dayNumText = this.GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
