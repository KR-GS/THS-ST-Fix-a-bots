using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndScreenAnimator : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation()
    {
        animator.SetTrigger("Play");
    }
}