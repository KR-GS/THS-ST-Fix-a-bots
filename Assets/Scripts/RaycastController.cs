using UnityEngine;

public class RaycastController: MonoBehaviour
{
    public bool enableRaycast = true;

    void Update()
    {
        if (enableRaycast)
        {
            // Perform your raycast logic here
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Raycast hit: " + hit.collider.name);
            }
        }
    }

    // Call this method to enable the raycast
    public void EnableRaycasting()
    {
        enableRaycast = true;
    }

    // Call this method to disable the raycast
    public void DisableRaycasting()
    {
        enableRaycast = false;
    }
}
