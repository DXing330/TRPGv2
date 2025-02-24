using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainPassivesList", menuName = "ScriptableObjects/BattleLogic/TerrainPassivesList", order = 1)]
public class TerrainPassivesList : ScriptableObject
{
    public List<string> keys;
    public List<TerrainPassives> passives;

    public bool TerrainPassivesExist(string key)
    {
        int indexOf = keys.IndexOf(key);
        return indexOf >= 0;
    }

    public TerrainPassives ReturnTerrainPassive(string key)
    {
        int indexOf = keys.IndexOf(key);
        if (indexOf == -1){return null;}
        return passives[indexOf];
    }
}
