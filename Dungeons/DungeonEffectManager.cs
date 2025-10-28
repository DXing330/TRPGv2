using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DungeonEffectManager : MonoBehaviour
{
    public SkillEffect basicEffects;
    public PartyDataManager partyData;
    public Dungeon dungeon;
    public DungeonMap dungeonMap;
    public StatDatabase itemData;
    public StatDatabase itemDescriptions;
    public StatDatabase trapData;
    public SelectList dungeonItemSelect;
    public TMP_Text useItemName;
    public TMP_Text useItemDescription;
    public string selectedItem;
    public void UpdateItemSelect()
    {
        dungeonItemSelect.SetSelectables(partyData.dungeonBag.GetItems());
    }
    public void SelectItem()
    {
        selectedItem = dungeonItemSelect.GetSelectedString();
        useItemName.text = selectedItem;
        useItemDescription.text = itemDescriptions.ReturnValue(selectedItem);
    }
    public void UseItem()
    {
        // Long switch statement incoming.
    }
}
