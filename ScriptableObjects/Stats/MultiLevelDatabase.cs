using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MultiLevelStats", menuName = "ScriptableObjects/DataContainers/MultiLevelStats", order = 1)]
public class MultiLevelDatabase : StatDatabase
{
    public string allData;
    public List<string> multiLevelData;
    public string allDataDelimiter;
    public StatDatabase statDatabase;
    public int statKeyIndex;
    public int statValueIndex;
    public MultiKeyStatDatabase multiKeyDatabase;
    public int multiKeyIndex1;
    public int multiKeyIndex2;
    public int multiKeyIndex3;
    public int multiKeyValueIndex;

    public override void Initialize()
    {
        multiLevelData = allData.Split(allDataDelimiter).ToList();
        statDatabase.SetAllKeys(multiLevelData[statKeyIndex]);
        statDatabase.SetValues(multiLevelData[statValueIndex]);
        multiKeyDatabase.SetAllKeys(multiLevelData[multiKeyIndex1]);
        multiKeyDatabase.SetAllSecondKeys(multiLevelData[multiKeyIndex2]);
        multiKeyDatabase.SetValues(multiLevelData[multiKeyValueIndex]);
    }
}
