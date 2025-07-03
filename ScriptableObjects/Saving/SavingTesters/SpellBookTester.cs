using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBookTester : MonoBehaviour
{
    public SpellBook spellBook;
    public int testEffectCount = 6;

    [ContextMenu("Random Spell")]
    public string TestRandomSpell()
    {
        string spell = spellBook.ReturnRandomSpell(testEffectCount);
        Debug.Log(spell);
        return spell;
    }
}
