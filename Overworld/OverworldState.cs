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
    public PartyDataManager partyData;
    // Bandits/Enemies will appear as the days go by.
    public List<OverworldEnemyManager> enemyManagers;
    // Enemies will be added to the enemy list before battle.
    public CharacterList enemyList;
    public int hoursInDay = 24;
    public int restingPeriod = 8;
    public int GetRestingPeriod() { return restingPeriod; }
    public int location;
    public string GetLocationTerrain(){return savedOverworld.ReturnTerrain(location);}
    public int GetLocation() { return location; }
    public bool SetLocation(int newLocation)
    {
        location = newLocation;
        // Check if there are any enemies;
        bool enemies = false;
        List<int> enemyManagerIndices = new List<int>();
        for (int i = 0; i < enemyManagers.Count; i++)
        {
            if (enemyManagers[i].EnemiesOnTile(location))
            {
                enemyManagerIndices.Add(i);
                enemies = true;
            }
        }
        if (enemies)
        {
            // Load all enemies;
            enemyList.ResetLists();
            for (int i = 0; i < enemyManagerIndices.Count; i++)
            {
                enemyList.AddCharacters(enemyManagers[enemyManagerIndices[i]].GetCurrentEnemies());
            }
            return true;
        }
        else
        {
            Save();
            return false;
        }
    }
    public int dayCount;
    public int GetDay() { return dayCount; }
    public void SetDay(int newDate) { dayCount = newDate; }
    public int currentHour;
    public void SetHour(int newHour) { currentHour = newHour; }
    public int GetHour() { return currentHour % hoursInDay; }
    // Stored here for convenience.
    public string battleType; // Quest/Feature/Event/"";
    public void ResetBattleType(){ battleType = ""; }
    public void EnterBattleFromFeature() { SetBattleType("Feature"); }
    public void EnterBattleFromQuest(){ SetBattleType("Quest"); }
    public void SetBattleType(string newType) { battleType = newType; }
    public string GetBattleType() { return battleType; }
    public override void AddHours(int newHours)
    {
        currentHour += newHours;
        if (currentHour < hoursInDay) { return; }
        dayCount++;
        currentHour -= hoursInDay;
        NewDay(dayCount);
    }
    public override void NewDay(int dayCount)
    {
        partyData.NewDay(dayCount);
        Save();
    }
    public override void Rest()
    {
        AddHours(GetRestingPeriod());
        partyData.Rest();
    }
    public void UpdateEnemies(int except)
    {
        for (int i = 0; i < enemyManagers.Count; i++)
        {
            enemyManagers[i].NewDay(dayCount);
            enemyManagers[i].MoveEnemies(except);
        }
    }
    public override void NewGame()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        location = savedOverworld.GetCenterCityLocation();
        dayCount = 0;
        currentHour = 0;
        Save();
        Load();
    }
    public override void Save()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        allData = location + delimiter + dayCount + delimiter + currentHour + delimiter + battleType;
        File.WriteAllText(dataPath, allData);
        partyData.Save();
    }
    public override void Load()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        if (File.Exists(dataPath)) { allData = File.ReadAllText(dataPath); }
        else
        {
            NewGame();
            return;
        }
        dataList = allData.Split(delimiter).ToList();
        for (int i = 0; i < dataList.Count; i++)
        {
            SetData(dataList[i], i);
        }
    }

    protected void SetData(string data, int index)
    {
        switch (index)
        {
            case 0:
                SetLocation(int.Parse(data));
                break;
            case 1:
                SetDay(int.Parse(data));
                break;
            case 2:
                SetHour(int.Parse(data));
                break;
            case 3:
                SetBattleType(data);
                break;
        }
    }
}
