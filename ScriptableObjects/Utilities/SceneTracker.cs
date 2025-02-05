using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneTracker", menuName = "ScriptableObjects/DataContainers/SavedData/SceneTracker", order = 1)]
public class SceneTracker : SavedData
{
    public List<string> sceneNames;
    public string previousScene;
    public void SetPreviousScene(string sceneName)
    {
        previousScene = sceneName;
    }
    public string GetPreviousScene(){return previousScene;}
    public string defaultScene;
    public void SetDefaultScene(string sceneName){defaultScene = sceneName;}
    public string GetDefaultScene(){return defaultScene;}

    public override void Save()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = previousScene+delimiter+defaultScene;
        File.WriteAllText(dataPath, allData);
    }

    public override void Load()
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
        SetPreviousScene(dataList[0]);
        SetDefaultScene(dataList[1]);
    }
}
