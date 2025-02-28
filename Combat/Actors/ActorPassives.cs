using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorPassives : MonoBehaviour
{
    // Usually only get new passives based on equipment at the start of battle, before you start organizing passives.
    // Can also use to this learn new permanent passive skills.
    public void AddPassiveSkill(string skillName, string skillLevel)
    {
        int indexOf = passiveSkills.IndexOf(skillName);
        if (indexOf < 0)
        {
            passiveSkills.Add(skillName);
            passiveLevels.Add(skillLevel);
        }
        else
        {
            int newLevel = int.Parse(passiveLevels[indexOf])+int.Parse(skillLevel);
            passiveLevels[indexOf] = newLevel.ToString();
        }
    }
    public List<string> passiveSkills;
    public void SetPassiveSkills(List<string> newSkills)
    {
        passiveSkills = newSkills;
        if (passiveSkills.Count == 0){return;}
        for (int i = passiveSkills.Count - 1; i >= 0; i--)
        {
            if (passiveSkills[i].Length <= 1){passiveSkills.RemoveAt(i);}
        }
    }
    public List<string> GetPassiveSkills(){return passiveSkills;}
    
    public List<string> passiveLevels;
    public void SetPassiveLevels(List<string> newLevels)
    {
        passiveLevels = newLevels;
        if (passiveLevels.Count == 0){return;}
        for (int i = passiveLevels.Count - 1; i >= 0; i--)
        {
            if (passiveLevels[i].Length < 1){passiveLevels.RemoveAt(i);}
        }
    }
    public List<string> GetPassiveLevels(){return passiveLevels;}
    public void ShowPassives()
    {
        for (int i = 0; i < passiveSkills.Count; i++)
        {
            Debug.Log(passiveSkills[i]);
        }
    }
    public string GetPassiveSkill(int index){return passiveSkills[index];}    
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
        startBattlePassives = new List<string>(passives);
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
    public void SetStartTurnPassives(List<string> passives){startTurnPassives = new List<string>(passives);}
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
    public void SetEndTurnPassives(List<string> passives){endTurnPassives = new List<string>(passives);}
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
    public void SetAttackingPassives(List<string> passives){attackingPassives = new List<string>(passives);}
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
    public void SetDefendingPassives(List<string> passives){defendingPassives = new List<string>(passives);}
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
    public void SetTakeDamagePassives(List<string> passives){takeDamagePassives = new List<string>(passives);}
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
    public void SetMovingPassives(List<string> passives){movingPassives = new List<string>(passives);}
    public List<string> deathPassives;
    public List<string> GetDeathPassives(){return deathPassives;}
    public void AddDeathPassive(string passiveName){deathPassives.Add(passiveName);}
    public void AddDeathPassives(List<string> newSkills)
    {
        for (int i = 0; i < newSkills.Count; i++)
        {
            if (newSkills[i].Length <= 1){continue;}
            AddDeathPassive(newSkills[i]);
        }
    }
    public void SetDeathPassives(List<string> passives){deathPassives = new List<string>(passives);}
}
