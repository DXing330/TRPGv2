using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapFeatures", menuName = "ScriptableObjects/DataContainers/MapFeatures", order = 1)]
public class MapFeaturesList : ScriptableObject
{
    public string baseTileType;
    public int maxFeatures;
    public List<string> features;
    public List<string> patterns;

    public string RandomFeature()
    {
        return features[Random.Range(0, features.Count)];
    }

    public List<string> RandomFeaturePattern()
    {
        List<string> featurePattern = new List<string>();
        int index = Random.Range(0, features.Count);
        featurePattern.Add(features[index]);
        featurePattern.Add(patterns[index]);
        return featurePattern;
    }
}
