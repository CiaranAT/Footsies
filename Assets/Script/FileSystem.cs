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

    public void StartNewMatchDataFile()
    {
        matchSaveData.clearData();

        time = DateTime.Now;

        fileNameTime = time.Day.ToString() + "-" + time.Month.ToString() + "-" + time.Year.ToString()
            + "-" + time.Hour.ToString() + "-" + time.Minute.ToString() + "-" + time.Second.ToString();

        matchSaveData.matchStartData = new MatchStartData(fileNameTime);
    }

    public void SaveMatchDataFile(BattleCore battleCore)
    {
        matchSaveData.matchEndData = new MatchEndData((int)battleCore.fighter1RoundWon, (int)battleCore.fighter2RoundWon);

        string filePath = getFilePath();
        string txt = JsonUtility.ToJson(matchSaveData, true);
        File.WriteAllText(filePath, contents: txt);

        matchSaveData.clearData();
    }
    public void StoreFighterData(Fighter fighter1, Fighter fighter2)
    {
        FighterRecordData fighter1Data = new FighterRecordData(fighter1);
        FighterRecordData fighter2Data = new FighterRecordData(fighter2);

        matchSaveData.fighter1DataList.Add(fighter1Data);
        matchSaveData.fighter2DataList.Add(fighter2Data);
    }
}

[Serializable]
public class FighterRecordData
{
    public float posX;
    public int action;

    public FighterRecordData(Fighter fighter)
    {
        posX = fighter.position.x;
        action = fighter.currentActionID;
    }
}

[Serializable]
public class MatchStartData
{
    public int gameModeID;
    public string matchStartTime;

    public MatchStartData(string startTime) {
        gameModeID = (int)GameManager.Instance.gameMode;
        matchStartTime = startTime;
    }
}

[Serializable]
public class MatchEndData
{
    public int p1Wins;
    public int p2Wins;
    public MatchEndData(int p1_wins, int p2_wins)
    {
        p1Wins = p1_wins;
        p2Wins = p2_wins;
    }

}

[Serializable]
public class MatchSaveData
{
    public MatchStartData matchStartData;
    public List<FighterRecordData> fighter1DataList;
    public List<FighterRecordData> fighter2DataList;
    public MatchEndData matchEndData;

    public void clearData()
    {
        matchStartData = null;
        fighter1DataList.Clear();
        fighter2DataList.Clear();
        matchEndData = null;
    }
}

