using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

[CreateAssetMenu(fileName = "StatData", menuName = "ScriptableObjects/DataContainers/StatData", order = 1)]
public class StatDatabase : ScriptableObject
{
    public bool inputKeysAndValues = false;
    public bool inputBothv2 = false;
    public string allKeysAndValues;
    public virtual void SetAllData(string newData){allKeysAndValues = newData;}
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
        #if UNITY_EDITOR
                EditorUtility.SetDirty(this);
        #endif
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

    public bool KeyExists(string key)
    {
        return keys.Contains(key);
    }

    public string ReturnRandomKey()
    {
        int index = Random.Range(0, keys.Count);
        return keys[index];
    }

    // Used specifically for selecting random enemies for the roguelike.
    public string ReturnRandomKeyBasedOnIntValue(int value)
    {
        List<string> possibleKeys = new List<string>();
        for (int i = 0; i < keys.Count; i++)
        {
            if (int.Parse(values[i]) == value)
            {
                possibleKeys.Add(keys[i]);
            }
        }
        if (possibleKeys.Count == 0 && value > 0)
        {
            return ReturnRandomKeyBasedOnIntValue(value - 1);
        }
        else if (possibleKeys.Count > 0)
        {
            return possibleKeys[Random.Range(0, possibleKeys.Count)];
        }
        return "";
    }

    public string ReturnKeyAtIndex(int index)
    {
        if (index >= 0 && index < keys.Count)
        {
            return keys[index];
        }
        return "";
    }

    public List<string> ReturnAllKeys()
    {
        return keys;
    }

    public string ReturnValue(string key)
    {
        int indexOf = keys.IndexOf(key);
        if (indexOf < 0){return "";}
        return values[indexOf];
    }

    public string ReturnValueAtIndex(int index)
    {
        if (index >= 0 && index < values.Count)
        {
            return values[index];
        }
        return "";
    }

    public string ReturnRandomValue()
    {
        int index = Random.Range(0, values.Count);
        return values[index];
    }
}
