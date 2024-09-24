using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorPassives : MonoBehaviour
{
    public List<string> passiveSkills;
    public string GetPassiveSkill(int index){return passiveSkills[index];}
    public void SetPassiveSkills(List<string> newSkills){passiveSkills = newSkills;}
    public List<string> startBattlePassives;
    public void AddStartBattlePassives(List<string> newSkills)
    {
        for (int i = 0; i < newSkills.Count; i++)
        {
            if (newSkills[i].Length <= 1){continue;}
            startBattlePassives.Add(newSkills[i]);
        }
    }
    public void SetStartBattlePassives(List<string> passives){startBattlePassives = passives;}
    public List<string> startTurnPassives;
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
