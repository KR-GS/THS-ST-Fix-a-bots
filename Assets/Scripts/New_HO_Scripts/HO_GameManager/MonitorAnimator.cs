using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MonitorAnimator : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("Play");
    }
}