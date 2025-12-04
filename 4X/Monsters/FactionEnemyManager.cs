using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionEnemyManager : MonoBehaviour
{
    // Takes combat units and decides what they do.
    public CombatUnit dummyCombatUnit;
    public FactionEnemyUnitData enemyData;// Gets information from a saved data.
    // There will be at least 3 of them, 1 for bandits, 1 for normal monsters and 1 for higher tier monsters.
    // This is a base class from which the other two will inherit from.
}
