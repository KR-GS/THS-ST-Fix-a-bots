using UnityEngine;

public class StationTimeManager : MonoBehaviour
{
    [SerializeField]
    float station_Time;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        station_Time = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        station_Time += Time.deltaTime;
    }

    public float GetStationTime()
    {
        return station_Time;
    }
}
