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
        for (int i = 0; i < passiveInfoTexts.Count; i++)
        {
            passiveInfoTexts[i].SetTextSize(textSize);
        }
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
    public List<GameObject> objects;
    public List<StatTextText> passiveInfoTexts;

    public void UpdatePassiveNames(string group, string newLevel)
    {
        SetPassiveGroupName(group);
        SetPassiveLevel(newLevel);
        panel.SetActive(true);
        for (int i = 0; i < passiveInfoTexts.Count; i++)
        {
            passiveInfoTexts[i].Reset();
        }
        int level = int.Parse(passiveLevel);
        passiveNames.Clear();
        passiveInfo.Clear();
        passiveDescription.Clear();
        // Max level for a passive is 4.
        for (int i = 1; i < Mathf.Min(5, level+1); i++)
        {
            passiveNames.Add(passiveNameLevels.GetMultiKeyValue(passiveGroupName, i.ToString()));
            passiveInfo.Add(allPassives.ReturnValue(passiveNames[i-1]));
        }
        for (int i = 0; i < passiveNames.Count; i++)
        {
            passiveDescription.Add(ReturnPassiveDetails(passiveInfo[i]));
            passiveInfoTexts[i].SetStatText(passiveNames[i]);
            passiveInfoTexts[i].SetText(passiveDescription[i]);
        }
        passiveGroupText.SetStatText(passiveGroupName);
        passiveGroupText.SetText(passiveLevel);
    }

    public string ReturnPassiveDetails(string passiveInfo)
    {
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
            case "Adjacent Ally":
                return " if another ally is adjacent.";
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
            case "Weapon":
                return " if a " + specifics + " is equipped.";
            case "Weather":
                return " if the weather is "+specifics+".";
            case "Time":
                return " if the time of day is "+specifics+".";
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
                return " inflict " + specifics;
            case "RemoveStatus":
                return " remove all " + specifics + " status effects";
            case "Health":
                return " regain up to " + specifics + " health";
            case "Health%":
                return " regain up to " + specifics + "% health";
            case "Attack%":
                return " increase attack by " + specifics + "%";
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
