using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveDetailViewer : MonoBehaviour
{
    void Start()
    {
        UpdateTextSize();
    }
    public int textSize;
    public void UpdateTextSize()
    {
        passiveStatTextList.UpdateTextSize();
    }
    public string passiveGroupName;
    public StatTextText passiveGroupText;
    public void SetPassiveGroupName(string newGroup){passiveGroupName = newGroup;}
    public string passiveLevel;
    public void SetPassiveLevel(string newLevel){passiveLevel = newLevel;}
    public List<string> passiveNames;
    public List<string> passiveInfo;
    public List<string> passiveDescription;
    public MultiKeyStatDatabase passiveNameLevels;
    public StatDatabase allPassives;
    public GameObject panel;
    public void DisablePanel(){panel.SetActive(false);}
    public StatTextList passiveStatTextList;

    public void UpdatePassiveNames(string group, string newLevel)
    {
        SetPassiveGroupName(group);
        SetPassiveLevel(newLevel);
        panel.SetActive(true);
        int level = int.Parse(passiveLevel);
        passiveNames.Clear();
        passiveInfo.Clear();
        passiveDescription.Clear();
        for (int i = 0; i < level; i++)
        {
            passiveNames.Add(passiveNameLevels.GetMultiKeyValue(passiveGroupName, (i+1).ToString()));
            passiveInfo.Add(allPassives.ReturnValue(passiveNames[i]));
            passiveDescription.Add(ReturnPassiveDetails(passiveInfo[i]));
        }
        passiveStatTextList.SetStatsAndData(passiveNames, passiveDescription);
        passiveGroupText.SetStatText(passiveGroupName);
        passiveGroupText.SetText(passiveLevel);
    }

    public string ReturnPassiveDetails(string passiveInfo)
    {
        if (!passiveInfo.Contains("|"))
        {
            return "";
        }
        string[] dataBlocks = passiveInfo.Split("|");
        string description = "";
        description += PassiveTiming(dataBlocks[0]);
        description += PassiveEffect(dataBlocks[4], dataBlocks[5], dataBlocks[3]);
        description += PassiveConditionText(dataBlocks[1], dataBlocks[2]);
        return description;
    }

    protected string PassiveTiming(string data)
    {
        switch (data)
        {
            case "BattleStart":
            return "At the start of each battle";
            case "Start":
            return "At the start of each turn";
            case "Moving":
            return "When moving";
            case "Attacking":
            return "When attacking";
            case "Defending":
            return "When being attacked";
            case "TakeDamage":
            return "When receiving damage";
            case "End":
            return "At the end of each turn";
            case "Death":
            return "Upon death";
        }
        return "";
    }

    protected string PassiveConditionText(string condition, string specifics)
    {
        switch (condition)
        {
            case "None":
                return ".";
            case "AllyCount<":
                return " if there are less than "+specifics+" allies left.";
            case "AllyCount>":
                return " if there are more than " + specifics + " allies left.";
            case "Adjacent Ally":
                return " if another ally is adjacent.";
            case "Adjacent Ally A":
                return " if another ally is adjacent.";
            case "Adjacent Ally D":
                return " if another ally is adjacent.";
            case "Adjacent Ally Sprite":
                return " if a " + specifics + " ally is adjacent.";
            case "Adjacent Ally Sprite A":
                return " if a " + specifics + " ally is adjacent.";
            case "Adjacent Ally Sprite D":
                return " if a " + specifics + " ally is adjacent.";
            case "Direction":
                switch (specifics)
                {
                    case "Front":
                        return " if you are facing the front side of the enemy.";
                    case "Back":
                        return " if you are facing the back side of the enemy.";
                    case "Same":
                        return " if you are facing the back as the enemy.";
                    case "Opposite":
                        return " if you are facing the enemy.";
                }
                break;
            case "Distance":
                return " if within " + specifics + " tile(s).";
            case "Distance>":
                return " if at least " + specifics + " tile(s) away.";
            //case "Type":
            //return " if the damage is "+damageTypes.ReturnValue(dummyPassive.conditionSpecifics)+".";
            case "Health":
                switch (specifics)
                {
                    case "<Half":
                        return " if health is <50%.";
                    case "Full":
                        return " if health is full.";
                }
                break;
            case "Tile":
                return " if on a " + specifics + " tile.";
            case "Tile<>":
                return " if not on a " + specifics + " tile.";
            case "Weapon":
                return " if a " + specifics + " is equipped.";
            case "Weather":
                return " if the weather is " + specifics + ".";
            case "Time":
                return " if the time of day is " + specifics + ".";
            case "MoveType":
                return " if " + specifics + ".";
            case "MoveType<>":
                return " if not " + specifics + ".";
            case "MentalState":
                return " if " + specifics + ".";
            case "MentalStateA":
                return " if " + specifics + ".";
            case "MentalStateD":
                return " if " + specifics + ".";
            case "Status":
                return " if you have " + specifics + " status.";
            case "StatusA":
                return " if you have " + specifics + " status.";
            case "StatusD":
                return " if the target has " + specifics + " status.";
            case "RangeD>":
                return " if the target's attack range is greater than " + specifics + ".";
            case "RangeD<":
                return " if the target's attack range is less than " + specifics + ".";
            case "RangeA>":
                return " if your attack range is greater than " + specifics + ".";
            case "RangeA<":
                return " if your attack range is less than " + specifics + ".";
            case "Round":
                switch (specifics)
                {
                    case "Even":
                        return " every other round.";
                    case "Odd":
                        return " at the start of the first round and every other round.";
                }
                break;
        }
        return ".";
    }

    protected string PassiveEffect(string effect, string specifics, string target)
    {
        switch (effect)
        {
            case "Increase":
                return IncreaseDecreaseTargetSpecifics(effect, specifics, target);
            case "Increase%":
                return IncreaseDecreaseTargetSpecifics(effect, specifics, target);
            case "Decrease":
                return IncreaseDecreaseTargetSpecifics(effect, specifics, target);
            case "Decrease%":
                return IncreaseDecreaseTargetSpecifics(effect, specifics, target);
            case "BaseHealth":
                return " increase maximum health by " + specifics;
            case "Movement":
                return " gain " + specifics + " movement";
            case "Skill":
                return " gain the " + specifics + " skill";
            case "Status":
                return " inflict " + specifics+" on "+target;
            case "RemoveStatus":
                return " remove all " + specifics + " status effects";
            case "Health":
                return " regain up to " + specifics + " health";
            case "Health%":
                return " regain up to " + specifics + "% health";
            case "Attack%":
                return " increase attack by " + specifics + "%";
            case "BaseAttack%":
                return " increase base attack by " + specifics + "%";
            case "Defense%":
                return " increase defense by " + specifics + "%";
            case "MoveType":
                return " change movement type to " + specifics;
            case "AttackRange":
                return " increase Attack Range by up to " + specifics;
            case "BaseSpeed":
                return " increase Base Speed by up to " + specifics;
            case "TempAttack%":
                return " change attack by " + specifics + "%, until the end of next turn";
            case "TempAttack":
                return " change attack by " + specifics + ", until the end of next turn";
            case "TempDefense%":
                return " change defense by " + specifics + "%, until the end of next turn";
            case "TempDefense":
                return " change defense by " + specifics + ", until the end of next turn";
            case "TempHealth%":
                return " gain a shield that absorbs damage equal to " + specifics + "% of max health, until the end of next turn";
            case "TempHealth":
                return " gain a shield that absorbs damage equal to " + specifics + ", until the end of next turn";
            case "TerrainEffect":
                return " create " + specifics;
            case "Tile":
                return " create " + specifics;
            case "Map":
                return " create " + specifics;
            case "MentalState":
                return " change mental state to " + specifics;
            case "Amnesia":
                return " make "+target+" forget 1 active skill";
            case "Active":
                return " use " + specifics;
        }
        return " increase "+effect+" by "+specifics;
    }

    protected string IncreaseDecreaseTargetSpecifics(string effect, string specifics, string target)
    {
        switch (target)
        {
            case "MoveCost":
            switch (effect)
            {
                case "Increase":
                return " expend "+specifics+" more movement";
                case "Decrease":
                return " expend "+specifics+" less movement";
            }
            return "";
            case "BaseDamage":
            switch (effect)
            {
                case "Increase%":
                return " increase damage dealt by "+specifics+"%";
                case "Decrease%":
                return " decrease damage dealt by "+specifics+"%";
            }
            return " "+effect+" damage dealt by "+specifics;
            case "Damage%":
            return " "+effect+" damage multipler by "+specifics+"%";
        }
        return " "+effect+" "+target+" by "+specifics;
    }
}
