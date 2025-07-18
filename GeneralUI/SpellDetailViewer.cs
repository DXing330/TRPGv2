using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpellDetailViewer : ActiveDescriptionViewer
{
    public MagicSpell dummySpell;
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
    public void LoadSpell(string spellData)
    {
        dummySpell.LoadSkillFromString(spellData);
        // Update all the spells stats;
        spellEffects.text = ReturnSpellDescription(dummySpell);
        spellStats[0].SetText(dummySpell.ReturnManaCost().ToString());
        for (int i = 1; i < spellStats.Count; i++)
        {
            spellStats[i].SetText(dummySpell.GetStat(spellStatNames[i]));
        }
    }
}
