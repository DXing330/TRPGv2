using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillEffect", menuName = "ScriptableObjects/BattleLogic/SkillEffect", order = 1)]
public class SkillEffect : ScriptableObject
{
    public PassiveOrganizer passiveOrganizer;
    public int basicDenominator = 100;
    public void AffectActor(TacticActor target, string effect, string effectSpecifics, int level = 1)
    {
        switch (effect)
        {
            case "Temporary Passive":
            target.AddTempPassive(effectSpecifics, level);
            passiveOrganizer.OrganizeActorPassives(target);
            break;
            case "Status":
            int duration = level;
            // Some statuses don't naturally wear off and are permanent.
            switch (effectSpecifics)
            {
                case "Poison":
                duration = -1;
                break;
                case "Burn":
                duration = -1;
                break;
            }
            target.AddStatus(effectSpecifics, duration);
            break;
            case "RemoveStatus":
            target.RemoveStatus(effectSpecifics);
            break;
            // Default is increasing health.
            case "Health":
            target.UpdateHealth(int.Parse(effectSpecifics)*level, false);
            break;
            case "Energy":
            target.UpdateEnergy(int.Parse(effectSpecifics)*level);
            break;
            case "Attack":
            target.UpdateAttack(int.Parse(effectSpecifics)*level);
            break;
            case "Defense":
            target.UpdateDefense(int.Parse(effectSpecifics)*level);
            break;
            case "AllStats":
            target.UpdateHealth(int.Parse(effectSpecifics)*level, false);
            target.UpdateAttack(int.Parse(effectSpecifics)*level);
            target.UpdateDefense(int.Parse(effectSpecifics)*level);
            break;
            case "BaseHealth":
            target.UpdateBaseHealth(int.Parse(effectSpecifics)*level, false);
            target.UpdateHealth(int.Parse(effectSpecifics)*level, false);
            break;
            case "BaseAttack":
            target.UpdateBaseAttack(int.Parse(effectSpecifics)*level);
            break;
            case "BaseDefense":
            target.UpdateBaseDefense(int.Parse(effectSpecifics)*level);
            break;
            case "AttackRange":
            target.SetAttackRangeMax(int.Parse(effectSpecifics));
            break;
            case "Health%":
            // % changes should not go below a minimum amount or else low stat characters are effectively immune.
            int changeAmount = level*int.Parse(effectSpecifics)*target.GetBaseHealth()/basicDenominator;
            if (Mathf.Abs(changeAmount) < Mathf.Abs(int.Parse(effectSpecifics)*level))
            {
                changeAmount = int.Parse(effectSpecifics);
            }
            target.UpdateHealth((changeAmount), false);
            break;
            case "Attack%":
            target.UpdateAttack(level*(int.Parse(effectSpecifics)*target.GetBaseAttack())/basicDenominator);
            break;
            case "Defense%":
            target.UpdateDefense(level*int.Parse(effectSpecifics)*target.GetBaseDefense()/basicDenominator);
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
            target.UpdateSpeed(int.Parse(effectSpecifics)*level);
            break;
            case "BaseSpeed":
            target.SetMoveSpeedMax(int.Parse(effectSpecifics));
            break;
            case "Movement":
            //target.GainMovement(level*int.Parse(effectSpecifics)*target.GetSpeed());
            target.GainMovement(level*int.Parse(effectSpecifics));
            break;
            case "Actions":
            target.AdjustActionAmount(level*int.Parse(effectSpecifics));
            break;
            case "MoveType":
            target.SetMoveType(effectSpecifics);
            break;
            case "Initiative":
            target.ChangeInitiative(int.Parse(effectSpecifics));
            break;
        }
    }
}
