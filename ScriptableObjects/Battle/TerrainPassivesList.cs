using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

[CreateAssetMenu(fileName = "TerrainPassivesList", menuName = "ScriptableObjects/BattleLogic/TerrainPassivesList", order = 1)]
public class TerrainPassivesList : StatDatabase
{
    public List<TerrainPassives> passives;

    public override void Initialize()
    {
        if (inputKeysAndValues)
        {
            string[] keysAndValues = allKeysAndValues.Split(keyValueDelimiter);
            SetAllKeys(keysAndValues[0]);
            SetValues(keysAndValues[1]);
            GetKeys();
            GetValues();
            #if UNITY_EDITOR
                EditorUtility.SetDirty(this);
            #endif
        }
        for (int i = 0; i < passives.Count; i++)
        {
            passives[i].SetAllData(values[i]);
            passives[i].Initialize();
        }
    }

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
