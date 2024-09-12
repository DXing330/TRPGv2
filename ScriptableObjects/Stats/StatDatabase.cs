using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatData", menuName = "ScriptableObjects/DataContainers/StatData", order = 1)]
public class StatDatabase : ScriptableObject
{
    public string keyDelimiter;
    public string valueDelimiter;
    public string allKeys;
    public string allValues;
    public List<string> keys;
    public List<string> values;

    public void Initialize()
    {
        GetKeys();
        GetValues();
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
}
