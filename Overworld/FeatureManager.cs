using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatureManager : MonoBehaviour
{
    public StatDatabase featureToScenes;
    public StatDatabase featureToSpecifics;

    public string ReturnSceneName(string feature)
    {
        return featureToScenes.ReturnValue(feature);
    }

    public string ReturnFeatureSpecifics(string feature)
    {
        return featureToSpecifics.ReturnValue(feature);
    }

    public List<string> ReturnFeatureSpecificsList(string feature)
    {
        string specifics = ReturnFeatureSpecifics(feature);
        return specifics.Split(featureToSpecifics.valueDelimiter).ToList();
    }

    public List<string> ReturnRandomFeatureSpecificsList(string feature)
    {
        List<string> possibleFeatures = ReturnFeatureSpecificsList(feature);
        int index = Random.Range(0, possibleFeatures.Count);
        return possibleFeatures[index].Split(",").ToList();
    }
}
