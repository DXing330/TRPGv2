using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecificStat", menuName = "ScriptableObjects/DataContainers/SpecificStat", order = 1)]
public class SpecificStatDatabase : StatDatabase
{
    public StatDatabase allStats;
    public int specificIndex;

    public override void GetKeys()
    {
        keys = allStats.allKeys.Split(allStats.keyDelimiter).ToList();
    }

    public override void GetValues()
    {
        allStats.GetValues();
        List<string> allValues = new List<string>(allStats.values);
        values.Clear();
        string[] specificValues = new string[0];
        for (int i = 0; i < allValues.Count; i++)
        {
            specificValues = allValues[i].Split(allStats.valueDelimiter);
            values.Add(specificValues[specificIndex]);
        }
    }
}
