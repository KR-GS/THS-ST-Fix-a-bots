using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[System.Serializable]
public class Tutorial
{
    public Sprite patt_sprite;

    [TextArea(3, 10)]
    public string description;

    public Texture2D display;

    public VideoClip display_video;
}
