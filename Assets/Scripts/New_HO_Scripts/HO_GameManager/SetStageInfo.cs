using UnityEngine;

public class SetStageInfo: MonoBehaviour, IDataPersistence
{
    public void LoadData(GameData data)
    {
        StaticData.stageLives = new System.Collections.Generic.List<int>(data.lives);
        StaticData.stageRestarts = new System.Collections.Generic.List<int>(data.restarts);
        StaticData.stageTime = new System.Collections.Generic.List<float>(data.stageTimes);
        StaticData.numStageDone = data.stageDone;

        Debug.Log("[StageDataLoader] Data loaded into StaticData");
    }

    public void SaveData(ref GameData data)
    {
        StaticData.EnsureStageListSizes();

        data.lives = new System.Collections.Generic.List<int>(StaticData.stageLives);
        data.restarts = new System.Collections.Generic.List<int>(StaticData.stageRestarts);
        data.stageTimes = new System.Collections.Generic.List<float>(StaticData.stageTime);
        data.stageDone = StaticData.numStageDone;

        Debug.Log("[StageDataLoader] Data saved from StaticData");
    }

    private void Awake()
    {
        DataPersistenceManager.Instance.RegisterDataPersistence(this);
    }
}
