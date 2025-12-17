using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionCity : MonoBehaviour
{
    public GeneralUtility utility;
    public string delimiter;
    public string delimiterTwo;
    public string factionName;
    public void SetFaction(string fName){factionName = fName;}
    public string GetFaction(){return factionName;}
    public string factionColor;
    public void SetColor(string fColor){factionColor = fColor;}
    public string GetColor(){return factionColor;}
    public int location;
    public void SetLocation(int loc){location = loc;}
    public int GetLocation(){return location;}
    public List<int> ownedTiles;
    public List<int> GetOwnedTiles()
    {
        return ownedTiles;
    }
    public bool OwnTile(int tileNumber)
    {
        return ownedTiles.Contains(tileNumber);
    }
    public void AddTile(int tileNumber)
    {
        ownedTiles.Add(tileNumber);
    }
    public int mana;
    public int gold;
    public int food;
    public int materials;
    public List<string> resources;
    // Market, mage tower, barracks, defenses, storage silos
    public List<string> upgrades;
    public List<int> upgradeLevels;
    public bool UpgradeExists(string upgrade)
    {
        return upgrades.Contains(upgrade);
    }
    public int GetLevelOfUpgrade(string upgrade)
    {
        if (!UpgradeExists(upgrade)){return 0;}
        int indexOf = upgrades.IndexOf(upgrade);
        return upgradeLevels[indexOf];
    }

    public void ResetStats()
    {
        factionName = "";
        factionColor = "";
        location = -1;
        ownedTiles.Clear();
        mana = 0;
        gold = 0;
        food = 0;
        materials = 0;
        resources.Clear();
        upgrades.Clear();
        upgradeLevels.Clear();
    }

    public string GetStats()
    {
        string data = "";
        data += factionName + delimiter;
        data += factionColor + delimiter;
        data += location + delimiter;
        data += String.Join(delimiterTwo, ownedTiles) + delimiter;
        data += mana + delimiter;
        data += gold + delimiter;
        data += food + delimiter;
        data += materials + delimiter;
        data += String.Join(delimiterTwo, resources) + delimiter;
        data += String.Join(delimiterTwo, upgrades) + delimiter;
        data += String.Join(delimiterTwo, upgradeLevels) + delimiter;
        return data;
    }

    public void SetStats(string data)
    {
        string[] blocks = data.Split(delimiter);
        for (int i = 0; i < blocks.Length; i++)
        {
            LoadStat(blocks[i], i);
        }
    }

    protected void LoadStat(string stat, int index)
    {
        switch (index)
        {
            default:
            break;
            case 0:
                factionName = stat;
                break;
            case 1:
                factionColor = stat;
                break;
            case 2:
                location = int.Parse(stat);
                break;
            case 3:
                ownedTiles = utility.ConvertStringListToIntList(stat.Split(delimiterTwo).ToList());
                break;
            case 4:
                mana = int.Parse(stat);
                break;
            case 5:
                gold = int.Parse(stat);
                break;
            case 6:
                food = int.Parse(stat);
                break;
            case 7:
                materials = int.Parse(stat);
                break;
            case 8:
                resources = stat.Split(delimiterTwo).ToList();
                break;
            case 9:
                upgrades = stat.Split(delimiterTwo).ToList();
                break;
            case 10:
                upgradeLevels = utility.ConvertStringListToIntList(stat.Split(delimiterTwo).ToList());
                break;
        }
    }
}
