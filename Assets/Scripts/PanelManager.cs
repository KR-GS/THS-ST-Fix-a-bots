using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance;

    [SerializeField] private GameObject orderCompletePanel;
    [SerializeField] private RaycastInteractor raycastInteractor;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowOrderCompletePanel()
    {
        orderCompletePanel.SetActive(true);
    }

    public void HideOrderCompletePanel()
    {
        orderCompletePanel.SetActive(false);
    }
}
