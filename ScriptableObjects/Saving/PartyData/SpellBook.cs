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
    public List<string> shapes;
    public List<string> eShapes;
    public List<string> ranges;
    public List<string> spans;
    public List<string> powers;

    public string ReturnRandomSpell(int effectCount = 3)
    {
        string effectDelimiter = magicSpell.delimiter;
        string spell = "SpellName|1|1|";
        spell += ranges[Random.Range(0, ranges.Count)] + "|";
        spell += shapes[Random.Range(0, shapes.Count)] + "|";
        spell += eShapes[Random.Range(0, eShapes.Count)] + "|";
        spell += spans[Random.Range(0, spans.Count)] + "|";
        string effects = "";
        string specifics = "";
        string newPowers = "";
        for (int i = 0; i < effectCount; i++)
        {
            string effectAndSpecifics = ReturnRandomEffectAndSpecifics();
            // split
            string[] blocks = effectAndSpecifics.Split("|");
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
        spell += effects + "|" + specifics + "|" + newPowers + "|";
        return spell;
    }

    public string ReturnRandomEffectAndSpecifics()
    {
        string effectAndSpecifics = "";
        string effect = spellComponents.ReturnRandomKey();
        List<string> possibleSpecifics = spellComponents.ReturnStats(effect);
        string specifics = possibleSpecifics[Random.Range(0, possibleSpecifics.Count)];
        effectAndSpecifics = effect + "|" + specifics;
        return effectAndSpecifics;
    }
}
