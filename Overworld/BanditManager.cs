using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BanditManager", menuName = "ScriptableObjects/DataContainers/SavedData/BanditManager", order = 1)]
public class BanditManager : OverworldEnemyManager
{
    // Can't be too close to a city, it would be too easy for city guards to eliminate.
    public int minDistanceFromCity;
}
