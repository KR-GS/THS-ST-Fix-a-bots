using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;



public class NameInputter : MonoBehaviour , IDataPersistence
{
    public GameObject nameInputter;
    public TMP_InputField firstnameField;
    public TMP_InputField lastnameField;

    [HideInInspector] public bool isNameFinished = false;

    private void Awake()
    {
        if (nameInputter != null)
        {
            isNameFinished = false;
            nameInputter.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Name Inputting Object not assigned!");
        }
        
    }

    public void AskName()
    {
        nameInputter.SetActive(true);
    }

    public void FinishName()
    {
        if(SceneManager.GetActiveScene().name == "LO_WS2D")
        {
            if (!string.IsNullOrWhiteSpace(firstnameField.text) && !string.IsNullOrWhiteSpace(lastnameField.text))
            {
                StaticData.lo_firstname = firstnameField.text;
                StaticData.lo_lastname = lastnameField.text;
                DataPersistenceManager.Instance.SaveGame();
                nameInputter.SetActive(false);
                isNameFinished = true;
            }
            else
            {
                Debug.Log("Both entries should have names");
            }
        }
        else if (SceneManager.GetActiveScene().name == "Stage_Select")
        {
            if (!string.IsNullOrWhiteSpace(firstnameField.text) && !string.IsNullOrWhiteSpace(lastnameField.text))
            {
                StaticData.ho_firstname = firstnameField.text;
                StaticData.ho_lastname = lastnameField.text;
                DataPersistenceManager.Instance.SaveGame();
                nameInputter.SetActive(false);
                isNameFinished = true;
            }
            else
            {
                Debug.Log("Both entries should have names");
            }
        }

    }

        public void LoadData(GameData data)
    {

    }

    //TODO: Add things to be saved here
    public void SaveData(ref GameData data)
    {
        if(SceneManager.GetActiveScene().name == "LO_WS2D")
        {
            data.lo_firstname = StaticData.lo_firstname;
            data.lo_lastname = StaticData.lo_lastname;
        }
        else if (SceneManager.GetActiveScene().name == "Stage_Select")
        {
            data.ho_firstname = StaticData.ho_firstname;
            data.ho_lastname = StaticData.ho_lastname;
        }
    }
    
}
