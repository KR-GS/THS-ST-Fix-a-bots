using UnityEngine;

public class WireSlot : MonoBehaviour
{
    [SerializeField]
    private int slotValue;

    [SerializeField]
    private bool isMissing;

    private GameObject wireToAdd;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
