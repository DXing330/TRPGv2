using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentRunes : MonoBehaviour
{
    public GeneralUtility utility;
    public EquipmentRunesUI runeGrid;
    public Equipment equipment;
    public string equipSlot;
    public void SetEquipmentStats(string newStats)
    {
        equipment.SetAllStats(newStats);
        ResetSelected();
        if (equipment.GetSlot() == equipSlot)
        {
            LoadRunes();
        }
    }
    public List<GameObject> runeObjects;
    public SpriteContainer runeSprites;
    public List<string> runeNames;
    public List<Image> runeImages;
    [ContextMenu("Update Rune Images")]
    public void UpdateRuneImages()
    {
        for (int i = 0; i < runeNames.Count; i++)
        {
            runeObjects[i].SetActive(true);
            runeImages[i].sprite = runeSprites.SpriteDictionary(runeNames[i]);
        }
    }
    protected void LoadRunes()
    {
        runeNames = new List<string>(equipment.GetRunes());
        utility.DisableGameObjects(runeObjects);
        UpdateRuneImages();
    }
    // Select List Stuff.
    public int selected = -1;
    public void ResetSelected(){selected = -1;}
    public void Select(int index)
    {
        selected = index;
        runeGrid.ViewRune(GetSelectedRune());
    }
    public string selectedRune;
    public string GetSelectedRune()
    {
        if (selected < 0 || selected >= runeNames.Count){return "";}
        return runeNames[selected];
    }
}
