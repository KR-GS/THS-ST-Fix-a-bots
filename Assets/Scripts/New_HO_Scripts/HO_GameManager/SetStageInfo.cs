using UnityEngine;

public class SetStageInfo: MonoBehaviour, IDataPersistence
{
    public void LoadData(GameData data)
    {
        StaticData.stageLives = new System.Collections.Generic.List<int>(data.lives);
        StaticData.stageRestarts = new System.Collections.Generic.List<int>(data.restarts);
        StaticData.stageTime = new System.Collections.Generic.List<float>(data.stageTimes);
        StaticData.formulaAttempts = new System.Collections.Generic.List<string>(data.formulaAttempts);
        StaticData.stageStars = new System.Collections.Generic.List<int>(data.stageStars);
        StaticData.numStageDone = data.stageDone;

        Debug.Log("[StageDataLoader] Data loaded into StaticData");
    }

    public void SaveData(ref GameData data)
    {
        StaticData.EnsureStageListSizes();

        data.lives = new System.Collections.Generic.List<int>(StaticData.stageLives);
        data.restarts = new System.Collections.Generic.List<int>(StaticData.stageRestarts);
        data.stageTimes = new System.Collections.Generic.List<float>(StaticData.stageTime);
        data.formulaAttempts = new System.Collections.Generic.List<string>(StaticData.formulaAttempts);
        data.stageStars = new System.Collections.Generic.List<int>(StaticData.stageStars);
        data.stageDone = StaticData.numStageDone;

        Debug.Log("[StageDataLoader] Data saved from StaticData");
    }

    private void Start()
    {
        if (DataPersistenceManager.Instance != null)
        {
            DataPersistenceManager.Instance.RegisterDataPersistence(this);
            Debug.Log("Start is called!");
        }
        else
        {
            Debug.LogError("[SetStageInfo] DataPersistenceManager.Instance is null during Start!");
        }
    }
}
