using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSheet", menuName = "ScriptableObjects/CharacterSheet", order = 1)]
public class CharacterStatSheet : ScriptableObject
{
    public string resetData;
    public void Reset()
    {
        string[] blocks = resetData.Split("#");
    }
    public string characterName;
    public void SetName(string newName){characterName = newName;}
    public string GetName()
    {
        if (characterName.Length < 1){return characterType;}
        return characterName;
    }
    public string characterType;
    public int weightClass;
    public int maxHealth;
    public int currentHealth;
    public int maxEnergy;
    public int currentEnergy;
    public int attack;
    public int attackRange;
    public int defense;
    public int moveSpeed;
    public string moveType;
    public List<string> activeSkills;
    public List<string> passiveSkills;

    public List<string> ReturnStats()
    {
        List<string> stats = new List<string>();
        stats.Add(maxHealth.ToString());
        stats.Add(currentHealth.ToString());
        stats.Add(maxEnergy.ToString());
        stats.Add(currentEnergy.ToString());
        stats.Add(attack.ToString());
        stats.Add(defense.ToString());
        stats.Add(moveSpeed.ToString());
        return stats;
    }

    public List<string> ReturnAllStats()
    {
        List<string> stats = new List<string>();
        stats.Add(maxHealth.ToString());
        stats.Add(currentHealth.ToString());
        stats.Add(maxEnergy.ToString());
        stats.Add(currentEnergy.ToString());
        stats.Add(attack.ToString());
        stats.Add(attackRange.ToString());
        stats.Add(defense.ToString());
        stats.Add(moveSpeed.ToString());
        stats.Add(moveType.ToString());
        stats.Add(weightClass.ToString());
        return stats;
    }
}
