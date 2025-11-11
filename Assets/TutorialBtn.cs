using UnityEngine;

public class TutorialBtn : MonoBehaviour
{
    [SerializeField]
    private float secondsPass;

    [SerializeField]
    private float timeLimit = 20f;

    void Start()
    {
        secondsPass = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(secondsPass >= timeLimit)
        {
            GetComponent<Animator>().SetTrigger("Idle");
            secondsPass = 0f;
        }
        else
        {
            secondsPass += Time.deltaTime;
        }
    }
}
