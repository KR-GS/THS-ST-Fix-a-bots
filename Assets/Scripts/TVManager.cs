using UnityEngine;

public class TVManager : MonoBehaviour
{
    private void Awake()
    {
        // Make sure only one TVManager exists
        GameObject[] tvs = GameObject.FindGameObjectsWithTag("TV");
        if (tvs.Length > 1)
        {
            Destroy(gameObject); // Remove duplicates when coming back
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
