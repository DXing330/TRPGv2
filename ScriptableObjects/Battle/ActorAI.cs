using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActorAI", menuName = "ScriptableObjects/ActorAI", order = 1)]
public class ActorAI : ScriptableObject
{
    public string AIType;

    public string NaiveChoseAction(bool adjacentEnemies = false)
    {
        // Naive AI: Move toward target, then attack.
        // If no enemies in range.
        if (!adjacentEnemies){return "Move";}
        // If enemies in range.
        else{return "Attack";}
    }
}
