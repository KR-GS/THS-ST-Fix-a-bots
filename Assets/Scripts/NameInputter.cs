using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;



public class NameInputter : MonoBehaviour , IDataPersistence
{
    public GameObject nameInputter;
    public TMP_InputField nameField;


    private void Awake()
    {
        if (nameInputter != null)
        {

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
            StaticData.lo_name = nameField.text;
        }
        else if (SceneManager.GetActiveScene().name == "Stage_Select")
        {
            StaticData.ho_name = nameField.text;
        }

        DataPersistenceManager.Instance.SaveGame();

        nameInputter.SetActive(false);
    }

        public void LoadData(GameData data)
    {

    }

    //TODO: Add things to be saved here
    public void SaveData(ref GameData data)
    {
        if(SceneManager.GetActiveScene().name == "LO_WS2D")
        {
            data.lo_name = StaticData.lo_name;
        }
        else if (SceneManager.GetActiveScene().name == "Stage_Select")
        {
            data.ho_name = StaticData.ho_name;
        }
    }
    
}
