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

    public const string FILENAMESTART = "/matchdata-";
    public const string FILENAMEEND = ".json";

    public string fileNameTime;
    public DateTime time;

    public void SaveNewReplayFile()
    {
        time = DateTime.Now;

        fileNameTime = time.Day.ToString() + "-" + time.Month.ToString() + "-" + time.Year.ToString()
            + "-" + time.Hour.ToString() + "-" + time.Minute.ToString() + "-" + time.Second.ToString();

        MatchStartData matchStartData = new MatchStartData(fileNameTime);

        //DateTime startTime = DateTime.Now;

        //fileStartTime = "/" + 

        string filePath = Application.persistentDataPath + FILENAMESTART + fileNameTime + FILENAMEEND;
        string txt = JsonUtility.ToJson(matchStartData);
        File.WriteAllText(filePath, contents: txt);
    }

    public void AppendInputsToReplayFile(string inputs_str)
    {
        string filePath = Application.persistentDataPath + fileNameTime + FILENAMEEND;
        string txt = JsonUtility.ToJson(inputs_str);
        File.AppendAllText(filePath, contents: inputs_str);
    }

    public void AppendFighterData(Fighter fighter)
    {
        SavedFighterData fighterData = new SavedFighterData(fighter);

        string filePath = Application.persistentDataPath + fileNameTime + FILENAMEEND;
        string txt = JsonUtility.ToJson(fighterData);
        File.AppendAllText(filePath, contents: txt);
    }

    public void AppendMatchEndData(BattleCore battleCore)
    {
        MatchEndData matchEndData = new MatchEndData(battleCore);

        string filePath = Application.persistentDataPath + fileNameTime + FILENAMEEND;
        string txt = JsonUtility.ToJson(matchEndData);
        File.AppendAllText(filePath, contents: txt);
    }
}

    public class SavedFighterData
    {
        bool isPlayerOne;
        float positionX;
        int currentActionID;

        public SavedFighterData(Fighter fighter)
        {
            isPlayerOne = fighter.isFaceRight;
            positionX = fighter.position.x;
            currentActionID = fighter.currentActionID;
        }
        public void setFighterData(Fighter fighter)
        {
            isPlayerOne = fighter.isFaceRight;
            positionX = fighter.position.x;
            currentActionID = fighter.currentActionID;
        }
    }

    public class MatchStartData
    {
        public bool isVsCPU;
        public bool isCPUVsCPU;
        public string matchStartTime;

        public MatchStartData(string startTime) {
            isVsCPU = GameManager.Instance.isVsCPU;
            isCPUVsCPU = GameManager.Instance.isCPUVsCPU;
            matchStartTime = startTime;

        }
    }

    public class MatchEndData
    {
        bool isPlayerOneWinner;
        int playerOneWinCount;
        int playerTwoWinCount;
        public MatchEndData(BattleCore battleCore)
        {
            playerOneWinCount = (int)battleCore.fighter1RoundWon;
            playerTwoWinCount = (int)battleCore.fighter2RoundWon;
        }

    }
