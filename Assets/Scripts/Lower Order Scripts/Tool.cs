using System.Collections;
using UnityEngine;

public class Tool : MonoBehaviour
{
    private Animator statusAnimator;

    void Start()
    {
        statusAnimator = GetComponentInChildren<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public IEnumerator TriggerToolAnimation()
    {
        GetComponent<Collider2D>().enabled = false;
        statusAnimator.SetTrigger("IsHitting");
        yield return new WaitForSeconds(1);
        GetComponent<Collider2D>().enabled = true;
    }

}
