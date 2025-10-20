using UnityEngine;
using UnityEngine.TextCore.Text;
using TMPro;

public class RulerLines : MonoBehaviour
{
    [SerializeField]
    private GameObject selected;

    [SerializeField]
    private TMP_FontAsset selected_Font;

    [SerializeField]
    private TextMeshProUGUI ruler_Text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHighLight()
    {
        selected.SetActive(true);

        ruler_Text.font = selected_Font;

        GetComponent<BoxCollider2D>().enabled = false;
    }
}
