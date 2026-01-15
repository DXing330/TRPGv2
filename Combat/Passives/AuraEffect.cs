using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// These will be generated at runtime whenever an aura is created.
// Aura list is stored in the terrain effects.
[System.Serializable]
public class AuraEffect
{
    // Auras don't store anything else, so they can use the most basic delimiters.
    public string delimiter = "|";
    public void InitializeAura(TacticActor user, int newDuration, string auraData)
    {
        actor = user;
        team = actor.GetTeam();
        triggeredActors = new List<TacticActor>();
        actorAura = 1;
        location = user.GetLocation();
        duration = newDuration;
        string[] data = auraData.Split(delimiter);
        for (int i = 0; i < data.Length; i++)
        {
            LoadStat(data[i], i);
        }
    }
    protected void LoadStat(string stat, int index)
    {
        switch (index)
        {
            default:
            return;
            case 0:
                auraName = stat;
                return;
            case 1:
                teamTarget = stat;
                return;
            case 2:
                shape = stat;
                return;
            case 3:
                span = int.Parse(stat);
                return;
            case 4:
                trigger = int.Parse(stat);
                return;
            case 5:
                condition = stat;
                return;
            case 6:
                conditionSpecifics = stat;
                return;
            case 7:
                target = stat;
                return;
            case 8:
                effect = stat;
                return;
            case 9:
                effectSpecifics = stat;
                return;
        }
    }
    // For now only actor auras, terrain already has terrain effects.
    // Auras only affect other actors, we have passives that can do every map effect anyway.
    // Actor auras move with the actor.
    public int actorAura;
    public bool ActorAura()
    {
        return actorAura > 0;
    }
    public TacticActor actor;
    public int team;
    public int location;
    public void UpdateLocation()
    {
        if (ActorAura() && actor != null && actor.GetHealth() > 0)
        {
            location = actor.GetLocation();
        }
    }
    // Auras last for some amount of rounds/turns.
    public int duration;
    public void NextRound(BattleMap map)
    {
        triggeredActors.Clear();
        duration--;
        if (duration < 0 || actor == null || actor.GetHealth() <= 0)
        {
            map.RemoveAura(this);
        }
    }
    // Used for logging.
    public string auraName;
    public string GetAuraName(){return auraName;}
    // Ally -> same team, Enemy -> !same team
    public string teamTarget;
    public bool TeamCheck(TacticActor target)
    {
        switch (teamTarget)
        {
            case "Enemy":
            return target.GetTeam() != team;
            case "Ally":
            return target.GetTeam() == team;
        }
        return false;
    }
    // Circle, Cone, ELine
    public string shape;
    public int span;
    public bool ActorInAura(TacticActor actor, BattleMap map)
    {
        return GetAuraTiles(map).Contains(actor.GetLocation());
    }
    // Use the map utility.
    // Also use the actor direction for cone auras.
    public List<int> GetAuraTiles(BattleMap map)
    {
        int start = map.mapUtility.PointInDirection(location, actor.GetDirection(), map.mapSize);
        List<int> tiles = map.mapUtility.GetTilesByShapeSpan(location, shape, span, map.mapSize);
        return tiles;
    }
    // Trigger auras apply whenever someone moves into them for the first time each round and meets the conditions, ex. Spirit Guardians.
    // Non trigger aura apply extra passive effects during battle if the actors meets the conditions, ex. Bernhardt.
    // Trigger - Start, End, Moving
    // Nontrigger - Attacking, Defending
    public int trigger;
    public bool TriggerAura()
    {
        return trigger > 0;
    }
    public bool AlreadyTriggered(TacticActor actor)
    {
        if (!TriggerAura()){return false;}
        return triggeredActors.Contains(actor);
    }
    public void ActorTriggersAura(TacticActor actor)
    {
        if (TriggerAura() && !triggeredActors.Contains(actor))
        {
            triggeredActors.Add(actor);
        }
    }
    // Each actor can only be affected by each trigger aura once per round.
    public List<TacticActor> triggeredActors;
    // Regular passive stuff.
    public string condition;
    public string conditionSpecifics;
    public string target;
    public string effect;
    public string effectSpecifics;
}