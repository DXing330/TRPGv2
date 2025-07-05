using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object that handles applying various effects to actors in battle.
/// Uses a strategy pattern to handle different effect types professionally.
/// </summary>
[CreateAssetMenu(fileName = "SkillEffect", menuName = "ScriptableObjects/BattleLogic/SkillEffect", order = 1)]
public class SkillEffect : ScriptableObject
{
    #region Serialized Fields
    [SerializeField] private PassiveOrganizer passiveOrganizer;
    [SerializeField] private int basicDenominator = 100;
    [SerializeField] private int baseStatusDuration = 3;
    #endregion

    #region Effect Types
    /// <summary>
    /// Enumeration of all possible effect types that can be applied to actors.
    /// </summary>
    public enum EffectType
    {
        TemporaryPassive,
        Status,
        RemoveStatus,
        TempHealth,
        Health,
        Energy,
        TempAttack,
        Attack,
        TempDefense,
        Defense,
        AllStats,
        BaseHealth,
        BaseEnergy,
        BaseAttack,
        BaseAttackPercent,
        BaseDefense,
        AttackRange,
        TempHealthPercent,
        HealthPercent,
        TempAttackPercent,
        AttackPercent,
        TempDefensePercent,
        DefensePercent,
        Skill,
        Speed,
        BaseSpeed,
        Movement,
        BaseActions,
        Actions,
        MoveType,
        Initiative,
        Weight,
        Death,
        MentalState,
        Amnesia
    }
    #endregion

    #region Effect Handlers
    /// <summary>
    /// Dictionary mapping effect types to their corresponding handler methods.
    /// </summary>
    private readonly Dictionary<EffectType, System.Action<TacticActor, string, int>> effectHandlers = 
        new Dictionary<EffectType, System.Action<TacticActor, string, int>>();

    /// <summary>
    /// Initialize effect handlers dictionary.
    /// </summary>
    private void InitializeEffectHandlers()
    {
        if (effectHandlers.Count > 0) return; // Already initialized

        effectHandlers[EffectType.TemporaryPassive] = HandleTemporaryPassive;
        effectHandlers[EffectType.Status] = HandleStatus;
        effectHandlers[EffectType.RemoveStatus] = HandleRemoveStatus;
        effectHandlers[EffectType.TempHealth] = HandleTempHealth;
        effectHandlers[EffectType.Health] = HandleHealth;
        effectHandlers[EffectType.Energy] = HandleEnergy;
        effectHandlers[EffectType.TempAttack] = HandleTempAttack;
        effectHandlers[EffectType.Attack] = HandleAttack;
        effectHandlers[EffectType.TempDefense] = HandleTempDefense;
        effectHandlers[EffectType.Defense] = HandleDefense;
        effectHandlers[EffectType.AllStats] = HandleAllStats;
        effectHandlers[EffectType.BaseHealth] = HandleBaseHealth;
        effectHandlers[EffectType.BaseEnergy] = HandleBaseEnergy;
        effectHandlers[EffectType.BaseAttack] = HandleBaseAttack;
        effectHandlers[EffectType.BaseAttackPercent] = HandleBaseAttackPercent;
        effectHandlers[EffectType.BaseDefense] = HandleBaseDefense;
        effectHandlers[EffectType.AttackRange] = HandleAttackRange;
        effectHandlers[EffectType.TempHealthPercent] = HandleTempHealthPercent;
        effectHandlers[EffectType.HealthPercent] = HandleHealthPercent;
        effectHandlers[EffectType.TempAttackPercent] = HandleTempAttackPercent;
        effectHandlers[EffectType.AttackPercent] = HandleAttackPercent;
        effectHandlers[EffectType.TempDefensePercent] = HandleTempDefensePercent;
        effectHandlers[EffectType.DefensePercent] = HandleDefensePercent;
        effectHandlers[EffectType.Skill] = HandleSkill;
        effectHandlers[EffectType.Speed] = HandleSpeed;
        effectHandlers[EffectType.BaseSpeed] = HandleBaseSpeed;
        effectHandlers[EffectType.Movement] = HandleMovement;
        effectHandlers[EffectType.BaseActions] = HandleBaseActions;
        effectHandlers[EffectType.Actions] = HandleActions;
        effectHandlers[EffectType.MoveType] = HandleMoveType;
        effectHandlers[EffectType.Initiative] = HandleInitiative;
        effectHandlers[EffectType.Weight] = HandleWeight;
        effectHandlers[EffectType.Death] = HandleDeath;
        effectHandlers[EffectType.MentalState] = HandleMentalState;
        effectHandlers[EffectType.Amnesia] = HandleAmnesia;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Applies an effect to the target actor.
    /// </summary>
    /// <param name="target">The actor to apply the effect to</param>
    /// <param name="effect">The effect type as a string</param>
    /// <param name="effectSpecifics">Additional parameters for the effect</param>
    /// <param name="level">The level/intensity of the effect</param>
    public void AffectActor(TacticActor target, string effect, string effectSpecifics, int level = 1)
    {
        try
        {
            // Validate input parameters
            if (!ValidateInput(target, effect, effectSpecifics, level))
            {
                return;
            }

            // Initialize handlers if not already done
            InitializeEffectHandlers();

            // Convert string effect to enum
            if (!TryParseEffectType(effect, out EffectType effectType))
            {
                Debug.LogWarning($"Unknown effect type: {effect}");
                return;
            }

            // Apply the effect using the appropriate handler
            if (effectHandlers.TryGetValue(effectType, out var handler))
            {
                handler(target, effectSpecifics, level);
            }
            else
            {
                Debug.LogWarning($"No handler found for effect type: {effectType}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error applying effect '{effect}' to {target.name}: {ex.Message}");
        }
    }
    #endregion

    #region Validation and Parsing
    /// <summary>
    /// Validates input parameters for the AffectActor method.
    /// </summary>
    private bool ValidateInput(TacticActor target, string effect, string effectSpecifics, int level)
    {
        if (target == null)
        {
            Debug.LogError("Target actor cannot be null");
            return false;
        }

        if (string.IsNullOrEmpty(effect))
        {
            Debug.LogError("Effect type cannot be null or empty");
            return false;
        }

        if (level < 0)
        {
            Debug.LogWarning($"Negative level ({level}) provided for effect {effect}");
        }

        return true;
    }

    /// <summary>
    /// Attempts to parse a string effect type to the corresponding enum.
    /// </summary>
    private bool TryParseEffectType(string effectString, out EffectType effectType)
    {
        // Handle special cases with different naming conventions
        switch (effectString)
        {
            case "Temporary Passive":
                effectType = EffectType.TemporaryPassive;
                return true;
            case "BaseAttack%":
                effectType = EffectType.BaseAttackPercent;
                return true;
            case "TempHealth%":
                effectType = EffectType.TempHealthPercent;
                return true;
            case "Health%":
                effectType = EffectType.HealthPercent;
                return true;
            case "TempAttack%":
                effectType = EffectType.TempAttackPercent;
                return true;
            case "Attack%":
                effectType = EffectType.AttackPercent;
                return true;
            case "TempDefense%":
                effectType = EffectType.TempDefensePercent;
                return true;
            case "Defense%":
                effectType = EffectType.DefensePercent;
                return true;
            default:
                return Enum.TryParse(effectString, true, out effectType);
        }
    }

    /// <summary>
    /// Safely parses a string to an integer with error handling.
    /// </summary>
    /// <param name="value">The string value to parse</param>
    /// <param name="defaultValue">The default value to return if parsing fails</param>
    /// <returns>The parsed integer or the default value</returns>
    private int SafeParseInt(string value, int defaultValue = 0)
    {
        if (int.TryParse(value, out int result))
        {
            return result;
        }
        
        Debug.LogWarning($"Failed to parse '{value}' as integer, using default value {defaultValue}");
        return defaultValue;
    }

    /// <summary>
    /// Calculates percentage-based changes with minimum thresholds.
    /// </summary>
    /// <param name="baseValue">The base value to calculate percentage from</param>
    /// <param name="percentage">The percentage to apply</param>
    /// <param name="level">The level multiplier</param>
    /// <param name="minimumChange">The minimum absolute change value</param>
    /// <returns>The calculated change amount</returns>
    private int CalculatePercentageChange(int baseValue, int percentage, int level, int minimumChange)
    {
        int changeAmount = level * percentage * baseValue / basicDenominator;
        int minimumAbsoluteChange = Mathf.Abs(percentage * level);
        
        if (Mathf.Abs(changeAmount) < minimumAbsoluteChange)
        {
            changeAmount = minimumChange;
        }
        
        return changeAmount;
    }
    #endregion

    #region Effect Handler Methods
    private void HandleTemporaryPassive(TacticActor target, string effectSpecifics, int level)
    {
        target.AddTempPassive(effectSpecifics, level);
        if (passiveOrganizer != null)
        {
            passiveOrganizer.OrganizeActorPassives(target);
        }
    }

    private void HandleStatus(TacticActor target, string effectSpecifics, int level)
    {
        int duration = level <= 1 ? baseStatusDuration : level;
        
        // Handle special status durations
        switch (effectSpecifics)
        {
            case "Poison":
            case "Burn":
            case "Bleed":
                duration = -1; // Permanent until removed
                break;
            case "Stun":
                target.ResetActions();
                duration = 1;
                break;
        }
        
        target.AddStatus(effectSpecifics, duration);
    }

    private void HandleRemoveStatus(TacticActor target, string effectSpecifics, int level)
    {
        target.RemoveStatus(effectSpecifics);
    }

    private void HandleTempHealth(TacticActor target, string effectSpecifics, int level)
    {
        int amount = SafeParseInt(effectSpecifics);
        target.UpdateTempHealth(amount);
    }

    private void HandleHealth(TacticActor target, string effectSpecifics, int level)
    {
        int amount = SafeParseInt(effectSpecifics) * level;
        target.UpdateHealth(amount, false);
    }

    private void HandleEnergy(TacticActor target, string effectSpecifics, int level)
    {
        int amount = SafeParseInt(effectSpecifics) * level;
        target.UpdateEnergy(amount);
    }

    private void HandleTempAttack(TacticActor target, string effectSpecifics, int level)
    {
        int amount = SafeParseInt(effectSpecifics);
        target.UpdateTempAttack(amount);
    }

    private void HandleAttack(TacticActor target, string effectSpecifics, int level)
    {
        int amount = SafeParseInt(effectSpecifics) * level;
        target.UpdateAttack(amount);
    }

    private void HandleTempDefense(TacticActor target, string effectSpecifics, int level)
    {
        int amount = SafeParseInt(effectSpecifics);
        target.UpdateTempDefense(amount);
    }

    private void HandleDefense(TacticActor target, string effectSpecifics, int level)
    {
        int amount = SafeParseInt(effectSpecifics) * level;
        target.UpdateDefense(amount);
    }

    private void HandleAllStats(TacticActor target, string effectSpecifics, int level)
    {
        int amount = SafeParseInt(effectSpecifics) * level;
        target.UpdateHealth(amount, false);
        target.UpdateAttack(amount);
        target.UpdateDefense(amount);
    }

    private void HandleBaseHealth(TacticActor target, string effectSpecifics, int level)
    {
        int amount = SafeParseInt(effectSpecifics) * level;
        target.UpdateBaseHealth(amount, false);
        target.UpdateHealth(amount, false);
    }

    private void HandleBaseEnergy(TacticActor target, string effectSpecifics, int level)
    {
        int amount = SafeParseInt(effectSpecifics) * level;
        target.UpdateBaseEnergy(amount);
        target.UpdateEnergy(amount);
    }

    private void HandleBaseAttack(TacticActor target, string effectSpecifics, int level)
    {
        int amount = SafeParseInt(effectSpecifics) * level;
        target.UpdateBaseAttack(amount);
    }

    private void HandleBaseAttackPercent(TacticActor target, string effectSpecifics, int level)
    {
        int percentage = SafeParseInt(effectSpecifics);
        int amount = level * (percentage * target.GetBaseAttack()) / basicDenominator;
        target.UpdateBaseAttack(amount);
    }

    private void HandleBaseDefense(TacticActor target, string effectSpecifics, int level)
    {
        int amount = SafeParseInt(effectSpecifics) * level;
        target.UpdateBaseDefense(amount);
    }

    private void HandleAttackRange(TacticActor target, string effectSpecifics, int level)
    {
        int range = SafeParseInt(effectSpecifics);
        target.SetAttackRangeMax(range);
    }

    private void HandleTempHealthPercent(TacticActor target, string effectSpecifics, int level)
    {
        int percentage = SafeParseInt(effectSpecifics);
        int changeAmount = CalculatePercentageChange(target.GetBaseHealth(), percentage, 1, percentage);
        target.UpdateTempHealth(changeAmount);
    }

    private void HandleHealthPercent(TacticActor target, string effectSpecifics, int level)
    {
        int percentage = SafeParseInt(effectSpecifics);
        int changeAmount = CalculatePercentageChange(target.GetBaseHealth(), percentage, level, percentage);
        target.UpdateHealth(changeAmount, false);
    }

    private void HandleTempAttackPercent(TacticActor target, string effectSpecifics, int level)
    {
        int percentage = SafeParseInt(effectSpecifics);
        int amount = (percentage * target.GetBaseAttack()) / basicDenominator;
        target.UpdateTempAttack(amount);
    }

    private void HandleAttackPercent(TacticActor target, string effectSpecifics, int level)
    {
        int percentage = SafeParseInt(effectSpecifics);
        int amount = level * (percentage * target.GetBaseAttack()) / basicDenominator;
        target.UpdateAttack(amount);
    }

    private void HandleTempDefensePercent(TacticActor target, string effectSpecifics, int level)
    {
        int percentage = SafeParseInt(effectSpecifics);
        int amount = (percentage * target.GetBaseDefense()) / basicDenominator;
        target.UpdateTempDefense(amount);
    }

    private void HandleDefensePercent(TacticActor target, string effectSpecifics, int level)
    {
        int percentage = SafeParseInt(effectSpecifics);
        int amount = level * percentage * target.GetBaseDefense() / basicDenominator;
        target.UpdateDefense(amount);
    }

    private void HandleSkill(TacticActor target, string effectSpecifics, int level)
    {
        if (string.IsNullOrEmpty(effectSpecifics)) return;
        
        string[] newSkills = effectSpecifics.Split(',');
        foreach (string skill in newSkills)
        {
            if (!string.IsNullOrWhiteSpace(skill))
            {
                target.AddActiveSkill(skill.Trim());
            }
        }
    }

    private void HandleSpeed(TacticActor target, string effectSpecifics, int level)
    {
        int amount = SafeParseInt(effectSpecifics) * level;
        target.UpdateSpeed(amount);
    }

    private void HandleBaseSpeed(TacticActor target, string effectSpecifics, int level)
    {
        int speed = SafeParseInt(effectSpecifics);
        target.SetMoveSpeedMax(speed);
    }

    private void HandleMovement(TacticActor target, string effectSpecifics, int level)
    {
        int amount = SafeParseInt(effectSpecifics) * level;
        target.GainMovement(amount);
    }

    private void HandleBaseActions(TacticActor target, string effectSpecifics, int level)
    {
        int amount = SafeParseInt(effectSpecifics) * level;
        target.UpdateBaseActions(amount);
    }

    private void HandleActions(TacticActor target, string effectSpecifics, int level)
    {
        int amount = SafeParseInt(effectSpecifics) * level;
        target.AdjustActionAmount(amount);
    }

    private void HandleMoveType(TacticActor target, string effectSpecifics, int level)
    {
        target.SetMoveType(effectSpecifics);
    }

    private void HandleInitiative(TacticActor target, string effectSpecifics, int level)
    {
        int amount = SafeParseInt(effectSpecifics);
        target.ChangeInitiative(amount);
    }

    private void HandleWeight(TacticActor target, string effectSpecifics, int level)
    {
        int amount = SafeParseInt(effectSpecifics);
        target.UpdateWeight(amount);
    }

    private void HandleDeath(TacticActor target, string effectSpecifics, int level)
    {
        target.SetCurrentHealth(0);
        target.ResetActions();
    }

    private void HandleMentalState(TacticActor target, string effectSpecifics, int level)
    {
        target.SetMentalState(effectSpecifics);
    }

    private void HandleAmnesia(TacticActor target, string effectSpecifics, int level)
    {
        target.RemoveRandomActiveSkill();
    }
    #endregion
}
