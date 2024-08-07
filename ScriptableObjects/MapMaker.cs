using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapMaker", menuName = "ScriptableObjects/MapMaker", order = 1)]
public class MapMaker : ScriptableObject
{
    public List<string> possibleTiles;
    // Should add up to 100.
    public List<int> tileProbabilities;
}
