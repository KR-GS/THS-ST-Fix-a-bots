using UnityEngine;

public class VfxSegment : MonoBehaviour
{
    [SerializeField]
    private int side;

    [SerializeField]
    private ParticleSystem sparks_Vfx;

    [SerializeField]
    private ParticleSystem static_Vfx;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    
    public void ToggleVFXAnimOff()
    {
        var sparks_emission = sparks_Vfx.emission;

        var static_emission = static_Vfx.emission;

        sparks_emission.enabled = false;

        static_emission.enabled = false;

        GetComponent<BoxCollider2D>().enabled = false;
    }

    public void ToggleVFXAnimOn()
    {
        var sparks_emission = sparks_Vfx.emission;

        var static_emission = static_Vfx.emission;

        sparks_emission.enabled = true;

        static_emission.enabled = true;

        GetComponent<BoxCollider2D>().enabled = true;
    }

    public int GetSide()
    {
        return side;
    }
}
