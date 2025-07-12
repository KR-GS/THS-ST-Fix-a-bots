using System.Collections;
using UnityEngine;

public class Tool : MonoBehaviour
{
    private Animator statusAnimator;
    private AnimationClip[] clips;
    private AnimationEvent[] events;
    
    void Awake()
    {
        statusAnimator = GetComponentInChildren<Animator>();
        clips = statusAnimator.runtimeAnimatorController.animationClips;
        Debug.Log(clips.Length);
        events = clips[1].events;

        Debug.Log("Event time: " + events[0].time);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public IEnumerator TriggerToolAnimation(PartTile fastener)
    {
        GetComponentInChildren<ToolEvent>().SetCurrentFastener(fastener);
        GetComponent<Collider2D>().enabled = false;
        statusAnimator.SetTrigger("IsUsed");
        yield return new WaitForSeconds(0.5f);
        GetComponent<Collider2D>().enabled = true;
    }

    public void SetHeightValue(float value)
    {
        GetComponentInChildren<ToolEvent>().SetHeightToSet(value);
    }


}
