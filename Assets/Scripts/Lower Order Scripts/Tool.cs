using System.Collections;
using UnityEngine;

public class Tool : MonoBehaviour
{
    private Animator statusAnimator;
    private AnimationClip[] clips;

    void Start()
    {
        statusAnimator = GetComponentInChildren<Animator>();
        clips = statusAnimator.runtimeAnimatorController.animationClips;
        Debug.Log(clips[1].length);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public IEnumerator TriggerToolAnimation(PartTile fastener, float posY)
    {
        GetComponent<Collider2D>().enabled = false;
        statusAnimator.SetTrigger("IsUsed");
        yield return new WaitForSeconds(clips[1].length +0.5f);
        fastener.SetFastenerPosition(posY);
        GetComponent<Collider2D>().enabled = true;
    }

}
