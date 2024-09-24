using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorPassives : MonoBehaviour
{
    public List<string> passiveSkills;
    public void ShowPassives()
    {
        for (int i = 0; i < passiveSkills.Count; i++)
        {
            Debug.Log(passiveSkills[i]);
        }
    }
    public string GetPassiveSkill(int index){return passiveSkills[index];}
    public void SetPassiveSkills(List<string> newSkills)
    {
        passiveSkills = newSkills;
        if (passiveSkills.Count == 0){return;}
        for (int i = passiveSkills.Count - 1; i >= 0; i--)
        {
            if (passiveSkills[i].Length <= 1){passiveSkills.RemoveAt(i);}
        }
    }
    public List<string> startBattlePassives;
    public List<string> GetStartBattlePassives(){return startBattlePassives;}
    public void AddStartBattlePassives(List<string> newSkills)
    {
        for (int i = 0; i < newSkills.Count; i++)
        {
            if (newSkills[i].Length <= 1){continue;}
            startBattlePassives.Add(newSkills[i]);
        }
    }
    public void SetStartBattlePassives(List<string> passives)
    {
        startBattlePassives = passives;
    }
    public List<string> startTurnPassives;
    public List<string> GetStartTurnPassives(){return startTurnPassives;}
    public void AddStartTurnPassive(string passiveName){startTurnPassives.Add(passiveName);}
    public void AddStartTurnPassives(List<string> newSkills)
    {
        for (int i = 0; i < newSkills.Count; i++)
        {
            if (newSkills[i].Length <= 1){continue;}
            AddStartTurnPassive(newSkills[i]);
        }
    }
    public void SetStartTurnPassives(List<string> passives){startTurnPassives = passives;}
    public List<string> endTurnPassives;
    public List<string> GetEndTurnPassives(){return endTurnPassives;}
    public void AddEndTurnPassive(string passiveName){endTurnPassives.Add(passiveName);}
    public void AddEndTurnPassives(List<string> newSkills)
    {
        for (int i = 0; i < newSkills.Count; i++)
        {
            if (newSkills[i].Length <= 1){continue;}
            AddEndTurnPassive(newSkills[i]);
        }
    }
    public void SetEndTurnPassives(List<string> passives){endTurnPassives = passives;}
    public List<string> attackingPassives;
    public List<string> GetAttackingPassives(){return attackingPassives;}
    public void AddAttackingPassive(string passiveName){attackingPassives.Add(passiveName);}
    public void AddAttackingPassives(List<string> newSkills)
    {
        for (int i = 0; i < newSkills.Count; i++)
        {
            if (newSkills[i].Length <= 1){continue;}
            AddAttackingPassive(newSkills[i]);
        }
    }
    public void SetAttackingPassives(List<string> passives){attackingPassives = passives;}
    public List<string> defendingPassives;
    public List<string> GetDefendingPassives(){return defendingPassives;}
    public void AddDefendingPassive(string passiveName){defendingPassives.Add(passiveName);}
    public void AddDefendingPassives(List<string> newSkills)
    {
        for (int i = 0; i < newSkills.Count; i++)
        {
            if (newSkills[i].Length <= 1){continue;}
            AddDefendingPassive(newSkills[i]);
        }
    }
    public void SetDefendingPassives(List<string> passives){defendingPassives = passives;}
    public List<string> takeDamagePassives;
    public List<string> GetTakeDamagePassives(){return takeDamagePassives;}
    public void AddTakeDamagePassive(string passiveName){takeDamagePassives.Add(passiveName);}
    public void AddTakeDamagePassives(List<string> newSkills)
    {
        for (int i = 0; i < newSkills.Count; i++)
        {
            if (newSkills[i].Length <= 1){continue;}
            AddTakeDamagePassive(newSkills[i]);
        }
    }
    public void SetTakeDamagePassives(List<string> passives){takeDamagePassives = passives;}
    public List<string> movingPassives;
    public List<string> GetMovingPassives(){return movingPassives;}
    public void AddMovingPassive(string passiveName){movingPassives.Add(passiveName);}
    public void AddMovingPassives(List<string> newSkills)
    {
        for (int i = 0; i < newSkills.Count; i++)
        {
            if (newSkills[i].Length <= 1){continue;}
            AddMovingPassive(newSkills[i]);
        }
    }
    public void SetMovingPassives(List<string> passives){movingPassives = passives;}
}
