using UnityEngine;

public class WireColor : MonoBehaviour
{
    private enum ButtonColor
    {
        red,
        blue,
        yellow
    }

    [SerializeField]
    private ButtonColor colorAssigned;

    [SerializeField]
    private Color btnColor;

    void Awake()
    {
        if(colorAssigned == ButtonColor.red)
        {
            btnColor = Color.red;
        }
        else if(colorAssigned == ButtonColor.blue)
        {
            btnColor = Color.blue;
        }
        else
        {
            btnColor = Color.yellow;
        }
    }

    public Color GetBtnColor()
    {
        return btnColor;
    }
}
