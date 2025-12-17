using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FactionCityData", menuName = "ScriptableObjects/4X/FactionCityData", order = 1)]
public class FactionCityData : SavedData
{
    public string delimiterTwo;
    public List<string> savedCities;
    public List<int> cityLocations;

    public string GetCityAtLocation(int location)
    {
        int indexOf = cityLocations.IndexOf(location);
        if (indexOf < 0){return "";}
        return savedCities[indexOf];
    }

    public void RemoveCityAtLocation(int location)
    {
        int indexOf = cityLocations.IndexOf(location);
        if (indexOf < 0){return;}
        savedCities.RemoveAt(indexOf);
        cityLocations.RemoveAt(indexOf);
        Save();
    }

    public void UpdateCityAtLocation(string city, int location)
    {
        int indexOf = cityLocations.IndexOf(location);
        if (indexOf < 0){return;}
        savedCities[indexOf] = city;
    }

    public void AddCity(string city, int location)
    {
        savedCities.Add(city);
        cityLocations.Add(location);
    }

    public override void NewGame()
    {
        savedCities.Clear();
        cityLocations.Clear();
        Save();
    }

    public override void Save()
    {
        dataPath = Application.persistentDataPath+"/"+filename;
        allData = "";
        allData += String.Join(delimiterTwo, savedCities) + delimiter;
        allData += String.Join(delimiterTwo, cityLocations) + delimiter;
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
        for (int i = 0; i < dataList.Count; i++)
        {
            LoadStat(dataList[i], i);
        }
    }

    protected void LoadStat(string stat, int i)
    {
        switch (i)
        {
            default:
            break;
            case 0:
                savedCities = stat.Split(delimiterTwo).ToList();
                break;
            case 1:
                cityLocations = utility.ConvertStringListToIntList(stat.Split(delimiterTwo).ToList());
                break;
        }
    }
}
