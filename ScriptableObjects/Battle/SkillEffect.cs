using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillEffect", menuName = "ScriptableObjects/BattleLogic/SkillEffect", order = 1)]
public class SkillEffect : ScriptableObject
{
    public GeneralUtility utility;
    public PassiveOrganizer passiveOrganizer;
    public int basicDenominator = 100;
    public int baseStatusDuration = 3;
    public void AffectActor(TacticActor target, string effect, string effectSpecifics, int level = 1, CombatLog combatLog = null)
    {
        int changeAmount = 0;
        switch (effect)
        {
            case "Passive":
                target.AddPassiveSkill(effectSpecifics, "1");
                break;
            case "Temporary Passive":
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
                        duration = 1;
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
                target.UpdateBaseEnergy(int.Parse(effectSpecifics) * level * target.GetBaseEnergy() / basicDenominator);
                break;
            case "BaseAttack":
                int bAtkChange = int.Parse(effectSpecifics) * level;
                target.UpdateBaseAttack(bAtkChange);
                target.UpdateAttack(bAtkChange);
                break;
            case "BaseAttack%":
                int bAtkPChange = level * (int.Parse(effectSpecifics) * target.GetBaseAttack()) / basicDenominator;
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
                target.UpdateTempDefense((int.Parse(effectSpecifics) * target.GetBaseAttack()) / basicDenominator);
                break;
            case "Defense%":
                target.UpdateDefense(level * int.Parse(effectSpecifics) * target.GetBaseDefense() / basicDenominator);
                break;
            case "Skill":
                // Add an active skill.
                target.AddActiveSkill(effectSpecifics);
                break;
            case "Temporary Skill":
                target.AddTempActive(effectSpecifics);
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
                target.RemoveRandomActiveSkill();
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
        }
    }

    public int SafeParseInt(string intString, int defaultValue = 1)
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
