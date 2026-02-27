using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class FileSystem : MonoBehaviour
{
    public const string FILENAME = "/replaydata.json";
    string recorded_inputs = "test";

    public static void SaveNewReplayFile(string inputs_str)
    {
        string filePath = Application.persistentDataPath + FILENAME;
        string txt = JsonUtility.ToJson(inputs_str);
        File.WriteAllText(filePath, contents:inputs_str);
    }

    public static void AppendToReplayFile(string inputs_str)
    {
        string filePath = Application.persistentDataPath + FILENAME;
        string txt = JsonUtility.ToJson(inputs_str);
        File.AppendAllText(filePath, contents: inputs_str);
    }
}
