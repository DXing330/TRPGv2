using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

[CreateAssetMenu(fileName = "TerrainPassivesList", menuName = "ScriptableObjects/BattleLogic/TerrainPassivesList", order = 1)]
public class TerrainPassivesList : StatDatabase
{
    public string delimiterTwo;

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
    }

    public bool TerrainPassivesExist(string key)
    {
        int indexOf = keys.IndexOf(key);
        return indexOf >= 0;
    }

    protected string ReturnSpecificPassive(string key, int index)
    {
        string[] values = ReturnValue(key).Split(delimiterTwo);
        if (index < 0 || index >= values.Length)
        {
            return "";
        }
        return values[index];
    }

    public string ReturnAttackingPassive(string key)
    {
        return ReturnSpecificPassive(key, 0);
    }

    public string ReturnDefendingPassive(string key)
    {
        return ReturnSpecificPassive(key, 1);
    }

    public string ReturnMovingPassive(string key)
    {
        return ReturnSpecificPassive(key, 2);
    }

    public string ReturnStartPassive(string key)
    {
        return ReturnSpecificPassive(key, 3);
    }

    public string ReturnEndPassive(string key)
    {
        return ReturnSpecificPassive(key, 4);
    }
}
