using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorEquipment : ActorPassives
{
    public void SetStats(List<string> newStats)
    {
        slot = newStats[0];
        equipType = newStats[1];
        SetPassiveSkills(newStats[2].Split(",").ToList());
    }
    public string slot;
    public string equipType;

    public void GiveActorPassives(TacticActor actor)
    {
        switch (slot)
        {
            case "Weapon":
            actor.allStats.AddStartBattlePassives(startBattlePassives);
            actor.allStats.AddAttackingPassives(attackingPassives);
            break;
            case "Armor":
            actor.allStats.AddStartBattlePassives(startBattlePassives);
            actor.allStats.AddDefendingPassives(defendingPassives);
            actor.allStats.AddTakeDamagePassives(takeDamagePassives);
            break;
            case "Boots":
            actor.allStats.AddStartBattlePassives(startBattlePassives);
            actor.allStats.AddMovingPassives(movingPassives);
            break;
            case "Accessory":
            actor.allStats.AddStartTurnPassives(startTurnPassives);
            actor.allStats.AddEndTurnPassives(endTurnPassives);
            break;
        }
    }
}
