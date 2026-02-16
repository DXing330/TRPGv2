using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SavedMaps", menuName = "ScriptableObjects/DataContainers/SavedData/SavedMaps", order = 1)]
public class MapEditorSaver : SavedData
{
    public int maxMaps = 999999;
    public int currentMap;
    public string delimiterTwo;
    public void ChangeCurrentMap(MapEditor map, bool right = true)
    {
        currentMap = utility.ChangeIndex(currentMap, right, maxMaps);
        LoadMap(map);
    }
    public void SetCurrentMap(MapEditor map, int newInfo)
    {
        if (newInfo > maxMaps)
        {
            newInfo = maxMaps;
        }
        currentMap = newInfo;
        LoadMap(map);
    }
    public void SaveMap(MapEditor map)
    {
        dataPath = Application.persistentDataPath + "/" + filename + currentMap;
        allData = "";
        allData += "Tiles=" + String.Join(delimiterTwo, map.cMapInfo) + delimiter;
        allData += "TEffects=" + String.Join(delimiterTwo, map.cTerrainEffects) + delimiter;
        allData += "Elevations=" + String.Join(delimiterTwo, map.cTileElevations) + delimiter;
        allData += "Buildings=" + String.Join(delimiterTwo, map.cBuildings) + delimiter;
        allData += "Borders=" + String.Join(delimiterTwo, map.cBorders);
        File.WriteAllText(dataPath, allData);
    }
    public void LoadMap(MapEditor map)
    {
        dataPath = Application.persistentDataPath + "/" + filename + currentMap;
        if (File.Exists(dataPath)){allData = File.ReadAllText(dataPath);}
        else
        {
            map.InitializeNewMap();
            return;
        }
        dataList = allData.Split(delimiter).ToList();
        for (int i = 0; i < dataList.Count; i++)
        {
            if (!LoadStat(dataList[i], map))
            {
                map.InitializeNewMap();
                return;
            }
        }
        map.UndoEdits();
    }
    protected virtual bool LoadStat(string data, MapEditor map)
    {
        string[] blocks = data.Split("=");
        if (blocks.Length < 2){return false;}
        string value = blocks[1];
        switch (blocks[0])
        {
            default:
                return false;
            case "Tiles":
                map.mapInfo = value.Split(delimiterTwo).ToList();
                return true;
            case "TEffects":
                map.terrainEffects = value.Split(delimiterTwo).ToList();
                return true;
            case "Elevations":
                map.tileElevations = value.Split(delimiterTwo).ToList();
                return true;
            case "Buildings":
                map.buildings = value.Split(delimiterTwo).ToList();
                return true;
            case "Borders":
                map.borders = value.Split(delimiterTwo).ToList();
                return true;
        }
    }
}
