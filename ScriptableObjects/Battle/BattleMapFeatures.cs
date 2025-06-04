using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleMapFeatures", menuName = "ScriptableObjects/DataContainers/BattleMapFeatures", order = 1)]
public class BattleMapFeatures : ScriptableObject
{
    public string terrainType;
    public void SetTerrainType(string newType){terrainType = newType;}
    public void ResetTerrainType(){terrainType = "";}
    public List<MapFeaturesList> mapFeatures;

    public MapFeaturesList CurrentMapFeatures()
    {
        for (int i = 0; i < mapFeatures.Count; i++)
        {
            if (terrainType == mapFeatures[i].baseTileType)
            {
                return mapFeatures[i];
            }
        }
        return mapFeatures[0];
    }
}
