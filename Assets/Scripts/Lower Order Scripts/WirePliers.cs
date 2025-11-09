using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WirePliers : MonoBehaviour
{
    [SerializeField]
    private Animator plier_animator;

    [SerializeField]
    private VfxSegment partToCut;

    [SerializeField]
    private List<Transform> positionsToCutAt = new List<Transform>();

    [SerializeField]
    private float speed;

    private bool onPart = false;

    private int slotNo = -1;

    private Vector3 origPos;

    private int current_Index = 0;

    void Awake()
    {
        origPos = transform.position;
    }

    void Update()
    {
        StartCoroutine(TriggerPlierAnimation());
    }

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

    public void GetWiresToCut(Transform new_pos)
    {
        positionsToCutAt.Add(new_pos);
    }

    public void ClearWiresToCut()
    {
        positionsToCutAt.Clear();
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

    public void SetSegment(VfxSegment next_Segment)
    {
        partToCut = next_Segment;
    }

    public void TriggerPlierMovement(Vector3 position, int side)
    {
        Quaternion target_rot = Quaternion.Euler(0,0,0);
        Vector3 target = new Vector3(0, 0, 0);

        //target = partToCut.transform.position;

        if (partToCut.GetSide() == 1)
        {
            target_rot = Quaternion.Euler(0, 0, -90);
        }
        else
        {
            target_rot = Quaternion.Euler(0, 0, 0);
        }

        transform.position = position;

        transform.rotation = target_rot;

        Debug.Log("Movement Done!");
    }

    private IEnumerator TriggerPlierAnimation()
    {
        if(current_Index < positionsToCutAt.Count)
        {
            partToCut = positionsToCutAt[current_Index].GetComponent<VFXManager>().GetSegment(0);
            TriggerPlierMovement(positionsToCutAt[current_Index].GetComponent<VFXManager>().GetPosition(0), 0);

            yield return new WaitForSeconds(2f);

            partToCut = positionsToCutAt[current_Index].GetComponent<VFXManager>().GetSegment(1);

            TriggerPlierMovement(positionsToCutAt[current_Index].GetComponent<VFXManager>().GetPosition(1), 1);

            yield return null;

            current_Index++;
        }
        else
        {
            current_Index = 0;
        }
    }
}
