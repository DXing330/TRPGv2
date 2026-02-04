using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveDetailViewer : MonoBehaviour
{
    public string MapTilePassives(BattleMap map, int tileNumber)
    {
        string tileDetails = map.mapInfo[tileNumber];
        string[] tilePassives = map.terrainPassives.ReturnValue(tileDetails).Split("!");
        for (int i = 0; i < tilePassives.Length; i++)
        {
            if (tilePassives[i].Length < 6){continue;}
            tileDetails += "\n";
            tileDetails += ReturnPassiveDetails(tilePassives[i]);
        }
        return tileDetails;
    }
    public string MapTEffectPassives(BattleMap map, int tileNumber)
    {
        string tileDetails = map.terrainEffectTiles[tileNumber];
        if (tileDetails == ""){return "";}
        string[] tilePassives = map.terrainEffectData.ReturnValue(tileDetails).Split("!");
        for (int i = 0; i < tilePassives.Length; i++)
        {
            if (tilePassives[i].Length < 6){continue;}
            tileDetails += "\n";
            tileDetails += ReturnPassiveDetails(tilePassives[i]);
        }
        return tileDetails;
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
    public StatDatabase runePassives;
    public void ViewRunePassives(TacticActor actor)
    {
        List<string> rPassives = actor.GetRunePassives();
        panel.SetActive(true);
        passiveNames.Clear();
        passiveInfo.Clear();
        passiveDescription.Clear();
        passiveGroupText.SetStatText("Rune Passives");
        passiveGroupText.SetText("");
        for (int i = 0; i < rPassives.Count; i++)
        {
            passiveNames.Add(rPassives[i]);
            passiveDescription.Add(ReturnPassiveDetails(runePassives.ReturnValue(rPassives[i])));
        }
        passiveStatTextList.SetStatsAndData(passiveNames, passiveDescription);
    }
    public string GetRunePassiveString(string runeName)
    {
        string runePassive = runePassives.ReturnValue(runeName);
        return ReturnPassiveDetails(runePassive);
    }
    public void ViewRunePassive(string runeName)
    {
        SetPassiveGroupName(runeName);
        SetPassiveLevel("1");
        panel.SetActive(true);
        passiveNames.Clear();
        passiveInfo.Clear();
        passiveDescription.Clear();
        passiveNames.Add(runeName);
        passiveInfo.Add(runePassives.ReturnValue(passiveNames[0]));
        passiveDescription.Add(ReturnPassiveDetails(passiveInfo[0]));
        passiveStatTextList.SetStatsAndData(passiveNames, passiveDescription);
        passiveGroupText.SetStatText(passiveGroupName);
        passiveGroupText.SetText(passiveLevel);
    }
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

    public string ReturnSpecificPassiveLevelEffect(string group, int level)
    {
        string passiveName = passiveNameLevels.GetMultiKeyValue(group, (level).ToString());
        return ReturnPassiveDetails(allPassives.ReturnValue(passiveName));
    }

    // Used to make the display popup and the text equal to the passive details.
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
    
    public void ViewCustomPassives(TacticActor actor)
    {
        List<string> customPassives = actor.GetCustomPassives();
        panel.SetActive(true);
        passiveNames.Clear();
        passiveInfo.Clear();
        passiveDescription.Clear();
        passiveGroupText.SetStatText("Custom Passives");
        passiveGroupText.SetText(customPassives.Count.ToString());
        for (int i = 0; i < customPassives.Count; i++)
        {
            passiveNames.Add("Custom "+(i+1));
            passiveDescription.Add(ReturnPassiveDetails(customPassives[i]));
        }
        passiveStatTextList.SetStatsAndData(passiveNames, passiveDescription);
    }

    public List<string> ReturnAllPassiveDetails(TacticActor actor)
    {
        List<string> allDetails = new List<string>();
        // Loop through passives and levels.
        List<string> allAPassives = actor.GetPassiveSkills();
        List<string> allPassiveLevels = actor.GetPassiveLevels();
        for (int i = 0; i < allAPassives.Count; i++)
        {
            int level = int.Parse(allPassiveLevels[i]);
            if (level <= 0){continue;}
            for (int j = 1; j < level + 1; j++)
            {
                allDetails.Add(ReturnPassiveDetails(allPassives.ReturnValue(passiveNameLevels.GetMultiKeyValue(allAPassives[i], j.ToString()))));
            }
        }
        // Loop through custom passives.
        allAPassives = actor.GetCustomPassives();
        for (int i = 0; i < allAPassives.Count; i++)
        {
            allDetails.Add(ReturnPassiveDetails(allAPassives[i]));
        }
        return allDetails;
    }

    public string ReturnAuraDetails(AuraEffect aura)
    {
        string description = aura.GetAuraName() + " (" + aura.GetTeamTarget() + ") :\n";
        description += PassiveEffect(aura.effect, aura.effectSpecifics, aura.target);
        description += PassiveConditionText(aura.condition, aura.conditionSpecifics);
        return description;
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
            if (i < effects.Length - 1)
            {
                description += " and";
            }
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
            case "BS":
            return "At the start of each battle,";
            case "Start":
            return "At the start of each turn,";
            case "Moving":
            return "When moving,";
            case "Attack":
            return "When attacking,";
            case "Defend":
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
            case "Killing":
                return " if attack is greater than the sum of the target's health and defense";
            case "AllyCount<":
                return " if there are less than " + specifics + " allies left";
            case "AllyCount>":
                return " if there are more than " + specifics + " allies left";
            case "AllyCount<A":
                return " if the attacker has less than " + specifics + " allies left";
            case "AllyCount>A":
                return " if the attacker has more than " + specifics + " allies left";
            case "AllyCount<D":
                return " if the target has less than " + specifics + " allies left";
            case "AllyCount>D":
                return " if the target has more than " + specifics + " allies left";
            case "Ally<Enemy":
                return " if there are less allies than enemies";
            case "Ally>Enemy":
                return " if there are more allies than enemies";
            case "AllyEqualsEnemy":
                return " if there are equal allies and enemies";
            case "Ally<EnemyA":
                return " if the attacker has less allies than enemies";
            case "Ally>EnemyA":
                return " if the attacker has more allies than enemies";
            case "AllyEqualsEnemyA":
                return " if the attacker has equal allies and enemies";
            case "EnemyCount<":
                return " if there are less than " + specifics + " enemies left";
            case "EnemyCount>":
                return " if there are more than " + specifics + " enemies left";
            case "EnemyCount<A":
                return " if the attacker has less than " + specifics + " enemies left";
            case "EnemyCount>A":
                return " if the attacker has more than " + specifics + " enemies left";
            case "EnemyCount<D":
                return " if the target has less than " + specifics + " enemies left";
            case "EnemyCount>D":
                return " if the target has more than " + specifics + " enemies left";
            case "AdjacentAllyCount>":
                return " if there are more than " + specifics + " allies adjacent";
            case "AdjacentAllyCount<":
                return " if there are less than " + specifics + " allies adjacent";
            case "AdjacentAllyCount>A":
                return " if the attacker has more than " + specifics + " allies adjacent";
            case "AdjacentAllyCount<A":
                return " if the attacker has less than " + specifics + " allies adjacent";
            case "AdjacentAllyCount>D":
                return " if the target has more than " + specifics + " allies adjacent";
            case "AdjacentAllyCount<D":
                return " if the target has less than " + specifics + " allies adjacent";
            case "AdjacentAlly":
                return " if another ally is adjacent";
            case "AdjacentAllyA":
                return " if the attacker has an adjacent ally";
            case "AdjacentAllyD":
                return " if the target has an adjacent ally";
            case "AdjacentAlly<>":
                return " if another ally is not adjacent";
            case "AdjacentAlly<>A":
                return " if the attacker has no adjacent ally";
            case "AdjacentAlly<>D":
                return " if the target has no adjacent ally";
            case "AdjacentAllySprite":
                return " if a " + specifics + " ally is adjacent";
            case "AdjacentAllySpriteA":
                return " if a " + specifics + " ally is adjacent";
            case "AdjacentAllySpriteD":
                return " if a " + specifics + " ally is adjacent to the target";
            case "AdjacentEnemyCount>":
                return " if there are more than " + specifics + " enemies adjacent";
            case "AdjacentEnemyCount<":
                return " if there are less than " + specifics + " enemies adjacent";
            case "AdjacentEnemyCount>A":
                return " if there are more than " + specifics + " enemies adjacent to the attacker";
            case "AdjacentEnemyCount<A":
                return " if there are less than " + specifics + " enemies adjacent to the attacker";
            case "AdjacentEnemyCount>D":
                return " if there are more than " + specifics + " enemies adjacent to the target";
            case "AdjacentEnemyCount<D":
                return " if there are less than " + specifics + " enemies adjacent to the target";
            case "Direction":
                switch (specifics)
                {
                    case "Front":
                        return " if the attacker is facing the front side of the target";
                    case "Back":
                        return " if the attacker is facing the back side of the target";
                    case "Same":
                        return " if the attacker is facing the back of the target";
                    case "Opposite":
                        return " if the attacker is facing the target";
                }
                break;
            case "Direction<>":
                switch (specifics)
                {
                    case "Front":
                        return " if the attacker is not facing the front side of the target";
                    case "Back":
                        return " if the attacker is not facing the back side of the target";
                    case "Same":
                        return " if the attacker is not facing the back of the target";
                    case "Opposite":
                        return " if the attacker is not facing the target";
                }
                break;
            case "Elevation<A":
                return " if the attacker is on lower elevation";
            case "Elevation>A":
                return " if the attacker is on higher elevation";
            case "ElevationEqualsA":
                return " if the attacker is on equal elevation";
            case "Elevation<":
                return " if tile elevation is less than " + specifics;
            case "Elevation>":
                return " if tile elevation is more than " + specifics;
            case "Elevation":
                return " if tile elevation is equal to " + specifics;
            case "Distance":
                return " if within " + specifics + " tile(s)";
            case "Distance>":
                return " if more than " + specifics + " tile(s) away";
            //case "Type":
            //return " if the damage is "+damageTypesReturnValue(dummyPassiveconditionSpecifics)+"";
            case "Health":
                return " if health is "+specifics;
            case "HealthD":
                return " if the target's health is "+specifics;
            case "HealthA":
                return " if the attacker's health is "+specifics;
            case "Energy":
                return " if energy is "+specifics;
            case "EnergyD":
                return " if the target's energy is "+specifics;
            case "EnergyA":
                return " if the attacker's energy is "+specifics;
            case "Tile":
                return " if on a " + specifics + " tile";
            case "Tile<>":
                return " if not on a " + specifics + " tile";
            case "TileD":
                return " if the target is on a " + specifics + " tile";
            case "Tile<>D":
                return " if the target is not on a " + specifics + " tile";
            case "TileEffect":
                return " if on a " + specifics + " tile";
            case "TileEffect<>":
                return " if not on a " + specifics + " tile";
            case "TileA":
                return " if the attacker is on a " + specifics + " tile";
            case "Tile<>A":
                return " if the attacker is not on a " + specifics + " tile";
            case "TileEffectA":
                return " if the attacker is on a " + specifics + " tile";
            case "TileEffect<>A":
                return " if the attacker is not on a " + specifics + " tile";
            case "TileEffectD":
                return " if the target is on a " + specifics + " tile";
            case "TileEffect<>D":
                return " if the target is not on a " + specifics + " tile";
            case "Weapon":
                return " if a " + specifics + " is equipped";
            case "WeaponA":
                return " if the attacker has a " + specifics + " weapon equipped";
            case "WeaponD":
                return " if the target has a " + specifics + " weapon equipped";
            case "Weapon<>":
                return " if not weapon is equipped";
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
                return " if the attacker's movement type is not " + specifics;;
            case "MoveType<>D":
                return " if the target's movement type is not " + specifics;
            case "MoveTypeA":
                return " if the attacker's movement type is " + specifics;
            case "MoveTypeD":
                return " if the target's movement type is " + specifics;
            case "MentalState":
                return " if " + specifics + "";
            case "MentalStateA":
                return " if the attacker is " + specifics + "";
            case "MentalStateD":
                return " if the target is " + specifics + "";
            case "Status":
                return " if you have " + specifics + " status";
            case "StatusCount>":
                return " if you have more than " + specifics + " status effects";
            case "Status<>":
                return " if you do not have " + specifics + " status";
            case "StatusA":
                return " if the attacker has " + specifics + " status";
            case "StatusD":
                return " if the target has " + specifics + " status";
            case "Range>":
                return " if attack range is greater than " + specifics;
            case "Range<":
                return " if attack range is less than " + specifics;
            case "RangeD>":
                return " if the target's attack range is greater than " + specifics;
            case "RangeD<":
                return " if the target's attack range is less than " + specifics;
            case "RangeA>":
                return " if the attacker's attack range is greater than " + specifics;
            case "RangeA<":
                return " if the attacker's attack range is less than " + specifics;
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
                return " if the attacker's have more than " + specifics + " passive levels";
            case "PassiveLevelsA<":
                return " if the attacker's have less than " + specifics + " passive levels";
            case "Counter":
                return " if your counter is greater than " + specifics;
            case "CounterAttack":
                return " if a counter attack is available";
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
            case "Element":
                return " if " + specifics + " element";
            case "Element<>":
                return " if not " + specifics + " element";
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
            case "TargetD:":
                return " if the target is targeting the attacker";
            case "TargetD<>":
                return " if the target is not targeting the attacker";
            case "AverageHP>":
                return " if your health is greater than the average health of the battle";
            case "AverageHP<":
                return " if your health is less than the average health of the battle";
            case "AverageHP>A":
                return " if the attacker's health is greater than the average health of the battle";
            case "AverageHP<A":
                return " if the attacker's health is less than the average health of the battle";
            case "AverageHP>D":
                return " if the target's health is greater than the average health of the battle";
            case "AverageHP<D":
                return " if the target's health is less than the average health of the battle";
            case "Grappling":
                return " if you are grappling";
            case "Grappled":
                return " if you are grappled";
            case "GrapplingA":
                return " if the attacker is grappling";
            case "GrappledA":
                return " if the attacker is grappled";
            case "GrapplingD":
                return " if the target is grappling";
            case "GrappledD":
                return " if the target is grappled";
            case "BadRNG":
                return " with ~" + specifics + "% chance";
            case "GoodRNG":
                return " with ~" + specifics + "% chance";
            case "HurtByA":
                return " if the attacker was hurt by the target";
            case "HurtBy<>A":
                return " if the attacker was not hurt by the target";
            case "HurtMostA":
                return " if the attacker was hurt the most by the target";
            case "HurtLeastA":
                return " if the attacker was hurt the least by the target";
            case "HurtByD":
                return " if the target was hurt by the attacker";
            case "HurtBy<>D":
                return " if the target was not hurt by the attacker";
            case "HurtMostD":
                return " if the target was hurt the most by the attacker";
            case "HurtLeastD":
                return " if the target was hurt the least by the attacker";
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
            case "Attack/2":
                return "half your attack value";
        }
        return specifics;
    }

    protected string AffectMapText(string effect, string specifics)
    {
        switch (effect)
        {
            case "TerrainEffect":
                return " create " + specifics;
            case "Tile":
                return " create " + specifics;
            case "Spread":
                return " spread " + specifics;
            case "ChainSpread":
                return " greatly spread " + specifics;
            case "RandomTileSwap":
                return " switch your tile for a random adjacent tile";
        }
        return "";
    }

    protected string PassiveEffect(string effect, string specifics, string target)
    {
        specifics = AdjustSpecificsText(specifics);
        if (target == "Map")
        {
            return AffectMapText(effect, specifics);
        }
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
            case "CurrentHealth%":
                return " decrease current health by " + specifics + "%";
            case "BaseEnergy%":
                return " change base energy of " + target + " by " + specifics + "%";
            case "Movement":
                return " " + target + " gain " + specifics + " movement";
            case "Skill":
                return " gain the " + specifics + " skill";
            case "TemporarySkill":
                return " gain the " + specifics + " skill which can be used once";
            case "SingleTemporarySkill":
                return " gain the " + specifics + " skill which can only be used once";
            case "Spell":
                return " gain the " + specifics + " spell";
            case "TemporarySpell":
                return " gain the " + specifics + " spell once";
            case "Status":
                return " inflict " + specifics+" on " + target;
            case "Buff":
                return " give " + specifics+" to "+target;
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
            case "TempRange":
                return " increase Attack Range of " + target + " by " + specifics;
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
            case "MentalState":
                return " change mental state to " + specifics;
            case "Amnesia":
                return " make " + target + " forget 1 temporary active skill";
            case "Active":
                return " use " + specifics;
            case "Death":
                return " die";
            case "CounterAttack":
                return " gain " + specifics + " counter attacks";
            case "DisableDeathActives":
                return " disable death effects";
            case "ReleaseGrapple":
                return " release the grappled target";
            case "BreakGrapple":
                return " break from any grapples";
            case "BaseDamageResistance":
                string[] bDRes = specifics.Split(">>");
                return " increase base " + bDRes[0] + " resistance by " + bDRes[1];
            case "CurrentDamageResistance":
                string[] cDRes = specifics.Split(">>");
                return " increase " + cDRes[0] + " resistance by " + cDRes[1];
            case "BaseElementalBonus":
                string[] bDBonus = specifics.Split(">>");
                return " increase base " + bDBonus[0] + " damage by " + bDBonus[1];
            case "ElementalDamageBonus":
                string[] cDBonus = specifics.Split(">>");
                return " increase " + cDBonus[0] + " damage by " + cDBonus[1];
            case "ScalingElementalBonus":
                string[] sEB = specifics.Split(">>");
                return " increase base " + sEB[0] + " damage by " + sEB[3] + "% for each level of this passive";
            case "ScalingElementalResist":
                string[] sER = specifics.Split(">>");
                return " increase base " + sER[0] + " resistance by " + sER[3] + "% for each level of this passive";
            case "ElementalBonusDamage":
                string[] eBD = specifics.Split(">>");
                return " deal " + eBD[1] + " " + eBD[0] + " damage";
            case "ElementalReflectDamage":
                string[] eRD = specifics.Split(">>");
                return " deal " + eRD[1] + " " + eRD[0] + " damage";
            case "Sleep":
                return "put " + target + " to sleep for " + specifics + " turns";
            case "Silence":
                return " disable actives of " + target + " for " + specifics + " turns";
            case "Invisible":
                return " turn invisible for " + specifics + " turns";
            case "Barricade":
                return " prevent temporary health from decaying for " + specifics + " turns";
            case "Guard":
                return " protect adjacent allies from attacks for " + specifics + " turns";
            case "GuardRange":
                return "Increase the distance from which you can protected allies from attacks to up to " + specifics + " tiles.";
            case "MoveForwardRandom":
                return " move to a random forward tile";
            case "MoveBackwardRandom":
                return " move to a random backward tile";
            case "FirstStrikeA":
                return " if the attacker has not attacked yet";
            case "FirstStrikeD":
                return " if the target has not attacked yet";
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
                return " spend " + specifics + " more movement";
                case "Decrease":
                return " spend " + specifics + " less movement";
            }
            return "";
            case "AttackValue%":
            switch (effect)
            {
                case "Increase":
                return " increase attack damage by " + specifics + "%";
                case "Decrease":
                return " decrease attack damage by " + specifics + "%";
            }
            return " " + effect + " attack damage by " + specifics;
            case "Damage%":
            return " " + effect + " damage multipler by " + specifics + "%";
            case "DefenseValue%":
            switch (effect)
            {
                case "Increase":
                return " increase defense by " + specifics + "%";
                case "Decrease":
                return " ignore " + specifics + "% of defense";
            }
            return " " + effect + " defense value by " + specifics;
        }
        return " " + effect + " " + target + " by " + specifics;
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
