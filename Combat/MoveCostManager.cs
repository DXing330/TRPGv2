using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCostManager : MonoBehaviour
{
    public List<MoveCosts> moveCosts;

    public int ReturnMoveCost(TacticActor actor, string tileType)
    {
        for (int i = 0; i < moveCosts.Count; i++)
        {
            if (moveCosts[i].moveType == actor.allStats.GetMoveType())
            {
                int cost = moveCosts[i].ReturnMoveCost(tileType);
                // Might be able to change this with character passives later.
                return cost;
            }
        }
        return 1;
    }
}
