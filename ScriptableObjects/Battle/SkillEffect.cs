using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillEffect", menuName = "ScriptableObjects/BattleLogic/SkillEffect", order = 1)]
public class SkillEffect : ScriptableObject
{
    public GeneralUtility utility;
    public PassiveOrganizer passiveOrganizer;
    public StatDatabase standardSpells;
    public int basicDenominator = 100;
    public int baseStatusDuration = 3;
    public void AffectActor(TacticActor target, string effect, string effectSpecifics, int level = 1, CombatLog combatLog = null)
    {
        if (target == null){return;}
        int changeAmount = 0;
        switch (effect)
        {
            case "Passive":
                target.AddPassiveSkill(effectSpecifics, "1");
                break;
            case "TemporaryPassive":
                if (target.AddTempPassive(effectSpecifics, level))
                {
                    passiveOrganizer.AddSortedPassive(target, effectSpecifics);
                }
                break;
            case "Status":
                int duration = level;
                if (level <= baseStatusDuration && level >= 0) { duration = baseStatusDuration; }
                // Some statuses don't naturally wear off and are permanent or immediately take effect.
                switch (effectSpecifics)
                {
                    case "Poison":
                        duration = -1;
                        break;
                    case "Burn":
                        duration = -1;
                        break;
                    case "Bleed":
                        duration = -1;
                        break;
                    case "Stun":
                        target.ResetActions();
                        break;
                }
                target.AddStatus(effectSpecifics, duration);
                break;
            case "Statuses":
                int durations = level;
                if (level <= baseStatusDuration && level >= 0) { durations = baseStatusDuration; }
                string[] statuses = effectSpecifics.Split(",");
                for (int i = 0; i < statuses.Length; i++)
                {
                    AffectActor(target, "Status", statuses[i], durations);
                }
                break;
            case "Buff":
                target.AddBuff(effectSpecifics, level);
                break;
            case "RemoveBuff":
                target.RemoveBuff(effectSpecifics);
                break;
            case "RemoveStatus":
                target.RemoveStatus(effectSpecifics);
                break;
            case "RemoveStatuses":
                string[] removedStatuses = effectSpecifics.Split(",");
                for (int i = 0; i < removedStatuses.Length; i++)
                {
                    target.RemoveStatus(removedStatuses[i]);
                }
                break;
            // Temp health is always a shield.
            case "TempHealth":
                target.UpdateTempHealth(int.Parse(effectSpecifics));
                break;
            // Default is increasing health.
            case "Health":
                target.UpdateHealth(int.Parse(effectSpecifics) * level, false);
                break;
            case "Damage":
                int effectDamage = int.Parse(effectSpecifics) * level;
                int effectDamageTaken = Mathf.Max(0, effectDamage - target.GetDefense());
                target.TakeEffectDamage(int.Parse(effectSpecifics) * level);
                if (combatLog != null)
                {
                    combatLog.UpdateNewestLog(target.GetPersonalName() + " takes " + effectDamageTaken + " damage.");
                }
                break;
            case "Energy":
                target.UpdateEnergy(int.Parse(effectSpecifics) * level);
                break;
            case "TempAttack":
                target.UpdateTempAttack(int.Parse(effectSpecifics));
                break;
            case "Attack":
                target.UpdateAttack(int.Parse(effectSpecifics) * level);
                break;
            case "TempDefense":
                target.UpdateTempDefense(int.Parse(effectSpecifics));
                break;
            case "Defense":
                target.UpdateDefense(int.Parse(effectSpecifics) * level);
                break;
            case "AllStats":
                target.UpdateHealth(int.Parse(effectSpecifics) * level, false);
                target.UpdateAttack(int.Parse(effectSpecifics) * level);
                target.UpdateDefense(int.Parse(effectSpecifics) * level);
                break;
            case "AllStats%":
                AffectActor(target, "BaseHealth%", effectSpecifics, level);
                AffectActor(target, "BaseAttack%", effectSpecifics, level);
                AffectActor(target, "BaseDefense%", effectSpecifics, level);
                break;
            case "RandomBaseStat":
                // Health / Attack / Defense
                int baseStatRoll = Random.Range(0, 3);
                switch (baseStatRoll)
                {
                    case 0:
                    AffectActor(target, "BaseHealth", effectSpecifics, level);
                    break;
                    case 1:
                    AffectActor(target, "BaseAttack", effectSpecifics, level);
                    break;
                    case 2:
                    AffectActor(target, "BaseDefense", effectSpecifics, level);
                    break;
                }
                break;
            case "CurrentHealth%":
                int currentHealth = target.GetHealth();
                target.UpdateHealth(int.Parse(effectSpecifics) * currentHealth / basicDenominator);
                break;
            case "BaseHealth":
                target.UpdateBaseHealth(int.Parse(effectSpecifics) * level, false);
                target.UpdateHealth(int.Parse(effectSpecifics) * level, false);
                break;
            case "BaseHealth%":
                target.UpdateBaseHealth(int.Parse(effectSpecifics) * level * target.GetBaseHealth() / basicDenominator, false);
                target.UpdateHealth(int.Parse(effectSpecifics) * level * target.GetBaseHealth() / basicDenominator, false);
                break;
            case "MaxHealth%":
                target.UpdateBaseHealth(int.Parse(effectSpecifics) * level * target.GetBaseHealth() / basicDenominator, false);
                if (target.GetHealth() > target.GetBaseHealth())
                {
                    target.SetCurrentHealth(target.GetBaseHealth());
                }
                break;
            case "BaseEnergy":
                target.UpdateBaseEnergy(int.Parse(effectSpecifics) * level);
                target.UpdateEnergy(int.Parse(effectSpecifics) * level);
                break;
            case "BaseEnergy%":
                int bEnergyChange = int.Parse(effectSpecifics) * level * target.GetBaseEnergy() / basicDenominator;
                if (bEnergyChange < 1 && int.Parse(effectSpecifics) > 0)
                {
                    bEnergyChange = 1;
                }
                target.UpdateBaseEnergy(bEnergyChange);
                target.UpdateEnergy(bEnergyChange);
                break;
            case "BaseAttack":
                int bAtkChange = int.Parse(effectSpecifics) * level;
                target.UpdateBaseAttack(bAtkChange);
                target.UpdateAttack(bAtkChange);
                break;
            case "BaseAttack%":
                int bAtkPChange = level * (int.Parse(effectSpecifics) * target.GetBaseAttack()) / basicDenominator;
                if (int.Parse(effectSpecifics) > 0)
                {
                    bAtkPChange = Mathf.Max(1, bAtkPChange);
                }
                target.UpdateBaseAttack(bAtkPChange);
                target.UpdateAttack(bAtkPChange);
                break;
            case "BaseDefense":
                int bDefChange = int.Parse(effectSpecifics) * level;
                target.UpdateBaseDefense(bDefChange);
                target.UpdateDefense(bDefChange);
                break;
            case "BaseDefense%":
                int bDefPChange = level * (int.Parse(effectSpecifics) * target.GetBaseDefense()) / basicDenominator;
                // Positive boosts always increase stat by at least 1.
                if (int.Parse(effectSpecifics) > 0)
                {
                    bDefChange = Mathf.Max(1, bDefPChange);
                }
                target.UpdateBaseDefense(bDefPChange);
                target.UpdateDefense(bDefPChange);
                break;
            case "AttackRange":
                target.SetAttackRangeMax(int.Parse(effectSpecifics));
                break;
            case "TempRange":
                target.UpdateBonusAttackRange(int.Parse(effectSpecifics));
                break;
            case "TempHealth%":
                changeAmount = int.Parse(effectSpecifics) * target.GetBaseHealth() / basicDenominator;
                if (Mathf.Abs(changeAmount) < Mathf.Abs(int.Parse(effectSpecifics)))
                {
                    changeAmount = int.Parse(effectSpecifics);
                }
                target.UpdateTempHealth(changeAmount);
                break;
            case "Health%":
                // % changes should not go below a minimum amount or else low stat characters are effectively immune.
                changeAmount = level * int.Parse(effectSpecifics) * target.GetBaseHealth() / basicDenominator;
                if (Mathf.Abs(changeAmount) < Mathf.Abs(int.Parse(effectSpecifics) * level))
                {
                    changeAmount = int.Parse(effectSpecifics);
                }
                target.UpdateHealth((changeAmount), false);
                break;
            case "TempAttack%":
                target.UpdateTempAttack((int.Parse(effectSpecifics) * target.GetBaseAttack()) / basicDenominator);
                break;
            case "Attack%":
                target.UpdateAttack(level * (int.Parse(effectSpecifics) * target.GetBaseAttack()) / basicDenominator);
                break;
            case "TempDefense%":
                int tDefPChange = level * (int.Parse(effectSpecifics) * target.GetBaseDefense()) / basicDenominator;
                // Positive boosts always increase stat by at least 1.
                if (int.Parse(effectSpecifics) > 0)
                {
                    tDefPChange = Mathf.Max(1, tDefPChange);
                }
                target.UpdateTempDefense(tDefPChange);
                break;
            case "Defense%":
                int defPChange = level * (int.Parse(effectSpecifics) * target.GetBaseDefense()) / basicDenominator;
                // Positive boosts always increase stat by at least 1.
                if (int.Parse(effectSpecifics) > 0)
                {
                    defPChange = Mathf.Max(1, defPChange);
                }
                target.UpdateDefense(defPChange);
                break;
            case "Skill":
                // Add an active skill.
                target.AddActiveSkill(effectSpecifics);
                break;
            case "TemporarySkill":
                target.AddTempActive(effectSpecifics);
                break;
            case "Spell":
                target.LearnSpell(standardSpells.ReturnValue(effectSpecifics));
                break;
            case "TemporarySpell":
                target.LearnTempSpell(standardSpells.ReturnValue(effectSpecifics));
                break;
            case "Speed":
                target.UpdateSpeed(int.Parse(effectSpecifics) * level);
                break;
            case "BaseSpeed":
                target.SetMoveSpeedMax(int.Parse(effectSpecifics));
                break;
            case "Movement":
                AffectActorMovement(target, effectSpecifics, level);
                break;
            case "BaseActions":
                target.UpdateBaseActions(level * int.Parse(effectSpecifics));
                break;
            case "Actions":
                target.AdjustActionAmount(level * int.Parse(effectSpecifics));
                break;
            case "BonusActions":
                target.GainBonusActions(level * int.Parse(effectSpecifics));
                break;
            case "MoveType":
                target.SetMoveType(effectSpecifics);
                break;
            case "Initiative":
                target.ChangeInitiative(int.Parse(effectSpecifics));
                break;
            case "TempInitiative":
                target.UpdateTempInitiative(int.Parse(effectSpecifics));
                break;
            case "Weight":
                target.UpdateWeight(int.Parse(effectSpecifics));
                break;
            case "Death":
                target.SetCurrentHealth(0);
                target.ResetActions();
                break;
            case "MentalState":
                target.SetMentalState(effectSpecifics);
                break;
            case "Amnesia":
                target.RemoveRandomTempActiveSkill();
                break;
            case "Counter":
                target.UpdateCounter(int.Parse(effectSpecifics));
                break;
            case "CounterAttack":
                target.GainCounterAttacks(int.Parse(effectSpecifics));
                break;
            case "BaseHitChance":
                target.UpdateBaseHitChance(int.Parse(effectSpecifics));
                break;
            case "HitChance":
                target.UpdateHitChance(int.Parse(effectSpecifics));
                break;
            case "BaseDodge":
                target.UpdateBaseDodge(int.Parse(effectSpecifics));
                break;
            case "Dodge":
                target.UpdateDodgeChance(int.Parse(effectSpecifics));
                break;
            case "BaseCritChance":
                target.UpdateBaseCritChance(int.Parse(effectSpecifics));
                break;
            case "CritChance":
                target.UpdateCritChance(int.Parse(effectSpecifics));
                break;
            case "BaseCritDamage":
                target.UpdateBaseCritDamage(int.Parse(effectSpecifics));
                break;
            case "CritDamage":
                target.UpdateCritDamage(int.Parse(effectSpecifics));
                break;
            case "DisablePassives":
                target.DisableDeathPassives();
                break;
            case "ReleaseGrapple":
                target.ReleaseGrapple();
                break;
            case "BreakGrapple":
                target.BreakGrapple();
                break;
            case "BaseDamageResistance":
                string[] baseResist = effectSpecifics.Split("Equals");
                target.UpdateBaseDamageResist(baseResist[0], SafeParseInt(baseResist[1]));
                break;
            case "CurrentDamageResistance":
                string[] cResist = effectSpecifics.Split("Equals");
                target.UpdateCurrentDamageResist(cResist[0], SafeParseInt(cResist[1]));
                break;
            case "BaseElementalBonus":
                string[] baseBonus = effectSpecifics.Split("Equals");
                target.UpdateElementalDamageBonus(baseBonus[0], SafeParseInt(baseBonus[1]));
                break;
            case "ElementalDamageBonus":
                string[] cBonus = effectSpecifics.Split("Equals");
                target.UpdateCurrentElementalDamageBonus(cBonus[0], SafeParseInt(cBonus[1]));
                break;
            case "ScalingElementalBonus":
                string[] scalingEB = effectSpecifics.Split("Equals");
                target.UpdateElementalDamageBonus(scalingEB[0], GetScalingInt(target, scalingEB[1], scalingEB[2], scalingEB[3]));
                break;
            case "ScalingElementalResist":
                string[] scalingER = effectSpecifics.Split("Equals");
                target.UpdateBaseDamageResist(scalingER[0], GetScalingInt(target, scalingER[1], scalingER[2], scalingER[3]));
                break;
            case "VigorEfficiency":
                target.IncreaseVigorScaling(int.Parse(effectSpecifics));
                break;
            case "Vigor":
                target.RestoreVigor(int.Parse(effectSpecifics));
                break;
            case "Silence":
                target.Silence(int.Parse(effectSpecifics));
                break;
            case "Sleep":
                target.Sleep(int.Parse(effectSpecifics));
                break;
            case "Invisible":
                target.TurnInvisible(int.Parse(effectSpecifics));
                break;
            case "Barricade":
                target.GainBarricade(int.Parse(effectSpecifics));
                break;
            case "Guard":
                target.GainGuard(int.Parse(effectSpecifics));
                break;
            case "GuardRange":
                target.SetGuardRange(int.Parse(effectSpecifics));
                break;
            case "Disarm":
                string disarmedWeapon = target.Disarm();
                // TODO Try to remove any passives that the weapon granted and refresh the target's passives.
                break;
        }
    }

    protected int GetScalingInt(TacticActor target, string scaling, string scalingSpecifics, string scalingMultiplier)
    {
        switch (scaling)
        {
            case "PLevel":
            return GetTargetPassiveLevel(target, scalingSpecifics) * SafeParseInt(scalingMultiplier);
        }
        return 1;
    }

    protected int GetTargetPassiveLevel(TacticActor target, string passiveName)
    {
        return target.GetLevelFromPassive(passiveName);
    }

    protected int SafeParseInt(string intString, int defaultValue = 1)
    {
        try
        {
            return int.Parse(intString);
        }
        catch
        {
            return defaultValue;
        }
    }

    protected void AffectActorMovement(TacticActor target, string effectSpecifics, int power = 1)
    {
        int amount = SafeParseInt(effectSpecifics, -1);
        if (amount > 0)
        {
            target.GainMovement(power * amount);
            return;
        }
        switch (effectSpecifics)
        {
            case "Speed":
                amount = target.GetSpeed();
                break;
        }
        // You never lose movement, except by moving, if you want someone to stop moving you affect their speed, not their movement.
        if (amount <= 0) { amount = 1; }
        target.GainMovement(power * amount);
    }
}
