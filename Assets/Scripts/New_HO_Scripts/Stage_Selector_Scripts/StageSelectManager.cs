using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour
{
    public Button stage1, stage2, stage3, stage4, stage5, rand;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stage1.onClick.AddListener(() => LoadStage(25, 1f, 0.6f, 3, true, true, true, 2, 2, false));
        stage2.onClick.AddListener(() => LoadStage(25, 1f, 0.6f, 3, false, false, true, 2, -3, false));
        stage3.onClick.AddListener(() => LoadStage(25, 1f, 0.6f, 3, false, false, false, 3, -1, false));
        stage4.onClick.AddListener(() => LoadStage(25, 1f, 0.6f, 0, true, true, true, 3, 1, false));
        stage5.onClick.AddListener(() => LoadStage(25, 1f, 0.6f, 0, false, false, false, 4, -3, false));
        stage5.onClick.AddListener(() => LoadStage(25, 1f, 0.6f, 0, false, false, false, 4, -3, false));
        rand.onClick.AddListener(() => LoadStage(25, 1f, 0.6f, 0, false, false, false, 0, 0, true));
    }


    public void LoadStage(int max, float cycInt, float cycLen, int prePressed, bool formSeen, bool lockCoef,
    bool lockConst, int coef, int constant, bool randSeq)
    {
        StaticData.maxNumber = max;
        StaticData.cycleInterval = cycInt;
        StaticData.cycleLeniency = cycLen;
        StaticData.prePressedCount = prePressed;
        StaticData.isFormulaSeen = formSeen;
        StaticData.lockCoefficient = lockConst;
        StaticData.lockConstant = lockCoef;
        StaticData.coefficient = coef;
        StaticData.constant = constant;
        StaticData.isRandomSequence = randSeq;
        SceneManager.LoadScene("HO_Scene"); 
    }
}
