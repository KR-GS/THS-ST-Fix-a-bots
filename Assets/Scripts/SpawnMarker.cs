using UnityEngine;

public class SpawnMarker : MonoBehaviour
{
    public GameObject markerPrefab;
    public Transform spawnParent; 
    public Vector2 spawnPosition;

    public void SpawnNewMarker()
    {
        GameObject newMarker = Instantiate(markerPrefab, spawnParent);
        newMarker.transform.SetSiblingIndex(9);
        newMarker.GetComponent<RectTransform>().anchoredPosition = spawnPosition;
    }
}