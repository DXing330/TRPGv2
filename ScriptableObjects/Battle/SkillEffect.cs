using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillEffect", menuName = "ScriptableObjects/BattleLogic/SkillEffect", order = 1)]
public class SkillEffect : ScriptableObject
{
    public PassiveOrganizer passiveOrganizer;
    public int basicDenominator = 100;
    public int baseStatusDuration = 3;
    public void AffectActor(TacticActor target, string effect, string effectSpecifics, int level = 1)
    {
        int changeAmount = 0;
        switch (effect)
        {
            case "Temporary Passive":
                target.AddTempPassive(effectSpecifics, level);
                passiveOrganizer.OrganizeActorPassives(target);
                break;
            case "Status":
                int duration = level;
                if (level == 1) { duration = baseStatusDuration; }
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
            case "RemoveStatus":
                target.RemoveStatus(effectSpecifics);
                break;
            // Temp health is always a shield.
            case "TempHealth":
                target.UpdateTempHealth(int.Parse(effectSpecifics));
                break;
            // Default is increasing health.
            case "Health":
                target.UpdateHealth(int.Parse(effectSpecifics) * level, false);
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
            case "BaseEnergy":
                target.UpdateBaseEnergy(int.Parse(effectSpecifics) * level);
                target.UpdateEnergy(int.Parse(effectSpecifics) * level);
                break;
            case "BaseAttack":
                target.UpdateBaseAttack(int.Parse(effectSpecifics) * level);
                break;
            case "BaseAttack%":
                target.UpdateBaseAttack(level * (int.Parse(effectSpecifics) * target.GetBaseAttack()) / basicDenominator);
                break;
            case "BaseDefense":
                target.UpdateBaseDefense(int.Parse(effectSpecifics) * level);
                break;
            case "AttackRange":
                target.SetAttackRangeMax(int.Parse(effectSpecifics));
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
                string[] newSkills = effectSpecifics.Split(",");
                for (int i = 0; i < newSkills.Length; i++)
                {
                    target.AddActiveSkill(newSkills[i]);
                }
                break;
            case "Speed":
                target.UpdateSpeed(int.Parse(effectSpecifics) * level);
                break;
            case "BaseSpeed":
                target.SetMoveSpeedMax(int.Parse(effectSpecifics));
                break;
            case "Movement":
                target.GainMovement(level * int.Parse(effectSpecifics));
                break;
            case "BaseActions":
                target.UpdateBaseActions(level * int.Parse(effectSpecifics));
                break;
            case "Actions":
                target.AdjustActionAmount(level * int.Parse(effectSpecifics));
                break;
            case "MoveType":
                target.SetMoveType(effectSpecifics);
                break;
            case "Initiative":
                target.ChangeInitiative(int.Parse(effectSpecifics));
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
        }
    }
}
