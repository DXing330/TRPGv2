using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BanditManager", menuName = "ScriptableObjects/DataContainers/SavedData/BanditManager", order = 1)]
public class BanditManager : OverworldEnemyManager
{
    public string delimiterTwo;
    public SavedOverworld savedOverworld;
    // How often bandits spawn, maybe make this a setting or adjustable later.
    public int spawnRate;
    // Only really spawn on forests or plains.
    public List<string> spawnableTerrain;
    // Can't be too close to a city, it would be too easy for city guards to eliminate.
    public int minDistanceFromCity;
    // Can't have too many camps. Probably ~2 per city, only so much to rob after all.
    public int maxCamps;
    // Each camp can only spawn 1 bandit per level?
    public int maxBandits;
    public int maxCampSize;
    public int banditPerLevel;
    // Sprite name for bandit camps.
    public string banditCampString;
    // Sprite name for overworld bandits.
    public string banditString;
    // Camps will spawn randomly, store their locations here?
    // If you enter a camp you will have to fight a lot of bandits.
    // If you win the fight, then the bandit camp is destroyed.
    public List<string> banditCamps;
    // As time goes on cities will put bounties on larger camps.
    // Bandit camps naturally grow in size to a limit until they are eliminated.
    public List<string> banditCampSizes;
    // Bandits will spawn from camps every X days/weeks.
    // Bandits will move every day.
    public List<string> banditLocations;
    // Bigger camps spawn higher level bandits (bigger groups).
    public List<string> banditLevels;
    public void RemoveBanditsAtLocation(int location)
    {
        int indexOf = banditLocations.IndexOf(location.ToString());
        banditLocations.RemoveAt(indexOf);
        banditLevels.RemoveAt(indexOf);
        savedOverworld.RemoveCharacterAtLocation(location);
    }

    public override void NewDay(int dayCount)
    {
        if (dayCount%spawnRate == 0)
        {
            UpdateBanditCamps();
        }
        Save();
    }

    public override void MoveEnemies(int except)
    {
        UpdateBanditLocations(except);
    }

    public override bool EnemiesOnTile(int tileNumber)
    {
        int indexOf = banditLocations.IndexOf(tileNumber.ToString());
        if (indexOf >= 0)
        {
            GenerateEnemies(indexOf);
            // Later do this later, or else players might be able to avoid combat by quitting in the middle of battle?
            // Alternatively adjust the game state tracker so you can quit in the middle of battle and reload in battle.
            RemoveBanditsAtLocation(tileNumber);
            return true;
        }
        return false;
    }

    public override void GenerateEnemies(int index)
    {
        currentEnemyPool = new List<string>();
        int enemyCount = int.Parse(banditLevels[index])*banditPerLevel;
        for (int i = 0; i < enemyCount; i++)
        {
            currentEnemyPool.Add(baseEnemyPool[Random.Range(0, baseEnemyPool.Count)]);
        }
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
        allData = "";
        for (int i = 0; i < banditCamps.Count; i++)
        {
            allData += banditCamps[i];
            if (i < banditCamps.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        for (int i = 0; i < banditCampSizes.Count; i++)
        {
            allData += banditCampSizes[i];
            if (i < banditCampSizes.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        for (int i = 0; i < banditLocations.Count; i++)
        {
            allData += banditLocations[i];
            if (i < banditLocations.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        for (int i = 0; i < banditLevels.Count; i++)
        {
            allData += banditLevels[i];
            if (i < banditLevels.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
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
        banditCamps = dataList[0].Split(delimiterTwo).ToList();
        banditCampSizes = dataList[1].Split(delimiterTwo).ToList();
        banditLocations = dataList[2].Split(delimiterTwo).ToList();
        banditLevels = dataList[3].Split(delimiterTwo).ToList();
        utility.RemoveEmptyListItems(banditCamps);
        utility.RemoveEmptyListItems(banditCampSizes);
        utility.RemoveEmptyListItems(banditLocations);
        utility.RemoveEmptyListItems(banditLevels);
    }

    public void UpdateBanditCamps()
    {
        // Level them up.
        for (int i = 0; i < banditCampSizes.Count; i++)
        {
            int campLevel = int.Parse(banditCampSizes[i]);
            int roll = Random.Range(0, campLevel*campLevel);
            if (roll == 0)
            {
                banditCampSizes[i] = (campLevel+1).ToString();
            }
        }
        // Spawn new camps.
        if (banditCamps.Count < maxCamps)
        {
            // Get a random empty tile.
            int potentialCamp = savedOverworld.RandomTile();
            // Check if it is far away enough from a city.
            if (spawnableTerrain.Contains(savedOverworld.ReturnTerrain(potentialCamp)) && savedOverworld.ReturnClosestCityDistance(potentialCamp) >= minDistanceFromCity)
            {
                SpawnCamp(potentialCamp);
            }
        }
        // Spawn bandits from camps.
        for (int i = 0; i < banditCamps.Count; i++)
        {
            if (savedOverworld.AddCharacter(banditString, banditCamps[i]))
            {
                banditLocations.Add(banditCamps[i]);
                int newLevel = Random.Range(1, int.Parse(banditCampSizes[i])+1);
                banditLevels.Add(newLevel.ToString());
            }
        }
    }

    protected void SpawnCamp(int tileNumber)
    {
        if (savedOverworld.AddFeature(banditCampString, tileNumber.ToString()))
        {
            banditCamps.Add(tileNumber.ToString());
            banditCampSizes.Add("1");
        }
    }

    public void UpdateBanditLocations(int except)
    {
        for (int i = 0; i < banditLocations.Count; i++)
        {
            if (int.Parse(banditLocations[i]) == except){continue;}
            // Try having the bandits move randomly for now.
            int direction = Random.Range(0, 6);
            int newTile = mapUtility.PointInDirection(int.Parse(banditLocations[i]), direction, savedOverworld.GetSize());
            if (newTile < 0){continue;}
            if (savedOverworld.MoveCharacter(banditLocations[i], newTile.ToString()))
            {
                banditLocations[i] = newTile.ToString();
            }
        }
    }
}
