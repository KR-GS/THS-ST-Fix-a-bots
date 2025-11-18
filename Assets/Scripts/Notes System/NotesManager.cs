using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class NotesManager : MonoBehaviour
{
    [SerializeField]
    private GameObject notePrefab;

    [SerializeField]
    private Vector3 notesSize;

    [SerializeField]
    private Canvas notesMakerUI;

    [SerializeField]
    private Canvas notesUI;

    [SerializeField]
    private GameObject notesObj;

    private bool isDragging = false;

    private GameObject noteToDrag;

    private Color default_color;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Update()
    {
        if(Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (!isDragging)
                {
                    HandleClickEvent();
                }
            }
            else if(Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if (isDragging)
                {
                    isDragging = false;
                }
            }
            else
            {
                if (isDragging)
                {
                    Vector2 touchPos = Input.GetTouch(0).position;

                    noteToDrag.transform.position = new Vector2(touchPos.x, touchPos.y);
                }
            }
        }
    }

    private void HandleClickEvent() 
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.GetTouch(0).position;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        if (raycastResults.Count > 0)
        {
            foreach (var go in raycastResults)
            {
                if (go.gameObject.TryGetComponent(out Note note))
                {
                    isDragging = true;
                    noteToDrag = go.gameObject;
                }
            }
        }
    }

    public void OpenNoteSystem()
    {
        float cameraSize = Camera.main.orthographicSize;
        Vector3 newPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
        notesMakerUI.gameObject.SetActive(true);
        default_color = notesObj.GetComponent<Image>().color;
    }

    public void CloseNoteSystem()
    {
        notesMakerUI.gameObject.SetActive(false);
    }

    public void OnDoneBtnClick()
    {
        GameObject noteObject;
        notesObj.GetComponentInChildren<TextMeshProUGUI>().text = notesObj.GetComponentInChildren<TMP_InputField>().text;
        notesObj.GetComponentInChildren<TMP_InputField>().text = "00";

        noteObject = Instantiate(notesObj, notesUI.transform);

        noteObject.transform.localScale = notesSize;

        noteObject.AddComponent<Note>();

        noteObject.GetComponentInChildren<TMP_InputField>().gameObject.SetActive(false);

        notesObj.GetComponentInChildren<TextMeshProUGUI>().text = "";

        notesObj.GetComponent<Image>().color = default_color;

        notesMakerUI.gameObject.SetActive(false);
    }

    public void ToggleNotes()
    {
        notesUI.enabled = !notesUI.enabled;
    }

    public void ChangeColor(Image btnColor)
    {
        notesObj.GetComponent<Image>().color = btnColor.color;
    }
}
