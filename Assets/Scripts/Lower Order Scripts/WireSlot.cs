using UnityEngine;

public class WireSlot : MonoBehaviour
{
    [SerializeField]
    private int slotValue;

    [SerializeField]
    private bool isMissing;

    private GameObject wireToAdd;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Wire wire))
        {
            wire.SetSlotStatus();
            wire.SetNewWirePos(transform);
            wireToAdd = wire.transform.gameObject;
        }
    }
}
