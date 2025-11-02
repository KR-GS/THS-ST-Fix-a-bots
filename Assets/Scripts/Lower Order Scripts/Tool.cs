using System.Collections;
using UnityEngine;

public class Tool : MonoBehaviour
{
    [SerializeField]
    private Transform addTenPos;

    [SerializeField]
    private float speed;

    private Animator statusAnimator;
    private AnimationClip[] clips;
    
    void Awake()
    {
        statusAnimator = GetComponentInChildren<Animator>();
        clips = statusAnimator.runtimeAnimatorController.animationClips;
        Debug.Log(clips.Length);
        //events = clips[1].events;

        //Debug.Log("Event time: " + events[0].time);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public IEnumerator TriggerToolAnimation(PartTile fastener)
    {
        GetComponentInChildren<ToolEvent>().SetCurrentFastener(fastener);
        GetComponent<Collider2D>().enabled = false;
        statusAnimator.SetTrigger("IsUsed");
        yield return new WaitForSeconds(1f);
        GetComponent<Collider2D>().enabled = true;
    }

    public void SetHeightValue(float value)
    {
        GetComponentInChildren<ToolEvent>().SetHeightToSet(value);
    }

    public Vector3 GetAddTenPos()
    {
        return Camera.main.WorldToScreenPoint(addTenPos.position);
    }

    public IEnumerator TriggerToolChange(float value)
    {
        Vector2 position = new Vector2(0, value);

        Debug.Log(position);

        Debug.Log(transform.localPosition);

        while (Vector2.Distance(transform.localPosition, position) > 0.001f)
        {
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, position, speed * Time.deltaTime);
            yield return null;
        }
    }
}
