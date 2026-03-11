using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Footsies;

public class FileSystem : MonoBehaviour
{
    public static FileSystem Instance;

    private void Awake()
    {
        CreateSingleton();
    }

    void CreateSingleton()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public const string FILENAME_START = "/matchdata-";
    public const string FILENAME_END = ".json";

    public string fileNameTime;
    public DateTime time;

    public MatchSaveData matchSaveData;

    string getFilePath()
    {
        return Application.persistentDataPath + FILENAME_START + fileNameTime + FILENAME_END;
    }

    public void SaveNewReplayFile()
    {
        matchSaveData.fighterDataList.Clear();

        time = DateTime.Now;

        fileNameTime = time.Day.ToString() + "-" + time.Month.ToString() + "-" + time.Year.ToString()
            + "-" + time.Hour.ToString() + "-" + time.Minute.ToString() + "-" + time.Second.ToString();

        MatchStartData matchStartData = new MatchStartData(fileNameTime);

        string filePath = getFilePath();
        string txt = JsonUtility.ToJson(matchStartData, true);
        File.WriteAllText(filePath, contents: txt);
    }

    public void DumpFighterData()
    {
        string filePath = getFilePath();
        string txt = JsonUtility.ToJson(matchSaveData, true);
        File.AppendAllText(filePath, contents: txt);

        matchSaveData.fighterDataList.Clear();
    }

    public void AppendMatchEndData(BattleCore battleCore)
    {
        DumpFighterData();

        MatchEndData matchEndData = new MatchEndData(battleCore);

        string filePath = getFilePath();
        string txt = JsonUtility.ToJson(matchEndData, true);
        File.AppendAllText(filePath, contents: txt);
    }
    public void StoreFighterData(BattleCore battleCore)
    {
        FighterRecordData fighter1Data = new FighterRecordData(battleCore.fighter1);
        FighterRecordData fighter2Data = new FighterRecordData(battleCore.fighter2);

        matchSaveData.fighterDataList.Add(fighter1Data);
        matchSaveData.fighterDataList.Add(fighter2Data);
    }
}

[Serializable]
public struct MatchSetupData
{
    public bool isVsCPU;
    public bool isCPUVsCPU;
    public string matchStartTime;
}
[Serializable]
public struct MatchResultData
{
    public int p1Wins;
    public int p2Wins;
}

[Serializable]
public class FighterRecordData
{
    public bool isP1;
    public float posX;
    public int action;

    public FighterRecordData(Fighter fighter)
    {
        isP1 = fighter.isFaceRight;
        posX = fighter.position.x;
        action = fighter.currentActionID;
    }
}

[Serializable]
public class MatchStartData
{
    public MatchSetupData setupData;

    public MatchStartData(string startTime) {
        setupData.isVsCPU = GameManager.Instance.isVsCPU;
        setupData.isCPUVsCPU = GameManager.Instance.isCPUVsCPU;
        setupData.matchStartTime = startTime;
    }
}

[Serializable]
public class MatchEndData
{
    public MatchResultData resultData;
    public MatchEndData(BattleCore battleCore)
    {
        resultData.p1Wins = (int)battleCore.fighter1RoundWon;
        resultData.p2Wins = (int)battleCore.fighter2RoundWon;
    }

}

[Serializable]
public class MatchSaveData
{
    public List<FighterRecordData> fighterDataList;
}

