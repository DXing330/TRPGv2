using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleMapUtility", menuName = "ScriptableObjects/Utility/BattleMapUtility", order = 1)]
public class BattleMapUtility : ScriptableObject
{
    public GeneralUtility utility;
    public MapUtility mapUtility;
}
