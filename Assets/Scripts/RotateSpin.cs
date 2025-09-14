using UnityEngine;

public class RotateSpin : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float rotateSpeed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles -= new Vector3(0, 0, rotateSpeed * Time.deltaTime); //rotate clockwise
    }
}
