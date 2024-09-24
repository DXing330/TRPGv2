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
            actor.AddStartBattlePassives(startBattlePassives);
            actor.AddAttackingPassives(attackingPassives);
            break;
            case "Armor":
            actor.AddStartBattlePassives(startBattlePassives);
            actor.AddDefendingPassives(defendingPassives);
            actor.AddTakeDamagePassives(takeDamagePassives);
            break;
            case "Boots":
            actor.AddStartBattlePassives(startBattlePassives);
            actor.AddMovingPassives(movingPassives);
            break;
            case "Accessory":
            actor.AddStartTurnPassives(startTurnPassives);
            actor.AddEndTurnPassives(endTurnPassives);
            break;
        }
    }
}
