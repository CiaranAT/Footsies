using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Footsies;

public class FileSystem : MonoBehaviour
{
    public const string FILENAME = "/matchdata.json";

    string fileStartTime;

    public void SaveNewReplayFile()
    {
        MatchStartData matchStartData = new MatchStartData();

        fileStartTime = matchStartData.fileStartTime;

        string filePath = Application.persistentDataPath + FILENAME;
        string txt = JsonUtility.ToJson(matchStartData);
        File.WriteAllText(filePath, contents: txt);
    }

    public void AppendInputsToReplayFile(string inputs_str)
    {
        string filePath = Application.persistentDataPath + fileStartTime + FILENAME;
        string txt = JsonUtility.ToJson(inputs_str);
        File.AppendAllText(filePath, contents: inputs_str);
    }

    public void AppendFighterData(Fighter fighter)
    {
        SavedFighterData fighterData = new SavedFighterData(fighter);

        string filePath = Application.persistentDataPath + fileStartTime + FILENAME;
        string txt = JsonUtility.ToJson(fighterData);
        File.AppendAllText(filePath, contents: txt);
    }

    public void AppendMatchEndData(BattleCore battleCore)
    {
        MatchEndData matchEndData = new MatchEndData(battleCore);

        string filePath = Application.persistentDataPath + fileStartTime + FILENAME;
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
        bool isVsCPU;
        bool isCPUVsCPU;
        public string fileStartTime;

        public MatchStartData() {
            isVsCPU = GameManager.Instance.isVsCPU;
            isCPUVsCPU = GameManager.Instance.isCPUVsCPU;
            DateTime startTime = DateTime.Now;
            fileStartTime = startTime.ToString();
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
