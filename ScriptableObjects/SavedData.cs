using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SavedData", menuName = "ScriptableObjects/SavedData", order = 1)]
public class SavedData : ScriptableObject
{
    protected string dataPath;
    public string filename;
    public string newGameData;
    public string allData;
    public List<string> dataList;
    public string delimiter;

    public virtual void Save()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = "";
        for (int i = 0; i < dataList.Count; i++)
        {
            allData += dataList[i];
            if (i < dataList.Count - 1){allData += delimiter;}
        }
        File.WriteAllText(dataPath, allData);
    }

    public virtual void Load()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        if (File.Exists(dataPath)){allData = File.ReadAllText(dataPath);}
        else{allData = newGameData;}
        if (allData.Contains(delimiter)){dataList = allData.Split(delimiter).ToList();}
        else
        {
            dataList.Clear();
            dataList.Add(allData);
        }
    }
}
