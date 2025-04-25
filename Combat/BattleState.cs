using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleState", menuName = "ScriptableObjects/DataContainers/SavedData/BattleState", order = 1)]
public class BattleState : SavedData
{
    public string delimiterTwo = "|";
    public SceneTracker sceneTracker;
    public CharacterList enemyList;
    public string previousScene;
    public void UpdatePreviousScene(){previousScene = sceneTracker.GetPreviousScene();}
    public void SetPreviousScene(string sceneName){previousScene = sceneName;}
    public List<string> enemies;
    public void AddEnemyName(string newName){enemies.Add(newName);}
    public void SetEnemyNames(List<string> newEnemies){enemies = new List<string>(newEnemies);}
    public void UpdateEnemyNames(){enemies = new List<string>(enemyList.characters);}
    public List<string> tiles;

    public override void NewGame()
    {
        base.NewGame();
    }

    public override void Save()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = previousScene+delimiter;
        for (int i = 0; i < enemies.Count; i++)
        {
            allData += enemies[i];
            if (i < enemies.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        for (int i = 0; i < tiles.Count; i++)
        {
            allData += tiles[i];
            if (i < tiles.Count - 1){allData += delimiterTwo;}
        }
        File.WriteAllText(dataPath, allData);
    }

    public override void Load()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = File.ReadAllText(dataPath);
        dataList = allData.Split(delimiter).ToList();
        previousScene = dataList[0];
        enemies = dataList[1].Split(delimiterTwo).ToList();
        tiles = dataList[2].Split(delimiterTwo).ToList();
        sceneTracker.SetPreviousScene(previousScene);
        enemyList.ResetLists();
        enemyList.AddCharacters(enemies);
    }
}
