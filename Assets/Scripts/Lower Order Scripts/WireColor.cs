using UnityEngine;

public class WireColor : MonoBehaviour
{
    [SerializeField]
    private Color btnColor;

    private Transform wire;

    private bool on_wire;

    public Color GetBtnColor()
    {
        return btnColor;
    }
}
