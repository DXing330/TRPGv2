using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpellDetailViewer : ActiveDescriptionViewer
{
    public MagicSpell dummySpell;
    public string spellData;
    [ContextMenu("Debug Spell Description")]
    public void ShowSpellDescription()
    {
        dummySpell.LoadSkillFromString(spellData);
        spellEffects.text = ReturnSpellDescription(dummySpell);
    }
    public List<string> spellStatNames;
    public TMP_Text spellEffects;
    public List<StatTextText> spellStats;
    public void ResetDetails()
    {
        spellEffects.text = "";
        for (int i = 0; i < spellStats.Count; i++)
        {
            spellStats[i].ResetText();
        }
    }
    public void LoadSpell(MagicSpell newSpell)
    {
        // Update all the spells stats;
        spellEffects.text = ReturnSpellDescription(newSpell);
        spellStats[0].SetText(newSpell.ReturnManaCost().ToString());
        for (int i = 1; i < spellStats.Count; i++)
        {
            spellStats[i].SetText(newSpell.GetStat(spellStatNames[i]));
        }
    }
}
