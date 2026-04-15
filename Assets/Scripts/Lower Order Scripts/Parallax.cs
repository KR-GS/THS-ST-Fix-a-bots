using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private bool isMoving = false;

    [SerializeField]
    private Transform bg;

    private Vector2 originalposition;

    private Vector2 defaultBGPos;
    // Update is called once per frame

    void Start()
    {
        originalposition = bg.position;
        defaultBGPos = bg.localPosition;
        Debug.Log("Camera local position: " + transform.localPosition);
    }

    void Update()
    {
        
    }

    public void SetIsMoving(bool status)
    {
        isMoving = status;

    }

    public void SetDefaultPosition()
    {
        bg.localPosition = new Vector3(0, defaultBGPos.y, 50f);
    }

    public void SetZoomPosition()
    {
        bg.position = new Vector3(0, originalposition.y, 50f);

        bg.localPosition = new Vector3(0, bg.localPosition.y, 50f);
    }
}
