using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatData", menuName = "ScriptableObjects/DataContainers/StatData", order = 1)]
public class StatDatabase : ScriptableObject
{
    public bool inputKeysAndValues = false;
    public bool inputBothv2 = false;
    public string allKeysAndValues;
    public void SetAllData(string newData){allKeysAndValues = newData;}
    public string keyValueDelimiter;
    public string keyDelimiter;
    public string valueDelimiter;
    public string allKeys;
    public string allValues;
    public List<string> keys;
    public List<string> values;

    public virtual void Initialize()
    {
        if (inputKeysAndValues)
        {
            string[] keysAndValues = allKeysAndValues.Split(keyValueDelimiter);
            SetAllKeys(keysAndValues[0]);
            SetValues(keysAndValues[1]);
        }
        GetKeys();
        GetValues();
    }

    public void SetAllKeys(string newKeys)
    {
        allKeys = newKeys;
    }

    public void SetValues(string newValues)
    {
        allValues = newValues;
    }

    public virtual void GetKeys()
    {
        keys = allKeys.Split(keyDelimiter).ToList();
    }

    public virtual void GetValues()
    {
        values = allValues.Split(keyDelimiter).ToList();
    }

    public List<string> ReturnStats(string statName)
    {
        int indexOf = keys.IndexOf(statName);
        List<string> stats = new List<string>();
        if (indexOf < 0){return stats;}
        stats = values[indexOf].Split(valueDelimiter).ToList();
        return stats;
    }

    public string ReturnValue(string key)
    {
        int indexOf = keys.IndexOf(key);
        if (indexOf < 0){return "";}
        return values[indexOf];
    }

    public bool KeyExists(string key)
    {
        return keys.Contains(key);
    }

    public string ReturnRandomKey()
    {
        int index = Random.Range(0, keys.Count);
        return keys[index];
    }

    public string ReturnRandomValue()
    {
        int index = Random.Range(0, values.Count);
        return values[index];
    }
}
