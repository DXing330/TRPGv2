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
        List<int> adjacentTiles = dungeonMap.AdjacentTilesToParty();
        switch (target)
        {
            default:
            break;
            case "AllEnemies":
                List<int> enemyLocations = dungeon.GetEnemyLocations();
                for (int i = 0; i < enemyLocations.Count; i++)
                {
                    AffectEnemyOnTile(enemyLocations[i], effect, specifics);
                }
                dungeonMap.UpdateMap();
                break;
            case "AdjacentEnemies":
            for (int i = 0; i < adjacentTiles.Count; i++)
                {
                    AffectEnemyOnTile(adjacentTiles[i], effect, specifics);
                }
                dungeonMap.UpdateMap();
                break;
            case "ClosestEnemy":
                // The map will get the closest enemy.
                AffectEnemyOnTile(dungeonMap.DetermineClosestEnemyLocation(), effect, specifics);
                dungeonMap.UpdateMap();
                break;
            case "Map":
                AffectMap(effect, specifics);
                dungeonMap.UpdateMap();
                break;
            case "Party":
                // Apply the effect to all party members.
                for (int i = 0; i < partyData.ReturnTotalPartyCount(); i++)
                {
                    // Need to check if this kills the main party. If it does then leave the dungeon.
                    if (AffectActor(partyData.ReturnActorAtIndex(i), effect, specifics, i))
                    {
                        dungeonMap.failureScreen.SetActive(true);
                        return;
                    }
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
                dungeonMap.UpdateMap();
                break;
            case "AllTraps":
                // What else would you do besides remove them?
                dungeon.ResetTraps();
                break;
            case "AllItems":
                // For now just claim them all. Can be expanded later.
                partyData.dungeonBag.GainItems(dungeon.ClaimAllItems());
                break;
            case "AllTiles":
                dungeon.UpdateFloorTiles(new List<int>());
                dungeonMap.UpdateMap();
                break;
            case "AdjacentTiles":
                dungeon.UpdateFloorTiles(adjacentTiles);
                dungeonMap.UpdateMap();
                break;
        }
    }

    protected bool AffectActor(TacticActor actor, string effect, string specifics, int index)
    {
        basicEffects.AffectActor(actor, effect, specifics);
        // A main party member has died, you lose.
        if (actor.GetHealth() <= 0 && index <= 1){return true;}
        partyData.UpdatePartyMember(actor, index);
        return false;
    }

    protected void AffectMap(string effect, string specifics)
    {
        switch (effect)
        {
            default:
            break;
            case "Escape":
            dungeonMap.EscapeDungeon();
            break;
            case "Reveal":
            RevealTiles(specifics);
            break;
            case "Teleport":
            TeleportToTile(specifics);
            break;
            case "Weather":
            dungeon.SetWeather(specifics);
            break;
        }
    }

    protected void TeleportToTile(string specifics)
    {
        switch (specifics)
        {
            default:
            break;
            case "Stairs":
            dungeonMap.TeleportToTile(dungeon.GetStairsDown());
            break;
            case "Random":
            dungeonMap.TeleportToTile(dungeon.ReturnRandomTile());
            break;
        }
    }

    protected void RevealTiles(string specifics)
    {
        switch (specifics)
        {
            default:
            break;
            case "All":
            dungeon.ViewAllTiles();
            break;
            case "Item":
            dungeon.UpdateViewedTiles(dungeon.GetItemLocations());
            break;
            case "Enemy":
            dungeon.UpdateViewedTiles(dungeon.GetEnemyLocations());
            break;
            case "Trap":
            dungeon.UpdateViewedTiles(dungeon.GetTrapLocations());
            break;
            case "Stairs":
            dungeon.UpdateViewedTiles(new List<int>(), dungeon.GetStairsDown());
            break;
            case "Treasure":
            dungeon.UpdateViewedTiles(dungeon.GetTreasureLocations());
            break;
        }
    }

    protected void AffectEnemyOnTile(int tileNumber, string effect, string specifics)
    {
        switch (effect)
        {
            default:
            break;
            case "Remove":
            dungeon.RemoveEnemyAtLocation(tileNumber);
            break;
            case "Transform":
            dungeon.TransformEnemyAtLocation(tileNumber, specifics);
            break;
            case "Teleport":
            dungeon.TeleportEnemyAtLocation(tileNumber, specifics);
            break;
        }
    }
}
