using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveDescriptionViewer : MonoBehaviour
{
    public ActiveSkill dummyActive;
    public StatDatabase activeData;
    public SelectStatTextList activeSelect;
    public PopUpMessage popUp;
    public void SelectActive()
    {
        if (activeSelect.GetSelected() < 0){return;}
        dummyActive.LoadSkillFromString(activeData.ReturnValue(activeSelect.GetSelectedStat()));
        popUp.SetMessage(ReturnActiveDescription(dummyActive));
    }

    public string ReturnSpellDescription(MagicSpell spell, TacticActor caster = null)
    {
        string fullDetails = "";
        List<string> effects = spell.GetAllEffects();
        List<string> specifics = spell.GetAllSpecifics();
        List<int> powers = spell.GetAllPowers();
        for (int i = 0; i < effects.Count; i++)
        {
            fullDetails += AED(effects[i], specifics[i], powers[i].ToString());
            if (i < effects.Count - 1)
            {
                fullDetails += "\n";
            }
        }
        fullDetails += "\n" + "Action Cost: " + spell.GetActionCost();
        //+"; Actions Left: " +activeSkill;
        fullDetails += "\n" + "Mana Cost: "+spell.ReturnManaCost(caster);
        return fullDetails;
    }
    public string ReturnActiveDescriptionOnly(ActiveSkill activeSkill)
    {
        return AED(activeSkill.GetEffect(), activeSkill.GetSpecifics(), activeSkill.GetPower().ToString());
    }
    public string ReturnActiveDescription(ActiveSkill activeSkill)
    {
        string activeDescription = AED(activeSkill.GetEffect(), activeSkill.GetSpecifics(), activeSkill.GetPower().ToString());
        activeDescription += "\n" + "Action Cost: " + activeSkill.GetActionCost();
        //+"; Actions Left: " +activeSkill;
        activeDescription += "\n" + "Energy Cost: " + activeSkill.GetEnergyCost();
        return activeDescription;
    }

    // ActiveEffectDescription
    public string AED(string e, string s, string p)
    {
        if (e.Contains("AllSpritesEquals"))
        {
            string[] eBlocks = e.Split("Equals");
            return "All " + eBlocks[1] + "s gain " + p + " " + ASD(s) + ".";
        }
        switch (e)
        {
            case "CurrentHealth%":
                return "The target(s) lose " + ASD(s) + "% of their current health.";
            case "Attack":
                return "Attack the target(s) " + ASD(s) + " time(s) with " + APD(p) + "% damage.";
            case "ElementalAttack":
                return "Attack the target(s) with " + ASD(s) + " energy for " + APD(p) + "% damage.";
            // The specifics will determine the direction anyway.
            case "Attack+Displace":
                if (s == "Sideways")
                {
                    return "Attack and move the target(s) sideways with " + APD(p) + " additional force.";
                }
                return "Attack and "+ASD(s) + " the target(s) with " + APD(p) + " additional force.";
            case "Displace":
                if (s == "Sideways")
                {
                    return "Move the target(s) sideways with " + APD(p) + " additional force.";
                }
                return ASD(s) + " the target(s) with " + APD(p) + " additional force.";
            case "Teleport":
                return "Move to the targeted tile.";
            case "Teleport+Attack":
                return "Try to move " + ASD(s) + " the target and attack with " + APD(p) + "% damage.";
            case "Status":
                return "Give the target(s) " + ASD(s) + " for " + APD(p,e) + " turn(s).";
            case "Statuses":
                return "Give the target(s) " + ASD(s) + " for " + APD(p,e) + " turn(s).";
            case "Buff":
                return "Give the target(s) " + ASD(s) + " for " + APD(p,e) + " turn(s).";
            case "RemoveBuff":
                if (s == "All")
                {
                return "Remove all buff effect(s) from the target(s).";
                }
                return "Remove " + ASD(s) + " from the target(s).";
            case "RemoveStatus":
                if (s == "All")
                {
                return "Remove all status effect(s) from the target(s).";
                }
                return "Remove " + ASD(s) + " from the target(s).";
            case "RemoveStatuses":
                return "Remove " + ASD(s) + " from the target(s).";
            case "Summon":
                return "Summon a " + ASD(s) + ".";
            case "MassSummon":
                return "Summon " + ASD(s) + "(s).";
            case "RandomSummon":
                return "Randomly summon one of the following: " + ASD(s) + ".";
            case "MassRandomSummon":
                return "Randomly summon the following: " + ASD(s) + ".";
            case "Summon Enemy":
                return "Create a " + ASD(s) + ".";
            case "TerrainEffect":
                return "Create " + ASD(s) + " on targeted tile(s).";
            case "DelayedTileEffect":
                return "Set up a " + ASD(s) + " on targeted tile(s) that will activate in " + APD(p) + " turns.";
            case "Trap":
                return "Set up a " + ASD(s) + " trap on the targeted tile(s).";
            case "Taunt":
                return "Make target(s) more likely to attack.";
            case "Attack+Move":
                return "Attack the target(s) and move " + ASD(s) + " " + APD(p) + " tile(s).";
            case "Move+Attack":
                return "Move to the targeted tile and attack any target in " + ASD(s) + " of the the targeted tile.";
            case "Charge+Attack":
                return "Move to the targeted tile and attack any target in " + ASD(s) + " of the the targeted tile.";
            case "TemporaryPassive":
                return "Give the target(s) " + ASD(s) + " for " + APD(p) + " turns.";
            case "Passive":
                return "Grant the target(s) " + ASD(s) + ".";
            case "Attack+Status":
                return "Attack the target(s) and inflict " + ASD(s) + " for " + APD(p,e) + " turns.";
            case "Attack+MentalState":
                return "Attack the target(s) and try to change their mental state to " + ASD(s) + ".";
            case "Attack+Drain":
                return "Attack the target(s) and absorb some damage as health.";
            case "Attack+TerrainEffect":
                return "Attack the target(s)  with " + APD(p) + "% damage, create " + ASD(s) + " on targeted tile(s).";
            case "Tile":
                return "Try to change the targeted tile(s) to " + ASD(s) + ".";
            case "Border":
                return "Create " + ASD(s) + " borders on the target's tile(s) based on current direction.";
            case "AllBorders":
                return "Create " + ASD(s) + " borders on the target's tile(s).";
            case "True Attack":
                return "Deal damage equal to " + APD(p) + "% of " + ASD(s) + ".";
            case "ElementalDamage":
                return "Deal " + APD(p) + " " + ASD(s) + " damage.";
            case "Flat Attack":
                return "Deal " + ASD(s) + " damage.";
            case "Attack+Tile":
                return "Attack the target(s)  with " + APD(p) + "% damage, and try to change the targeted tile(s) to " + ASD(s) + ".";
            case "MentalState":
                return "Try to change the target(s) mental state to " + ASD(s) + ".";
            case "Amnesia":
                return "Try to make the target forget " + ASD(s) + " temporary active skill(s).";
            case "Attack+Grapple":
                return "Grapple the target and attack them.";
            case "Grapple":
                return "Grapple the target.";
            case "Throw Grappled":
                return "Throw the grappled target.";
            case "Ingest":
                return "Try to consume the grappled target.";
            case "SwapRelease":
                return "Switch places and then release the grappled target.";
            case "Swap":
                return "Switch " + ASD(s) + " with the targeted location.";
            case "Attack+Amnesia":
                return "Attack the target(s)  with " + APD(p) + "% damage, and try to make the target forget " + ASD(s) + " temporary active skill(s).";
            case "Weather":
                return "Change the weather to " + ASD(s) + ".";
            case "Movement":
                return "The target(s) gain " + ASD(s) + " movement.";
            case "AllAllies":
                return "All allies gain " + APD(p) + " " + ASD(s) + ".";
            case "AllEnemies":
                return "All enemies receive " + APD(p) + " " + ASD(s) + ".";
            case "Command":
                return "Give a(n) " + ASD(s) + " command.";
            case "Escape":
                return "Escape from the battle if on a border tile without any enemies nearby.";
            case "Sleep":
                return "Put the target(s) to sleep for " + s + " turns.";
            case "Silence":
                return "Disable target(s) skills for " + s + " turns.";
            case "Barricade":
                return "Prevent temporary health from decaying for " + s + " turns.";
            case "Guard":
                return "Protect adjacent allies from attacks for " + s + " turns.";
            case "GuardRange":
                return "Increase the distance from which you can protected allies from attacks to up to " + s + " tiles.";
            case "Disarm":
                return "Remove the target(s) weapon.";
            case "Learn":
                return "Learn a random skill from the target(s).";
            case "Teach":
                return "Teach a random skill to the target(s).";
            case "Pain Split":
                return "Share health equally between all targets.";
            case "Aura":
                return "Gain the " + s + " aura for " + p + " turns.";
            case "Kill":
                return "Kill the target(s) if their health is less than " + s + ".";
            case "Manaize":
                return "Convert " + ASD(s) + " into mana.";
        }
        return "The target(s) gain " + ASD(s) + " " + e + ".";
    }

    // ActiveSpecificsDescription
    public string ASD(string specifics)
    {
        return specifics;
    }

    // ActivePowerDescription
    public string APD(string power, string effect = "")
    {
        if (power == "-1" && effect.Contains("Status"))
        {
            return "ALL";
        }
        return power;
    }
}
