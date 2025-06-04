using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleMapFeatures", menuName = "ScriptableObjects/DataContainers/BattleMapFeatures", order = 1)]
public class BattleMapFeatures : ScriptableObject
{
    public string currentTileType;
    public void SetTileType(string newType){currentTileType = newType;}
    public void ResetTileType(){currentTileType = "";}
    public List<MapFeaturesList> mapFeatures;

    public MapFeaturesList CurrentMapFeatures()
    {
        for (int i = 0; i < mapFeatures.Count; i++)
        {
            if (currentTileType == mapFeatures[i].baseTileType)
            {
                return mapFeatures[i];
            }
        }
        return mapFeatures[0];
    }
}
