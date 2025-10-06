using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorMaker : MonoBehaviour
{
    public Equipment equipmentPrefab;
    public TacticActor actorPrefab;
    public BattleModifier battleModifier;
    public StatDatabase actorStats;
    public StatDatabase spriteElementMapping;
    public StatDatabase elementPassives;
    public StatDatabase spriteSpeciesMapping;
    public StatDatabase speciesPassives;
    public PassiveOrganizer passiveOrganizer;
    public MapPatternLocations mapPatterns;
    public int mapSize;
    public void SetMapSize(int newSize) { mapSize = newSize; }

    [ContextMenu("New Actor")]
    public TacticActor CreateActor()
    {
        TacticActor newActor = Instantiate(actorPrefab, transform.position, new Quaternion(0, 0, 0, 0));
        // Need to reset somethings so that they don't carryover.
        newActor.ResetWeaponType();
        newActor.ClearStatuses();
        newActor.ResetPassives();
        newActor.ResetCounter();
        newActor.SetCurrentHealth(0);
        return newActor;
    }

    protected void SetActorName(TacticActor actor, string actorName)
    {
        // Currently species is useless at this rate.
        actor.SetSpriteName((actorName));
        actor.SetStats(actorStats.ReturnStats(actorName));
        // Set the element based on sprite name.
        actor.SetElement(spriteElementMapping.ReturnValue(actorName));
        actor.SetSpecies(spriteSpeciesMapping.ReturnValue(actorName));
    }

    protected void AddElementPassives(TacticActor actor)
    {
        string elemental = elementPassives.ReturnValue(actor.GetElement());
        if (elemental == "")
        {
            return;
        }
        string[] blocks = elemental.Split("|");
        string[] passives = blocks[0].Split(",");
        string[] levels = blocks[1].Split(",");
        for (int i = 0; i < passives.Length; i++)
        {
            actor.AddPassiveSkill(passives[i], levels[i]);
        }
    }

    protected void AddSpeciesPassives(TacticActor actor)
    {
        string speciesPassive = speciesPassives.ReturnValue(actor.GetSpecies());
        if (speciesPassive == "")
        {
            return;
        }
        string[] blocks = speciesPassive.Split("|");
        string[] passives = blocks[0].Split(",");
        string[] levels = blocks[1].Split(",");
        for (int i = 0; i < passives.Length; i++)
        {
            actor.AddPassiveSkill(passives[i], levels[i]);
        }
    }

    public TacticActor SpawnActor(int location, string actorName, int team = 0)
    {
        TacticActor newActor = CreateActor();
        newActor.SetLocation(location);
        SetActorName(newActor, actorName);
        newActor.SetPersonalName(actorName);
        newActor.SetTeam(team);
        passiveOrganizer.OrganizeActorPassives(newActor);
        newActor.ResetStats();
        return newActor;
    }

    public List<TacticActor> SpawnTeamInPattern(int pattern, int team, List<string> teamNames, List<string> teamStats = null, List<string> teamPersonalNames = null, List<string> teamEquipment = null)
    {
        if (teamStats == null) { teamStats = new List<string>(); }
        if (teamPersonalNames == null) { teamPersonalNames = new List<string>(); }
        if (teamEquipment == null) { teamEquipment = new List<string>(); }
        List<TacticActor> actors = new List<TacticActor>();
        // Randomize the team name order to randomize their spawn locations?
        List<int> patternLocations = mapPatterns.ReturnTilesOfPattern(pattern, teamNames.Count, mapSize);
        for (int i = 0; i < teamNames.Count; i++)
        {
            actors.Add(SpawnActor(patternLocations[i], teamNames[i], team));
            if (i < teamStats.Count)
            {
                actors[i].SetStatsFromString(teamStats[i]);
            }
            if (i < teamPersonalNames.Count)
            {
                actors[i].SetPersonalName(teamPersonalNames[i]);
            }
            else { actors[i].SetPersonalName(actors[i].GetSpriteName()); }
            if (i < teamEquipment.Count)
            {
                string[] equipData = teamEquipment[i].Split("@");
                for (int j = 0; j < equipData.Length; j++)
                {
                    equipmentPrefab.SetAllStats(equipData[j]);
                    equipmentPrefab.EquipToActor(actors[i]);
                }
            }
            // Add the elemental passives at the end.
            AddElementPassives(actors[i]);
            AddSpeciesPassives(actors[i]);
            passiveOrganizer.OrganizeActorPassives(actors[i]);
            actors[i].ResetStats();
        }
        return actors;
    }

    public void ChangeActorForm(TacticActor actor, string newForm)
    {
        // Change the sprite name.
        // Update the base stats of the actor.
        actor.SetSpriteName((newForm));
        actor.SetElement(spriteElementMapping.ReturnValue(newForm));
        actor.SetSpecies(spriteSpeciesMapping.ReturnValue(newForm));
        actor.ChangeForm(actorStats.ReturnStats(newForm));
        AddElementPassives(actor);
        AddSpeciesPassives(actor);
        // Set the new base health equal to the current health.
        actor.SetBaseHealth(actor.GetHealth());
        passiveOrganizer.OrganizeActorPassives(actor);
    }

    public TacticActor CloneActor(TacticActor actor, int location)
    {
        TacticActor newActor = CreateActor();
        SetActorName(newActor, actor.GetSpriteName());
        newActor.SetPersonalName(actor.GetPersonalName());
        newActor.SetLocation(location);
        newActor.CopyBaseStats(actor);
        passiveOrganizer.OrganizeActorPassives(actor);
        return newActor;
    }

    public void ApplyBattleModifiers(List<TacticActor> actors, List<string> battleMods)
    {
        for (int i = 0; i < battleMods.Count; i++)
        {
            battleModifier.LoadModifierByName(battleMods[i]);
            for (int j = 0; j < actors.Count; j++)
            {
                battleModifier.ApplyModifiers(actors[j]);
            }
        }
    }
}
