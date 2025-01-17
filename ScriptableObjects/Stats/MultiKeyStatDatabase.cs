using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiKeyStatData", menuName = "ScriptableObjects/DataContainers/MultiKeyStatData", order = 1)]
public class MultiKeyStatDatabase : StatDatabase
{
    public int keyCount;
    public string allSecondKeys;
    public string allThirdKeys;
    public List<string> secondKeys;
    public List<string> thirdKeys;

    public void SetAllSecondKeys(string newKeys)
    {
        allSecondKeys = newKeys;
    }

    public void SetAllThirdKeys(string newKeys)
    {
        allThirdKeys = newKeys;
    }

    public override void GetKeys()
    {
        keys = allKeys.Split(keyDelimiter).ToList();
        if (keyCount > 1)
        {
            secondKeys = allSecondKeys.Split(keyDelimiter).ToList();
            if (keyCount > 2)
            {
                thirdKeys = allThirdKeys.Split(keyDelimiter).ToList();
            }
        }
    }

    public string GetMultiKeyValue(string key1, string key2 = "", string key3 = "")
    {
        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i] == key1)
            {
                if (keyCount <= 1){return values[i];}
                if (keyCount <= 2 && secondKeys[i] == key2){return values[i];}
                if (keyCount <= 3 && secondKeys[i] == key2 && thirdKeys[i] == key3){return values[i];}
            }
        }
        return "";
    }
}
