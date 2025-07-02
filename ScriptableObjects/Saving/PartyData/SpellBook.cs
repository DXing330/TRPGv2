using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellBook", menuName = "ScriptableObjects/DataContainers/SavedData/SpellBook", order = 1)]
public class SpellBook : SavedData
{
    public StatDatabase spellComponents;
    public List<string> currentComponents;
    // Constants.
    // Action cost and energy default to 1 cost.
    public List<string> shapes;
    public List<string> ranges;
    public List<string> spans;
    public List<string> powers;

    public string ReturnRandomSpell(int effectCount = 3)
    {
        string spell = "SpellName|";
        return spell;
    }
}
