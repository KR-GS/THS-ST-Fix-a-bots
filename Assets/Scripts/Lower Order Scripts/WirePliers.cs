using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;

public class WirePliers : MonoBehaviour
{
    [SerializeField]
    private Animator plier_animator;

    [SerializeField]
    private VfxSegment partToCut;

    [SerializeField]
    private float speed;

    private bool onPart = false;

    private int slotNo = -1;

    private Vector3 origPos;

    void Awake()
    {
        origPos = transform.position;
    }

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

    public VfxSegment GetSegment()
    {
        return partToCut;
    }

    public void TriggerCuttingAnim()
    {
        plier_animator.SetTrigger("IsCutting");
    }

    public void StopParticleEmission()
    {
        partToCut.ToggleVFXAnimOff();
        //StartCoroutine(StopVFXInOrder());
    }

    public IEnumerator TriggerPlierMovement(bool cuttingPart, float speed_Inc)
    {
        Quaternion target_rot = Quaternion.Euler(0,0,0);
        Vector3 target = new Vector3(0, 0, 0);

        if (cuttingPart)
        {
            target = partToCut.transform.position;

            if (partToCut.GetSide() == 1)
            {
                target_rot = Quaternion.Euler(0, 0, -90);
            }
        }
        else
        {
            target = origPos;
        }

        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            if (Vector3.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, (speed * speed_Inc) * Time.deltaTime);
            }

            yield return null;
        }

        while(Quaternion.Angle(transform.rotation, target_rot) > 0.01f)
        {
            

            if (Quaternion.Angle(transform.rotation, target_rot) > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, target_rot, speed * Time.deltaTime);
            }
            
            yield return null;
        }

        Debug.Log("Movement Done!");
    }

    public void ChangePosRot()
    {

    }
}
