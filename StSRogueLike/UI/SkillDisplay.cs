using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// For Displaying Skillbook Rewards
public class SkillDisplay : MonoBehaviour
{
    public ActiveDescriptionViewer descriptionViewer;
    public SpellDetailViewer spellViewer;
    public PassiveDetailViewer passiveViewer;
    public TMP_Text skillName;
    public TMP_Text typeName;
    public TMP_Text skillDescription;

    public void SetSkill(string newName, string skillType = "Skill")
    {
        skillName.text = newName;
        typeName.text = "[" + skillType + "]";
        switch (skillType)
        {
            // Skill
            default:
            skillDescription.text = descriptionViewer.ReturnActiveDescriptionFromName(newName);
            break;
            case "Passive":
            skillDescription.text = passiveViewer.ReturnSpecificPassiveLevelEffect(newName, 1);
            break;
            case "Spell":
            skillDescription.text = spellViewer.ReturnSpellDescriptionFromName(newName);
            break;
        }
    }
}
