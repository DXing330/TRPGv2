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
    public void DiscardItem()
    {
        partyData.dungeonBag.DiscardItem(selectedItem);
        UpdateItemSelect();
    }
    // 0 = target, 1 = effect, 2 = specifics.
    public void UseItem()
    {
        string[] itemEffect = itemData.ReturnValue(selectedItem).Split("|");
        switch (itemEffect[0])
        {
            case "Stomach":
                if (itemEffect[1] == "Increase")
                {
                    dungeon.IncreaseStomach(int.Parse(itemEffect[2]));
                }
                else
                {
                    dungeon.IncreaseStomach(-int.Parse(itemEffect[2]));
                }
                break;
        }
        partyData.dungeonBag.UseItem(selectedItem);
        UpdateItemSelect();
    }
}
