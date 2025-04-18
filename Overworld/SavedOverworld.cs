using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SavedOverworld", menuName = "ScriptableObjects/DataContainers/SavedData/SavedOverworld", order = 1)]
public class SavedOverworld : SavedData
{
    public GeneralUtility utility;
    public string delimiterTwo;
    public OverworldGenerator owGen;
    public MapUtility mapUtility;
    public int overworldSize = 99;
    public int GetSize(){return overworldSize;}
    public int zoneCount = 9;
    public int zoneSizeDivisor = 3;
    public List<string> possibleLuxuries;
    public List<string> GetPossibleLuxuries(){return possibleLuxuries;}
    public List<string> terrainLayer;
    public string ReturnTerrain(int tileNumber){return terrainLayer[tileNumber];}
    // Cities, Guild Hub, Villages, Caves, Ruins, Bandit Camps
    public List<string> featureLayer;
    public int RandomTile()
    {
        return Random.Range(0, terrainLayer.Count);
    }
    public string ReturnFeature(int tileNumber)
    {
        return featureLayer[tileNumber];
    }
    public bool FeatureExists(int tileNumber)
    {
        return featureLayer[tileNumber] != "";
    }
    public void AddFeature(string featureName, int featureLocation)
    {
        // Can't have features overlapping.
        if (featureLayer[featureLocation] == "")
        {
            featureLayer[featureLocation] = featureName;
        }
    }
    public string guildHubSprite = "Citadel";
    public List<string> cityLocationKeys;
    public List<string> luxuryLayer;
    // Player, Monsters, Bandits, NPCs
    public List<string> characterLayer;
    public void AddCharacter(string characterName, int characterLocation)
    {
        // Can't overlap.
        if (characterLayer[characterLocation] == "")
        {
            characterLayer[characterLocation] = characterName;
        }
    }
    public string ReturnCharacter(int tileNumber)
    {
        return characterLayer[tileNumber];
    }
    public bool CharacterExists(int tileNumber)
    {
        return characterLayer[tileNumber] != "";
    }
    public int ReturnClosestCityDistance(int tileNumber)
    {
        return mapUtility.DistanceBetweenTiles(tileNumber, ReturnClosestCityLocation(tileNumber), overworldSize);
    }
    public int ReturnClosestCityLocation(int tileNumber)
    {
        int distance = 999;
        int cityIndex = -1;
        for (int i = 0; i < cityLocationKeys.Count; i++)
        {
            // Don't count the guild hub as a city.
            if (i == cityLocationKeys.Count/2){continue;}
            int newDistance = mapUtility.DistanceBetweenTiles(tileNumber, int.Parse(cityLocationKeys[i]), overworldSize);
            if (newDistance < distance)
            {
                distance = newDistance;
                cityIndex = i;
            }
        }
        return int.Parse(cityLocationKeys[cityIndex]);
    }
    // If you enter the center city then it's the guild hub, not a regular city.
    public bool CenterCity(int cityLocation)
    {
        return cityLocation == GetCenterCityLocation();
    }
    public int GetCenterCityLocation()
    {
        return GetCityLocationFromIndex(cityLocationKeys.Count/2);
    }
    public int GetCityLocationFromIndex(int index)
    {
        if (index < 0 || index >= cityLocationKeys.Count){return -1;}
        return int.Parse(cityLocationKeys[index]);
    }
    // Price tiers: supplier, adjacent, normal, demanded
    public List<string> cityLuxurySupplys; // List of what luxury the city exports.
    public List<string> cityLuxuryAdjacent; // Cheaper than normal, but not as cheap as supplied.
    // List of what luxuries are in demand in each city, more expensive than usual.
    public List<string> cityLuxuryDemands; // Probably have to make this here, the overworld gen won't know since it only makes them one at a time.
    protected void ResetData()
    {
        terrainLayer = new List<string>();
        featureLayer = new List<string>();
        luxuryLayer = new List<string>();
        characterLayer = new List<string>();
        cityLocationKeys = new List<string>();
        cityLuxurySupplys = new List<string>();
        cityLuxuryAdjacent = new List<string>();
        cityLuxuryDemands = new List<string>();
        for (int i = 0; i < GetSize()*GetSize(); i++)
        {
            terrainLayer.Add("");
            featureLayer.Add("");
            luxuryLayer.Add("");
            characterLayer.Add("");
        }
    }

    protected void GenerateNewOverworld()
    {
        ResetData();
        List<string> zones = new List<string>();
        List<string> luxuryZoneOrder = new List<string>();
        List<string> possibleLuxuriesCopy = new List<string>(possibleLuxuries);
        // Generate the zones.
        for (int i = 0; i < zoneCount; i++)
        {
            // The middle zone has no city/luxury.
            if (i == zoneCount/2)
            {
                luxuryZoneOrder.Add("");
                zones.Add(owGen.GenerateZone(GetSize()/zoneSizeDivisor, "", true));
                continue;
            }
            // Pick a random luxury for each zone.
            int randomLuxIndex = Random.Range(0, possibleLuxuriesCopy.Count);
            string randomLux = possibleLuxuriesCopy[randomLuxIndex];
            possibleLuxuriesCopy.RemoveAt(randomLuxIndex);
            luxuryZoneOrder.Add(randomLux);
            zones.Add(owGen.GenerateZone(GetSize()/zoneSizeDivisor, randomLux));
        }
        // Stitch them together.
        int extZoneRow = 0;
        int extZoneCol = 0;
        for (int i = 0; i < zones.Count; i++)
        {
            int intZoneRow = 0;
            int intZoneCol = 0;
            string[] zoneInfo = zones[i].Split("@");
            List<string> zoneTerrain = new List<string>(zoneInfo[0].Split("#").ToList());
            List<string> zoneCities = new List<string>(zoneInfo[1].Split("#").ToList());
            List<string> zoneLuxuries = new List<string>(zoneInfo[2].Split("#").ToList());
            for (int j = 0; j < zoneTerrain.Count; j++)
            {
                int tileNumber = (((extZoneRow*(GetSize()/zoneSizeDivisor))+(intZoneRow))*(GetSize()))+((extZoneCol*(GetSize()/zoneSizeDivisor))+(intZoneCol));
                terrainLayer[tileNumber] = zoneTerrain[j];
                featureLayer[tileNumber] = zoneCities[j];
                if (zoneCities[j] == "City")
                {
                    cityLocationKeys.Add(tileNumber.ToString());
                    // Supply is based on what zone.
                    cityLuxurySupplys.Add(luxuryZoneOrder[i]);
                    // Demand is based on the opposite side zone.
                    cityLuxuryDemands.Add(luxuryZoneOrder[(luxuryZoneOrder.Count-1)-i]);
                }
                luxuryLayer[tileNumber] = zoneLuxuries[j];
                intZoneCol++;
                if (intZoneCol >= GetSize()/zoneSizeDivisor)
                {
                    intZoneCol = 0;
                    intZoneRow++;
                }
            }
            extZoneCol++;
            if (extZoneCol >= zoneSizeDivisor)
            {
                extZoneCol = 0;
                extZoneRow++;
            }
        }
        // Make the center city the guild hub.
        featureLayer[int.Parse(cityLocationKeys[cityLocationKeys.Count/2])] = guildHubSprite;
    }

    public override void NewGame()
    {
        GenerateNewOverworld();
        Save();
    }

    // Hope this is quicker, using linq, this requires loading first;
    public void QuickSave()
    {

    }

    public override void Save()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = "";
        for (int i = 0; i < terrainLayer.Count; i++)
        {
            allData += terrainLayer[i];
            if (i < terrainLayer.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        for (int i = 0; i < featureLayer.Count; i++)
        {
            allData += featureLayer[i];
            if (i < featureLayer.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        for (int i = 0; i < luxuryLayer.Count; i++)
        {
            allData += luxuryLayer[i];
            if (i < luxuryLayer.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        for (int i = 0; i < cityLocationKeys.Count; i++)
        {
            allData += cityLocationKeys[i];
            if (i < cityLocationKeys.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        for (int i = 0; i < cityLuxurySupplys.Count; i++)
        {
            allData += cityLuxurySupplys[i];
            if (i < cityLuxurySupplys.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        for (int i = 0; i < cityLuxuryAdjacent.Count; i++)
        {
            allData += cityLuxuryAdjacent[i];
            if (i < cityLuxuryAdjacent.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        for (int i = 0; i < cityLuxuryDemands.Count; i++)
        {
            allData += cityLuxuryDemands[i];
            if (i < cityLuxuryDemands.Count - 1){allData += delimiterTwo;}
        }
        allData += delimiter;
        for (int i = 0; i < characterLayer.Count; i++)
        {
            allData += characterLayer[i];
            if (i < characterLayer.Count - 1){allData += delimiterTwo;}
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
        terrainLayer = dataList[0].Split(delimiterTwo).ToList();
        featureLayer = dataList[1].Split(delimiterTwo).ToList();
        luxuryLayer = dataList[2].Split(delimiterTwo).ToList();
        cityLocationKeys = dataList[3].Split(delimiterTwo).ToList();
        cityLuxurySupplys = dataList[4].Split(delimiterTwo).ToList();
        cityLuxuryAdjacent = dataList[5].Split(delimiterTwo).ToList();
        cityLuxuryDemands = dataList[6].Split(delimiterTwo).ToList();
        characterLayer = dataList[7].Split(delimiterTwo).ToList();
    }
}
