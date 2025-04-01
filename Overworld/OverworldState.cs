using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OverworldState", menuName = "ScriptableObjects/DataContainers/SavedData/OverworldState", order = 1)]
public class OverworldState : SavedData
{
    public SavedCaravan caravan;
    public int location;
    public int GetLocation(){return location;}
    public void SetLocation(int newLocation){location = newLocation;}
    public int moves;
    public int GetMoves(){return moves;}
    public void SetMoves(int newMoves){moves = newMoves;}
    public int dayCount;
    public int GetDay(){return dayCount;}
    public void SetDay(int newDate){dayCount = newDate;}
    public void NewDay()
    {
        dayCount++;
        ResetMoves();
    }
    public void ResetMoves()
    {
        moves = caravan.ReturnDailyTravelableDistance();
    }
    public override void NewGame()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = newGameData;
        File.WriteAllText(dataPath, allData);
        Load();
        Save();
    }
    public override void Save()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = location+delimiter+moves+delimiter+dayCount;
        File.WriteAllText(dataPath, allData);
    }
    public override void Load()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        if (File.Exists(dataPath)){allData = File.ReadAllText(dataPath);}
        else
        {
            NewGame();
            return;
        }
        dataList = allData.Split(delimiter).ToList();
        location = int.Parse(dataList[0]);
        moves = int.Parse(dataList[1]);
        dayCount = int.Parse(dataList[1]);
    }
}
