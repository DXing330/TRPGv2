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
    public StatDatabase damagingStatus;
    public StatDatabase regenPassives;
    public int damageAmount;
    public bool ApplyDamagingStatus()
    {
        return partyData.StatusDamage(damagingStatus.GetAllKeys());
    }
    public void ApplyNaturalRegeneration()
    {
        partyData.NaturalRegeneration(regenPassives.GetAllKeys());
    }
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
        string[] targets = itemEffect[0].Split(",");
        string[] effects = itemEffect[1].Split(",");
        string[] specifics = itemEffect[2].Split(",");
        for (int i = 0; i < targets.Length; i++)
        {
            ApplyEffect(targets[i], effects[i], specifics[i]);
        }
        partyData.dungeonBag.UseItem(selectedItem);
        UpdateItemSelect();
    }

    protected void ApplyEffect(string target, string effect, string specifics)
    {
        switch (target)
        {
            default:
            break;
            case "ClosestEnemy":
                // The map will get the closest enemy.
                break;
            case "Map":
                AffectMap(effect, specifics);
                break;
            case "Party":
                // Apply the effect to all party members.
                for (int i = 0; i < partyData.ReturnTotalPartyCount(); i++)
                {
                    AffectActor(partyData.ReturnActorAtIndex(i), effect, specifics, i);
                }
                partyData.RemoveDeadPartyMembers();
                break;
            case "BattleMod":
                dungeon.AddPartyModifier(effect, int.Parse(specifics));
                break;
            case "Stomach":
                if (effect == "Increase")
                {
                    dungeon.IncreaseStomach(int.Parse(specifics));
                }
                else
                {
                    dungeon.IncreaseStomach(-int.Parse(specifics));
                }
                break;
        }
    }

    protected void AffectActor(TacticActor actor, string effect, string specifics, int index)
    {
        basicEffects.AffectActor(actor, effect, specifics);
        partyData.UpdatePartyMember(actor, index);
    }

    protected void AffectMap(string effect, string specifics)
    {
        switch (effect)
        {
            default:
            break;
            case "Weather":
            dungeon.SetWeather(specifics);
            break;
        }
    }

    protected void AffectEnemyOnTile(int tileNumber, string effect, string specifics)
    {
        
    }
}
