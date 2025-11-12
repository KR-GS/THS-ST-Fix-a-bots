using System.Collections.Generic;
using UnityEngine;

public class HODifficultyManager : MonoBehaviour
{
    public void DifficultyPicker(int difficultyLevel)
    {
        switch (difficultyLevel)
        {
            case 1:
                StaticData.maxNumber[StaticData.stageNum] = Random.Range(14, 20); // Easy: max number between 14 and 15
                StaticData.constant[StaticData.stageNum] = Random.Range(1, 6); // Easy: constant between 1 and 5
                StaticData.coefficient[StaticData.stageNum] = Random.Range(2, 4); // Easy: coefficient between 2 and 3
                StaticData.prePressedCount[StaticData.stageNum] = 3;
                StaticData.isFormulaSeen[StaticData.stageNum] = false;
                StaticData.isRandomSequence[StaticData.stageNum] = false;
                StaticData.lockCoefficient[StaticData.stageNum] = false;
                StaticData.lockConstant[StaticData.stageNum] = false;
                StaticData.hintSeen[StaticData.stageNum] = true;
                StaticData.stageRandomCoefficientCount[StaticData.stageNum] = 1;
                StaticData.stageRandomConstantCount[StaticData.stageNum] = 1;
                StaticData.stageMaxCoefficientValue[StaticData.stageNum] = 4;
                StaticData.stageMaxConstantValue[StaticData.stageNum] = 8;
                break;
            case 2:
                StaticData.maxNumber[StaticData.stageNum] = Random.Range(18, 23); // Easy: max number between 18 and 22
                StaticData.constant[StaticData.stageNum] = Random.Range(-5, 25); // Easy: constant between -4 and 24
                StaticData.coefficient[StaticData.stageNum] = Random.Range(3, 6); // Easy: coefficient between 3 and 4
                StaticData.prePressedCount[StaticData.stageNum] = 2;
                StaticData.isFormulaSeen[StaticData.stageNum] = false;
                StaticData.isRandomSequence[StaticData.stageNum] = false;
                StaticData.lockCoefficient[StaticData.stageNum] = false;
                StaticData.lockConstant[StaticData.stageNum] = false;
                StaticData.hintSeen[StaticData.stageNum] = false;
                StaticData.stageRandomCoefficientCount[StaticData.stageNum] = 3;
                StaticData.stageRandomConstantCount[StaticData.stageNum] = 3;
                StaticData.stageMaxCoefficientValue[StaticData.stageNum] = 6;
                StaticData.stageMaxConstantValue[StaticData.stageNum] = 29;
                break;
            case 3:
                StaticData.maxNumber[StaticData.stageNum] = Random.Range(18, 26); // Easy: max number between 18 and 25
                StaticData.constant[StaticData.stageNum] = Random.Range(-20, 50); // Easy: constant between -4 and 49
                StaticData.coefficient[StaticData.stageNum] = Random.Range(2, 7); // Easy: coefficient between 3 and 4
                StaticData.prePressedCount[StaticData.stageNum] = 0;
                StaticData.isFormulaSeen[StaticData.stageNum] = true;
                StaticData.isRandomSequence[StaticData.stageNum] = false;
                StaticData.lockCoefficient[StaticData.stageNum] = false;
                StaticData.lockConstant[StaticData.stageNum] = false;
                StaticData.hintSeen[StaticData.stageNum] = false;
                StaticData.stageRandomCoefficientCount[StaticData.stageNum] = 4;
                StaticData.stageRandomConstantCount[StaticData.stageNum] = 4;
                StaticData.stageMaxCoefficientValue[StaticData.stageNum] = 6;
                StaticData.stageMaxConstantValue[StaticData.stageNum] = 29;
                break;
            default:
                Debug.LogWarning("Invalid difficulty level. Defaulting to Easy.");
                DifficultyPicker(1);
                break;
        }
    }
    

}