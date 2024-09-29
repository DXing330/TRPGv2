using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : ScriptableObject
{
    public void AffectActor(TacticActor target, string effect, string effectSpecifics, int level = 1)
    {
        switch (effect)
        {
            case "Status":
            target.AddCondition(effectSpecifics, level);
            break;
            // Default is increasing health.
            case "Health":
            target.UpdateHealth(int.Parse(effectSpecifics)*level, false);
            break;
            case "Attack":
            target.UpdateAttack(int.Parse(effectSpecifics)*level, false);
            break;
            case "Defense":
            target.UpdateDefense(int.Parse(effectSpecifics)*level, false);
            break;
            case "BaseHealth":
            target.UpdateBaseHealth(int.Parse(effectSpecifics)*level, false);
            target.UpdateHealth(int.Parse(effectSpecifics)*level, false);
            break;
            case "BaseAttack":
            target.UpdateBaseAttack(int.Parse(effectSpecifics)*level, false);
            break;
            case "BaseDefense":
            target.UpdateBaseDefense(int.Parse(effectSpecifics)*level, false);
            break;
            case "Skill":
            // Add an active skill.
            string[] newSkills = effectSpecifics.Split(",");
            for (int i = 0; i < newSkills.Length; i++)
            {
                target.AddActiveSkill(newSkills[i]);
            }
            break;
        }
    }
}
