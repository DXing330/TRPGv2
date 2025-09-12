using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class DatabaseInitializer : MonoBehaviour
{
    public string allData;
    public string allDataDelimiter = "}";
    public string allStatData;
    public string allSpriteData;
    public GroupedStatDatabase masterDatabase;
    public GroupedSpriteContainer masterSprites;
    public List<StatDatabase> statData;
    public List<SpriteContainer> spriteContainers;
    public List<SpecificStatDatabase> specificStats;

    public void Initialize()
    {
        for (int i = 0; i < statData.Count; i++)
        {
            statData[i].Initialize();
#if UNITY_EDITOR
                EditorUtility.SetDirty(statData[i]);
#endif
        }
    }
    [ContextMenu("InitializeStatAndSpriteData")]
    public void InitializeStatAndSpriteData()
    {
        string[] blocks = allData.Split(allDataDelimiter);
        allStatData = blocks[0];
        allSpriteData = blocks[1];
        masterDatabase.SetAllData(allStatData);
        masterDatabase.Initialize();
        masterSprites.SetAllData(allSpriteData);
        masterSprites.Initialize();
        for (int i = 0; i < specificStats.Count; i++)
        {
            specificStats[i].Initialize();
        }
    }
}
