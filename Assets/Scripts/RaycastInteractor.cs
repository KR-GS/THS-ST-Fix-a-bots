using System.Globalization;
using UnityEngine;

public class RaycastInteractor : MonoBehaviour
{
    public float rayLength = 10f;
    public int tvNum = 0;
    [SerializeField] private GameLoopManager gameLoopManager;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, rayLength))
            {
                Debug.Log("Raycast hit: " + hit.collider.name);

                // Station interaction
                Station station = hit.collider.GetComponent<Station>();
                if (station != null)
                {
                    Debug.Log("Station found: " + station.name);
                    station.Interact();
                    return;
                }

                // TV interaction
                if (hit.collider.CompareTag("TV"))
                {
                    if (tvNum != gameLoopManager.level)
                    {
                        tvNum++;
                        Debug.Log("TV clicked - generating new order.");
                        OrderManager.Instance.CreateNewOrder();
                    }
                    else
                    {
                        Debug.Log("You have unfinished orders!");
                    }
                }
            }
            else
            {
                Debug.Log("Raycast did not hit anything.");
            }
        }
    }
}