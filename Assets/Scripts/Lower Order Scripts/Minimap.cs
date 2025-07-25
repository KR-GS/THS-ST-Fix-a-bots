using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    [SerializeField]
    private int mapValue =0 ;

    [SerializeField]
    private Button btn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetValue(int val)
    {
        mapValue = val;
        Debug.Log(mapValue);
        
    }

    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(() => { FindAnyObjectByType<PaintMinimapManager>().ChangeSelectedSide(mapValue); FindAnyObjectByType<LoPaintMinigame>().ChangeSide(mapValue); });
    }

    void Update()
    {
        //GetComponent<Button>().onClick.AddListener(delegate { transform.parent.GetComponent<PaintMinimapManager>().ChangeSelectedSide(mapValue); });
    }
}
