using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyDataManager : MonoBehaviour
{
    // This is the one that the battle will actually read.
    public StatDatabase actorStats;
    public CharacterList fullParty;
    public List<PartyData> allParties;
    // For player + familiar.
    public PartyData permanentPartyData;
    // For hirelings + allies.
    public PartyData mainPartyData;
    // For quest party members (rescue/escort/etc)
    public PartyData tempPartyData;
    public List<SavedData> otherPartyData;
    public Inventory inventory;
    public EquipmentInventory equipmentInventory;

    public void Save()
    {
        for (int i = 0; i < allParties.Count; i++){allParties[i].Save();}
        for (int i = 0; i < otherPartyData.Count; i++){otherPartyData[i].Save();}
    }

    public void Load()
    {
        for (int i = 0; i < allParties.Count; i++){allParties[i].Load();}
        for (int i = 0; i < otherPartyData.Count; i++){otherPartyData[i].Load();}
        SetFullParty();
    }

    public void NewGame()
    {
        for (int i = 0; i < allParties.Count; i++){allParties[i].NewGame();}
        for (int i = 0; i < otherPartyData.Count; i++){otherPartyData[i].NewGame();}
    }

    public void AddTempPartyMember(string name)
    {
        // Don't need stats, just grab base stats.
        tempPartyData.AddMember(name, actorStats.ReturnValue(name), name);
        SetFullParty();
    }

    public void HireMember(string name, string stats, string personalName, string fee, string comp)
    {
        mainPartyData.AddMember(name, stats, personalName);
        mainPartyData.AddBattleFee(fee);
        mainPartyData.AddWorkerComp(comp);
        SetFullParty();
    }

    public string EquipToPartyMember(string equip, int selected, Equipment dummy)
    {
        int permanentCount = permanentPartyData.PartyCount();
        int mainCount = mainPartyData.PartyCount();
        int tempCount = tempPartyData.PartyCount();
        if (selected < permanentCount)
        {
            return permanentPartyData.EquipToMember(equip, selected, dummy);
        }
        else if (selected < permanentCount + mainCount)
        {
            return mainPartyData.EquipToMember(equip, selected - permanentCount, dummy);
        }
        else
        {
            return tempPartyData.EquipToMember(equip, selected - permanentCount - mainCount, dummy);
        }
    }

    public string UnequipFromPartyMember(int selected, string slot, Equipment dummy)
    {
        int permanentCount = permanentPartyData.PartyCount();
        int mainCount = mainPartyData.PartyCount();
        int tempCount = tempPartyData.PartyCount();
        if (selected < permanentCount)
        {
            return permanentPartyData.UnequipFromMember(selected, slot, dummy);
        }
        else if (selected < permanentCount + mainCount)
        {
            return mainPartyData.UnequipFromMember(selected - permanentCount, slot, dummy);
        }
        else
        {
            return tempPartyData.UnequipFromMember(selected - permanentCount - mainCount, slot, dummy);
        }
    }

    public string ReturnPartyMemberEquipFromIndex(int selected)
    {
        int permanentCount = permanentPartyData.PartyCount();
        int mainCount = mainPartyData.PartyCount();
        int tempCount = tempPartyData.PartyCount();
        if (selected < permanentCount)
        {
            return permanentPartyData.partyEquipment[selected];
        }
        else if (selected < permanentCount + mainCount)
        {
            return mainPartyData.partyEquipment[selected - permanentCount];
        }
        else
        {
            return tempPartyData.partyEquipment[selected - permanentCount - mainCount];
        }
    }

    public int ReturnHealingCost()
    {
        int healCost = 0;
        return healCost;
    }

    public void HealParty()
    {
        permanentPartyData.ResetCurrentStats();
        mainPartyData.ResetCurrentStats();
        tempPartyData.ResetCurrentStats();
        SetFullParty();
    }

    public void UpdatePartyAfterBattle(List<string> names, List<string> stats)
    {
        for (int i = 0; i < allParties.Count; i++){allParties[i].ClearCurrentStats();}
        int partyIndex = -1;
        int memberIndex = -1;
        for (int i = 0; i < names.Count; i++)
        {
            partyIndex = -1;
            memberIndex = -1;
            // Check which party they are in.
            for (int j = 0; j < allParties.Count; j++)
            {
                memberIndex = allParties[j].PartyMemberIndex(names[i]);
                if (memberIndex >= 0)
                {
                    partyIndex = j;
                    break;
                }
            }
            // Update their health if applicable.
            if (partyIndex >= 0)
            {
                allParties[partyIndex].SetCurrentStats(stats[i], memberIndex);
            }
        }
        // Permanent Parties Members Survive With 1 HP, Main Character Power.
        permanentPartyData.ReviveDefeatedMembers();
        mainPartyData.RemoveDefeatedMembers();
        tempPartyData.RemoveDefeatedMembers();
        // TODO: This isn't working inside the dungeon, party health isn't being tracked.
        SetFullParty();
    }

    public void PartyDefeated()
    {
        fullParty.ResetLists();
        permanentPartyData.ResetCurrentStats();
        mainPartyData.ResetCurrentStats();
        tempPartyData.ClearAllStats();
        SetFullParty();
    }

    public void SetFullParty()
    {
        fullParty.ResetLists();
        fullParty.AddToParty(permanentPartyData.GetNames(), permanentPartyData.GetStats(), permanentPartyData.GetSpriteNames(), permanentPartyData.GetEquipmentStats());
        fullParty.AddToParty(mainPartyData.GetNames(), mainPartyData.GetStats(), mainPartyData.GetSpriteNames(), mainPartyData.GetEquipmentStats());
        fullParty.AddToParty(tempPartyData.GetNames(), tempPartyData.GetStats(), tempPartyData.GetSpriteNames(), mainPartyData.GetEquipmentStats());
    }
}
