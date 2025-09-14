using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StSSettings", menuName = "ScriptableObjects/StS/StSSettings", order = 1)]
public class StSSettings : SavedData
{
    public int difficultySetting;
    public void SetDifficultly(int newInfo)
    {
        difficultySetting = newInfo;
        Save();
    }
    public override void Save()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        allData = difficultySetting.ToString();
        File.WriteAllText(dataPath, allData);
    }
    public override void Load()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        if (File.Exists(dataPath))
        {
            allData = File.ReadAllText(dataPath);
        }
        else
        {
            NewGame();
            return;
        }
        string[] blocks = allData.Split(delimiter);
        difficultySetting = int.Parse(blocks[0]);
    }
    public override void NewGame()
    {
        difficultySetting = 0;
        Save();
    }
}