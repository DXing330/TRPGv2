using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellBook", menuName = "ScriptableObjects/DataContainers/SavedData/SpellBook", order = 1)]
public class SpellBook : SavedData
{
    public StatDatabase spellComponents;
    public MagicSpell magicSpell;
    // Passive names that determine how many spells slots a character has.
    public List<string> spellSlotPassives;
    public int ReturnActorSpellSlots(TacticActor actor)
    {
        int slots = 0;
        for (int i = 0; i < actor.GetPassiveSkills().Count; i++)
        {
            string passiveName = actor.GetPassiveAtIndex(i);
            if (spellSlotPassives.Contains(passiveName))
            {
                slots += utility.SumDescending(actor.GetLevelFromPassive(passiveName));
            }
        }
        return slots;
    }
    public List<string> currentComponents;
    // Constants.
    // Action cost and energy default to 1 cost.
    public List<string> rangeShapes;
    public List<string> GetRangeShapes()
    {
        return rangeShapes;
    }
    public List<string> effectShapes;
    public List<string> GetEffectShapes()
    {
        return effectShapes;
    }
    public List<string> ranges;
    public List<string> GetRanges()
    {
        return ranges;
    }
    public List<string> spans;
    public List<string> GetSpans()
    {
        return spans;
    }
    public List<string> powers;

    public string ReturnRandomSpell(int effectCount = 3)
    {
        string activeDelimiter = magicSpell.activeSkillDelimiter;
        string effectDelimiter = magicSpell.effectDelimiter;
        string spell = "SpellName" + activeDelimiter + "1" + activeDelimiter + "1" + activeDelimiter;
        spell += ranges[Random.Range(0, ranges.Count)] + activeDelimiter;
        spell += rangeShapes[Random.Range(0, rangeShapes.Count)] + activeDelimiter;
        spell += effectShapes[Random.Range(0, effectShapes.Count)] + activeDelimiter;
        spell += spans[Random.Range(0, spans.Count)] + activeDelimiter;
        string effects = "";
        string specifics = "";
        string newPowers = "";
        for (int i = 0; i < effectCount; i++)
        {
            string effectAndSpecifics = ReturnRandomEffectAndSpecifics();
            // split
            string[] blocks = effectAndSpecifics.Split(activeDelimiter);
            effects += blocks[0];
            specifics += blocks[1];
            newPowers += powers[Random.Range(0, powers.Count)];
            if (i < effectCount - 1)
            {
                effects += effectDelimiter;
                specifics += effectDelimiter;
                newPowers += effectDelimiter;
            }
        }
        spell += effects + activeDelimiter + specifics + activeDelimiter + newPowers + activeDelimiter;
        return spell;
    }

    public string ReturnRandomEffectAndSpecifics()
    {
        string effectAndSpecifics = "";
        string effect = spellComponents.ReturnRandomKey();
        List<string> possibleSpecifics = spellComponents.ReturnStats(effect);
        string specifics = possibleSpecifics[Random.Range(0, possibleSpecifics.Count)];
        effectAndSpecifics = effect + magicSpell.activeSkillDelimiter + specifics;
        return effectAndSpecifics;
    }

    public void CombineSpells(MagicSpell spell1, MagicSpell spell2, MagicSpell combinedSpell)
    {
        combinedSpell.LoadSkillFromString(spell1.GetSkillInfo());
        combinedSpell.AddEffects(spell2.GetEffect());
        combinedSpell.AddSpecifics(spell2.GetSpecifics());
        combinedSpell.AddPowers(spell2.GetAllPowersString());
    }
}
