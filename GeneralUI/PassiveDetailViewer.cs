using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveDetailViewer : MonoBehaviour
{
    protected virtual void Start()
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
    public SelectStatTextList passiveSelect;
    public void SelectPassive()
    {
        if (passiveSelect.GetSelected() < 0){return;}
        UpdatePassiveNames(passiveSelect.GetSelectedStat(), passiveSelect.GetSelectedData());
    }

    public List<string> ReturnAllPassiveInfo(List<string> groups, List<string> levels)
    {
        List<string> allInfo = new List<string>();
        for (int i = 0; i < groups.Count; i++)
        {
            int level = int.Parse(levels[i]);
            for (int j = 0; j < level; j++)
            {
                string passiveName = passiveNameLevels.GetMultiKeyValue(groups[i], (j+1).ToString());
                allInfo.Add(allPassives.ReturnValue(passiveName));
            }
        }
        return allInfo;
    }

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

    public string ReturnPassiveDetails(string newInfo)
    {
        if (!newInfo.Contains("|"))
        {
            return "";
        }
        string[] dataBlocks = newInfo.Split("|");
        string description = "";
        description += PassiveTiming(dataBlocks[0]);
        string[] effects = dataBlocks[4].Split(",");
        string[] effectSpecifics = dataBlocks[5].Split(",");
        for (int i = 0; i < effects.Length; i++)
        {
            description += PassiveEffect(effects[i], effectSpecifics[i], dataBlocks[3]);
        }
        string[] conditions = dataBlocks[1].Split(",");
        string[] specifics = dataBlocks[2].Split(",");
        for (int i = 0; i < conditions.Length; i++)
        {
            description += PassiveConditionText(conditions[i], specifics[i]);
            if (i < conditions.Length - 1)
            {
                description += " and";
            }
            else
            {
                description += ".";
            }
        }
        return description;
    }


    protected string PassiveTiming(string data)
    {
        switch (data)
        {
            case "BattleStart":
            return "At the start of each battle,";
            case "Start":
            return "At the start of each turn,";
            case "Moving":
            return "When moving,";
            case "Attacking":
            return "When attacking,";
            case "Defending":
            return "When being attacked,";
            case "TakeDamage":
            return "When receiving damage,";
            case "End":
            return "At the end of each turn,";
            case "Death":
            return "Upon death,";
        }
        return "";
    }

    protected string PassiveConditionText(string condition, string specifics)
    {
        switch (condition)
        {
            case "None":
                return "";
            case "AllyCount<":
                return " if there are less than "+specifics+" allies left";
            case "AllyCount>":
                return " if there are more than " + specifics + " allies left";
            case "AdjacentAllyCount>":
                return " if there are more than " + specifics + " allies adjacent";
            case "AdjacentAllyCount<":
                return " if there are less than " + specifics + " allies adjacent";
            case "AdjacentAllyCount>A":
                return " if there are more than " + specifics + " allies adjacent";
            case "AdjacentAllyCount<A":
                return " if there are less than " + specifics + " allies adjacent";
            case "AdjacentAllyCount>D":
                return " if there are more than " + specifics + " allies adjacent";
            case "AdjacentAllyCount<D":
                return " if there are less than " + specifics + " allies adjacent";
            case "Adjacent Ally":
                return " if another ally is adjacent";
            case "Adjacent Ally A":
                return " if the attacker has an adjacent ally";
            case "Adjacent Ally D":
                return " if the target has an adjacent ally";
            case "Adjacent Ally<>":
                return " if another ally is not adjacent";
            case "Adjacent Ally A<>":
                return " if the attacker has no adjacent ally";
            case "Adjacent Ally D<>":
                return " if the target has no adjacent ally";
            case "Adjacent Ally Sprite":
                return " if a " + specifics + " ally is adjacent";
            case "Adjacent Ally Sprite A":
                return " if a " + specifics + " ally is adjacent";
            case "Adjacent Ally Sprite D":
                return " if a " + specifics + " ally is adjacent";
            case "AdjacentEnemyCount>":
                return " if there are more than " + specifics + " enemies adjacent";
            case "AdjacentEnemyCount<":
                return " if there are less than " + specifics + " enemies adjacent";
            case "AdjacentEnemyCount>A":
                return " if there are more than " + specifics + " enemies adjacent";
            case "AdjacentEnemyCount<A":
                return " if there are less than " + specifics + " enemies adjacent";
            case "AdjacentEnemyCount>D":
                return " if there are more than " + specifics + " enemies adjacent";
            case "AdjacentEnemyCount<D":
                return " if there are less than " + specifics + " enemies adjacent";
            case "Direction":
                switch (specifics)
                {
                    case "Front":
                        return " if you are facing the front side of the enemy";
                    case "Back":
                        return " if you are facing the back side of the enemy";
                    case "Same":
                        return " if you are facing the back as the enemy";
                    case "Opposite":
                        return " if you are facing the enemy";
                }
                break;
            case "Elevation":
                switch (specifics)
                {
                    case "0":
                        return " if you are on low ground";
                    case "1":
                        return " if you are on ground level";
                    case "2":
                        return " if you are on high ground";
                }
                break;
            case "Elevation<>":
                switch (specifics)
                {
                    case "0":
                        return " if not on low ground";
                    case "1":
                        return " if not on ground level";
                    case "2":
                        return " if not on high ground";
                }
                break;
            case "Elevation<>A":
                switch (specifics)
                {
                    case "0":
                        return " if not on low ground";
                    case "1":
                        return " if not on ground level";
                    case "2":
                        return " if not on high ground";
                }
                break;
            case "Elevation<>D":
                switch (specifics)
                {
                    case "0":
                        return " if not on low ground";
                    case "1":
                        return " if not on ground level";
                    case "2":
                        return " if not on high ground";
                }
                break;
            case "ElevationA":
                switch (specifics)
                {
                    case "0":
                        return " if on low ground";
                    case "1":
                        return " if on ground level";
                    case "2":
                        return " if on high ground";
                }
                break;
            case "ElevationD":
                switch (specifics)
                {
                    case "0":
                        return " if on low ground";
                    case "1":
                        return " if on ground level";
                    case "2":
                        return " if on high ground";
                }
                break;
            case "Elevation<":
                return " if the attacker is on lower elevation";
            case "Elevation>":
                return " if the attacker is on higher elevation";
            case "Elevation=":
                return " if the attacker is on equal elevation";
            case "Distance":
                return " if within " + specifics + " tile(s)";
            case "Distance>":
                return " if at least " + specifics + " tile(s) away";
            //case "Type":
            //return " if the damage is "+damageTypesReturnValue(dummyPassiveconditionSpecifics)+"";
            case "Health":
                return " if health is "+specifics;
            case "HealthD":
                return " if the target's health is "+specifics;
            case "HealthA":
                return " if health is "+specifics;
            case "Tile":
                return " if on a " + specifics + " tile";
            case "Tile<>":
                return " if not on a " + specifics + " tile";
            case "TileA":
                return " if on a " + specifics + " tile";
            case "Tile<>A":
                return " if not on a " + specifics + " tile";
            case "Weapon":
                return " if a " + specifics + " is equipped";
            case "Weather":
                return " if the weather is " + specifics + "";
            case "Weather<>":
                return " if the weather is not " + specifics + "";
            case "Time":
                return " if the time of day is " + specifics + "";
            case "Time<>":
                return " if the time of day is not " + specifics + "";
            case "MoveType":
                return " if movement type is " + specifics;
            case "MoveType<>":
                return " if movement type is not " + specifics;
            case "MoveType<>A":
                return " if your movement type is not " + specifics;;
            case "MoveType<>D":
                return " if the target's movement type is not " + specifics;
            case "MoveTypeA":
                return " if your movement type is " + specifics;
            case "MoveTypeD":
                return " if the target's movement type is " + specifics;
            case "MentalState":
                return " if " + specifics + "";
            case "MentalStateA":
                return " if " + specifics + "";
            case "MentalStateD":
                return " if " + specifics + "";
            case "Status":
                return " if you have " + specifics + " status";
            case "StatusA":
                return " if you have " + specifics + " status";
            case "StatusD":
                return " if the target has " + specifics + " status";
            case "RangeD>":
                return " if the target's attack range is greater than " + specifics + "";
            case "RangeD<":
                return " if the target's attack range is less than " + specifics + "";
            case "RangeA>":
                return " if your attack range is greater than " + specifics + "";
            case "RangeA<":
                return " if your attack range is less than " + specifics + "";
            case "Round":
                switch (specifics)
                {
                    case "Even":
                        return " every other round";
                    case "Odd":
                        return " at the start of the first round and every other round";
                }
                break;
            case "Passive":
                return " if you do have the " + specifics + " passive";
            case "Passive<>":
                return " if you do not have the " + specifics + " passive";
            case "PassiveLevelsD>":
                return " if the target has more than " + specifics + " passive levels";
            case "PassiveLevelsD<":
                return " if the target has less than " + specifics + " passive levels";
            case "PassiveLevelsA>":
                return " if you have more than " + specifics + " passive levels";
            case "PassiveLevelsA<":
                return " if you have less than " + specifics + " passive levels";
            case "Counter":
                return " if your counter is greater than " + specifics;
            case "Team":
                if (specifics == "Same")
                {
                    return " if you are on the same team";
                }
                return " if you are not on the same team";
            case "Direction<>D":
                return " if not attacking" + RelativeDirectionDescriptions(specifics);
            case "DirectionD":
                return " if attacking" + RelativeDirectionDescriptions(specifics);
            case "ElementD":
                return " if the target's element is "+specifics;
            case "Element<>D":
                return " if the target's element is not "+specifics;
            case "ElementA":
                return " if the attacker's element is "+specifics;
            case "Element<>A":
                return " if the attacker's element is not "+specifics;
            case "SpeciesD":
                return " if the target is " + specifics;
            case "Species<>D":
                return " if the target is not " + specifics;
            case "SpeciesA":
                return " if the attacker is " + specifics;
            case "Species<>A":
                return " if the attacker is not " + specifics;
        }
        return "";
    }

    protected string AdjustSpecificsText(string specifics)
    {
        switch (specifics)
        {
            case "Defense":
                return "your defense value";
            case "Attack":
                return "your attack value";
        }
        return specifics;
    }

    protected string PassiveEffect(string effect, string specifics, string target)
    {
        specifics = AdjustSpecificsText(specifics);
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
                return " increase maximum health of " + target + " by " + specifics;
            case "BaseHealth%":
                return " increase maximum health of " + target + " by " + specifics + "%";
            case "MaxHealth%":
                return " change maximum health of " + target + " by " + specifics + "%";
            case "BaseEnergy%":
                return " change base energy of " + target + " by " + specifics + "%";
            case "Movement":
                return " " + target + " gain " + specifics + " movement";
            case "Skill":
                return " gain the " + specifics + " skill";
            case "Temporary Skill":
                return " gain the " + specifics + " skill once";
            case "Status":
                return " inflict " + specifics+" on "+target;
            case "RemoveStatus":
                return " remove all " + specifics + " status effects from " + target;
            case "Health":
                return " " + target + " regain up to " + specifics + " health";
            case "Health%":
                return " " + target + " regain up to " + specifics + "% health";
            case "Attack%":
                return " increase attack of " + target + " by " + specifics + "%";
            case "BaseAttack%":
                return " increase base attack of " + target + " by " + specifics + "%";
            case "Defense%":
                return " increase defense of " + target + " by " + specifics + "%";
            case "MoveType":
                return " change movement type of " + target + " to " + specifics;
            case "AttackRange":
                return " increase Attack Range of " + target + " by up to " + specifics;
            case "BaseSpeed":
                return " increase Base Speed of " + target + " by up to " + specifics;
            case "TempAttack%":
                return " change attack of " + target + " by " + specifics + "%, until the end of next turn";
            case "TempAttack":
                return " change attack of " + target + " by " + specifics + ", until the end of next turn";
            case "TempDefense%":
                return " change defense of " + target + " by " + specifics + "%, until the end of next turn";
            case "TempDefense":
                return " change defense of " + target + " by " + specifics + ", until the end of next turn";
            case "TempHealth%":
                return " " + target + " gain a shield that absorbs damage equal to " + specifics + "% of max health, until the end of next turn";
            case "TempHealth":
                return " " + target + " gain a shield that absorbs damage equal to " + specifics + ", until the end of next turn";
            case "TerrainEffect":
                return " create " + specifics;
            case "Tile":
                return " create " + specifics;
            case "Map":
                return " create " + specifics;
            case "MentalState":
                return " change mental state to " + specifics;
            case "Amnesia":
                return " make " + target + " forget 1 active skill";
            case "Active":
                return " use " + specifics;
            case "Death":
                return " die";
            case "CounterAttack":
                return " gain " + specifics + " counter attacks";
        }
        return " increase " + effect + " of " + target + " by " + specifics;
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

    protected string RelativeDirectionDescriptions(string specifics)
    {
        switch (specifics)
        {
            case "0":
                return " from the front";
            case "1":
                return " from the front right";
            case "2":
                return " from the back right";
            case "3":
                return " from the back";
            case "4":
                return " from the back left";
            case "5":
                return " from the front left";
        }
        return "";
    }
}
