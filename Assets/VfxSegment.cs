using UnityEngine;

public class VfxSegment : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem sparks_Vfx;

    [SerializeField]
    private ParticleSystem static_Vfx;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    
    public void ToggleVFXAnimOn()
    {
        var sparks_emission = sparks_Vfx.emission;

        var static_emission = static_Vfx.emission;

        sparks_emission.enabled = false;

        static_emission.enabled = false;
    }

    public void ToggleVFXAnimOff()
    {
        var sparks_emission = sparks_Vfx.emission;

        var static_emission = static_Vfx.emission;

        sparks_emission.enabled = true;

        static_emission.enabled = true;
    }
}
