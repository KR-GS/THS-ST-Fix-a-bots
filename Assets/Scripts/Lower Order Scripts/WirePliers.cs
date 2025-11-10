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

    private bool isPlaying = false;

    private bool isCutting = false;

    void Awake()
    {
        origPos = transform.position;
    }

    void Update()
    {
        if (!isCutting)
        {
            if (!isPlaying)
            {
                StartCoroutine(TriggerPlierAnimation());
            }
        }
    }

    /*
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
    */

    public void DisableCutting()
    {
        isCutting = true;
        plier_animator.SetBool("IsWaiting", false);
    }

    public void EnableCutting()
    {
        isCutting = false;

        plier_animator.SetBool("IsWaiting", true);
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

    public void RemoveCurrentVFX()
    {
        positionsToCutAt.RemoveAt(current_Index);
    }

    public bool CheckList()
    {
        if (positionsToCutAt.Count > 0)
        {
            current_Index = 0;

            return true;
        } 

        return false;
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
        isPlaying = true;
        if (current_Index < positionsToCutAt.Count)
        {
            VfxSegment[] segments = positionsToCutAt[current_Index].GetComponent<VFXManager>().GetSegment();

            Debug.Log(segments.Length);
            partToCut = segments[0];
            TriggerPlierMovement(segments[0].transform.position, 0);

            yield return new WaitForSeconds(3f);

            //partToCut = positionsToCutAt[current_Index].GetComponent<VFXManager>().GetSegment(1);
            partToCut = segments[1];

            TriggerPlierMovement(segments[1].transform.position, 1);

            yield return new WaitForSeconds(3f);

            current_Index++;

            if (current_Index >= positionsToCutAt.Count)
            {
                current_Index = 0;
            }

            
        }

        //yield return new WaitForSeconds(3f);

        isPlaying = false;
    }
}
