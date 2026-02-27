using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class FileSystem
{
    public const string FILENAME = "/replaydata.json";

    public static void SaveReplayFile()
    {
        string filePath = Application.persistentDataPath + FILENAME;
        string txt = JsonUtility.ToJson(FILENAME);
        File.WriteAllText(filePath, contents:FILENAME);
    }
}
