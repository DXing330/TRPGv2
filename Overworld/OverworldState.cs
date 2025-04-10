using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OverworldState", menuName = "ScriptableObjects/DataContainers/SavedData/OverworldState", order = 1)]
public class OverworldState : SavedData
{
    // Need to get the location of the guild hub when starting a new game.
    public SavedOverworld savedOverworld;
    public SavedCaravan caravan;
    public int location;
    public int GetLocation(){return location;}
    public void SetLocation(int newLocation)
    {
        location = newLocation;
        Save();
    }
    public int moves;
    public int GetMoves(){return moves;}
    public void SetMoves(int newMoves){moves = newMoves;}
    public bool EnoughMovement(int moveCost){return moves >= moveCost;}
    public void SpendMovement(int moveCost)
    {
        moves -= moveCost;
    }
    public int dayCount;
    public int GetDay(){return dayCount;}
    public void SetDay(int newDate){dayCount = newDate;}
    public void NewDay()
    {
        dayCount++;
        ResetMoves();
        Save();
    }
    public void ResetMoves()
    {
        moves = caravan.ReturnDailyTravelableDistance();
    }
    public override void NewGame()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        //allData = newGameData;
        //File.WriteAllText(dataPath, allData);
        location = savedOverworld.GetCenterCityLocation();
        moves = 0;
        dayCount = 0;
        Save();
        Load();
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
