using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleEndManager : MonoBehaviour
{
    public PartyDataManager partyData;
    public TacticActor dummyActor;
    public Equipment dummyEquip;
    public SavedOverworld overworld;
    public OverworldState overworldState;
    public SceneMover sceneMover;
    public GameObject battleEndScreen;
    public TMP_Text battleResult;
    public List<string> actorNames;
    public List<string> skillUpNames;
    public StatTextList allSkillUps;
    public int maxSkillLevel = 4;
    public int winnerTeam = -1;
    public void SetWinnerTeam(int newInfo) { winnerTeam = newInfo; }

    public int FindWinningTeam(List<TacticActor> actors)
    {
        int winningTeam = -1;
        List<int> teams = new List<int>();
        for (int i = 0; i < actors.Count; i++)
        {
            if (teams.IndexOf(actors[i].GetTeam()) < 0)
            {
                teams.Add(actors[i].GetTeam());
            }
        }
        if (teams.Count == 1) { return teams[0]; }
        return winningTeam;
    }

    public void UpdatePartyAfterBattle(List<TacticActor> actors, int winningTeam = 0)
    {
        if (winningTeam != 0)
        {
            PartyDefeated();
            return;
        }
        List<string> names = new List<string>();
        List<string> stats = new List<string>();
        for (int i = 0; i < actors.Count; i++)
        {
            names.Add(actors[i].GetPersonalName());
            stats.Add(actors[i].ReturnPersistentStats());
        }
        partyData.UpdatePartyAfterBattle(names, stats);
    }

    public void UpdateOverworldAfterBattle(int winningTeam)
    {
        if (winningTeam != 0) { return; }
        string battleType = overworldState.GetBattleType();
        int location = overworldState.GetLocation();
        if (battleType == "") { return; }
        switch (battleType)
        {
            case "Quest":
                overworld.RemoveFeatureAtLocation(location);
                partyData.guildCard.CompleteDefeatQuest(location);
                break;
            case "Feature":
                overworld.RemoveFeatureAtLocation(location);
                break;
            case "Event":
                break;
        }
    }

    public void PartyDefeated()
    {
        partyData.PartyDefeated();
    }

    public void EndBattle(int winningTeam)
    {
        SetWinnerTeam(winningTeam);
        if (winnerTeam == 0)
        {
            battleResult.text = "<color=green>Victory!</color>";
            CalculateSkillUps();
        }
        else
        {
            battleResult.text = "<color=red>Defeat...</color>";
            allSkillUps.Disable();
        }
        battleEndScreen.SetActive(true);
    }

    public void ReturnFromBattle()
    {
        sceneMover.ReturnFromBattle(winnerTeam);
    }

    protected void CalculateSkillUps()
    {
        actorNames.Clear();
        skillUpNames.Clear();
        List<string> spriteNames = new List<string>(partyData.permanentPartyData.GetSpriteNames());
        List<string> names = new List<string>(partyData.permanentPartyData.GetNames());
        List<string> baseStats = new List<string>(partyData.permanentPartyData.GetBaseStats());
        List<string> equipment = new List<string>(partyData.permanentPartyData.GetEquipment());
        spriteNames.AddRange(partyData.mainPartyData.GetSpriteNames());
        names.AddRange(partyData.mainPartyData.GetNames());
        baseStats.AddRange(partyData.mainPartyData.GetBaseStats());
        equipment.AddRange(partyData.mainPartyData.GetEquipment());
        for (int i = 0; i < spriteNames.Count; i++)
        {
            dummyActor.SetStatsFromString(baseStats[i]);
            dummyActor.ResetWeaponType();
            string[] equipData = equipment[i].Split("@");
            for (int j = 0; j < equipData.Length; j++)
            {
                dummyEquip.SetAllStats(equipData[j]);
                dummyEquip.EquipWeapon(dummyActor);
            }
            int passiveLevel = dummyActor.GetLevelFromPassive(spriteNames[i]);
            if (passiveLevel > 0 && passiveLevel < maxSkillLevel)
            {
                int RNG = Random.Range(0, (passiveLevel + 1) * (passiveLevel + 1));
                Debug.Log(names[i] + ";" + spriteNames[i] + ", Current Level: " + passiveLevel + ", Roll: " + RNG + "/" + ((passiveLevel + 1) * (passiveLevel + 1)));
                if (RNG == 0)
                {
                    dummyActor.SetLevelOfPassive(spriteNames[i], passiveLevel + 1);
                    dummyActor.ReloadPassives();
                    baseStats[i] = dummyActor.GetStats();
                    AddSkillUp(names[i], spriteNames[i]);
                }
            }
            string weaponType = dummyActor.GetWeaponType() + " User";
            if (weaponType == " User") { continue; } // If no weapon is equipped then continue.
            passiveLevel = dummyActor.GetLevelFromPassive(weaponType);
            if (passiveLevel <= 0)
            {
                dummyActor.AddPassiveSkill(weaponType, "1");
                dummyActor.ReloadPassives();
                baseStats[i] = dummyActor.GetStats();
                AddSkillUp(names[i], weaponType);
            }
            else if (passiveLevel < maxSkillLevel)
            {
                int RNG = Random.Range(0, (passiveLevel + 1) * (passiveLevel + 1));
                Debug.Log(names[i] + ";" + spriteNames[i] + ", Current Level: " + passiveLevel + ", Roll: " + RNG + "/" + ((passiveLevel + 1) * (passiveLevel + 1)));
                if (RNG == 0)
                {
                    dummyActor.SetLevelOfPassive(weaponType, passiveLevel + 1);
                    dummyActor.ReloadPassives();
                    baseStats[i] = dummyActor.GetStats();
                    AddSkillUp(names[i], weaponType);
                }
            }
        }
        if (actorNames.Count <= 0) { allSkillUps.Disable(); return; }
        allSkillUps.SetStatsAndData(actorNames, skillUpNames);
    }

    protected void AddSkillUp(string actor, string skill)
    {
        int indexOf = actorNames.IndexOf(actor);
        if (indexOf < 0)
        {
            actorNames.Add(actor);
            skillUpNames.Add(skill+"+1");
        }
        else
        {
            skillUpNames[indexOf] += ", "+skill+"+1";
        }
    }
}
