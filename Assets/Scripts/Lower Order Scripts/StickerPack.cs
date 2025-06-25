using UnityEngine;

public class StickerPack : MonoBehaviour
{
    [SerializeField]
    protected string stickerPackType;

    [SerializeField]
    private Sticker[] stickers;

    public string GetPackType()
    {
        return stickerPackType;
    }

    public Sticker[] GetPackContents()
    {
        return stickers;
    }
}
