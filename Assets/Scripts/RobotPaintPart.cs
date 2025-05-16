using UnityEngine;

public class RobotPaintPart : MonoBehaviour
{
    private int stickerCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stickerCount = 0;
    }

    public void IncSticker()
    {
        stickerCount++;
        Debug.Log(stickerCount);
    }

    public void DecSticker()
    {
        stickerCount--;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Sticker sticker))
        {
            IncSticker();
            sticker.ToggleIsDuplicate();
            sticker.ToggleIsOnPart();
            Debug.Log("Sticker stuck");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Sticker sticker))
        {
            DecSticker();
            sticker.ToggleIsDuplicate();
            sticker.ToggleIsOnPart();
            Debug.Log("Sticker stuck");
        }
    }
}
