using UnityEngine;
using UnityEngine.Rendering;

public class WirePliers : MonoBehaviour
{
    [SerializeField]
    private Animator plier_animator;

    [SerializeField]
    private VfxSegment partToCut;

    private bool onPart = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out VfxSegment segment))
        {
            Debug.Log("Interacting with " + segment.transform.name);
            partToCut = segment;
            onPart = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out VfxSegment segment))
        {
            partToCut = null;
            onPart = false;
        }
    }

    public bool GetIsOnPart()
    {
        return onPart;
    }

    public void TriggerCuttingAnim()
    {
        plier_animator.SetTrigger("IsCutting");
    }

    public void SetPositionToPart()
    {
        transform.position = partToCut.transform.position;
    }

    public void StopParticleEmission()
    {
        partToCut.ToggleVFXAnim();
    }
}
