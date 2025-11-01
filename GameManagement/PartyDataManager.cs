using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyDataManager : MonoBehaviour
{
    void Start()
    {
        SetFullParty();
    }
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
    public DungeonBag dungeonBag;
    public GuildCard guildCard;
    public SavedCaravan caravan;
    public SpellBook spellBook;

    public void Save()
    {
        for (int i = 0; i < allParties.Count; i++) { allParties[i].Save(); }
        for (int i = 0; i < otherPartyData.Count; i++) { otherPartyData[i].Save(); }
        SetFullParty();
    }

    public void Load()
    {
        for (int i = 0; i < allParties.Count; i++) { allParties[i].Load(); }
        for (int i = 0; i < otherPartyData.Count; i++) { otherPartyData[i].Load(); }
        SetFullParty();
    }

    public void NewGame()
    {
        for (int i = 0; i < allParties.Count; i++) { allParties[i].NewGame(); }
        for (int i = 0; i < otherPartyData.Count; i++) { otherPartyData[i].NewGame(); }
    }

    // Since when starting a new run, the partydata is already set but you want to reset equipment and other stuff.
    public void OtherDataNewName()
    {
        for (int i = 0; i < otherPartyData.Count; i++) { otherPartyData[i].NewGame(); }
    }

    public virtual void NewDay(int dayCount)
    {
        for (int i = 0; i < otherPartyData.Count; i++) { otherPartyData[i].NewDay(dayCount); }
        // Pay the failure penalty for any failed quests.
        int penalty = guildCard.GetFailPenalty();
        if (penalty > 0)
        {
            if (inventory.QuantityExists(penalty))
            {
                inventory.RemoveItemQuantity(penalty);
            }
            else
            {
                inventory.LoseGold();
                // Maybe lose guild rank here.
            }
        }
        // Add exhaustion to all party members.
        for (int i = 0; i < allParties.Count; i++)
        {
            for (int j = allParties[i].PartyCount() - 1; j >= 0; j--)
            {
                allParties[i].Exhaust(j, i != 0);
            }
        }
        // People might die or get injured after a new day.
        // Or they might rest and heal if we're optimistic.
        SetFullParty();
    }

    public virtual void AddHours(int hours)
    {
        for (int i = 0; i < otherPartyData.Count; i++) { otherPartyData[i].AddHours(hours); }
    }

    public void RemoveExhaustion()
    {
        for (int i = 0; i < allParties.Count; i++)
        {
            for (int j = allParties[i].PartyCount() - 1; j >= 0; j--)
            {
                allParties[i].RemoveExhaustion(j);
            }
        }
    }

    public virtual void Rest()
    {
        for (int i = 0; i < otherPartyData.Count; i++) { otherPartyData[i].Rest(); }
        for (int i = 0; i < allParties.Count; i++)
        {
            for (int j = allParties[i].PartyCount() - 1; j >= 0; j--)
            {
                int hunger = 0;
                if (caravan.FoodAvailable())
                {
                    caravan.ConsumeFood();
                    allParties[i].Rest(j, true);
                }
                else
                {
                    allParties[i].Rest(j, false);
                    hunger = allParties[i].Hunger(j);
                }
            }
        }
    }

    public void NaturalRegeneration(List<string> regenPassives)
    {
        for (int i = 0; i < allParties.Count; i++)
        {
            allParties[i].NaturalRegeneration(regenPassives);
        }
    }

    public bool DungeonHunger()
    {
        bool permStarved = false;
        // Subtract 1 health from everyone.
        for (int i = 0; i < allParties.Count; i++)
        {
            for (int j = allParties[i].PartyCount() - 1; j >= 0; j--)
            {
                permStarved = allParties[i].HungerChipDamage(j, i != 0);
                if (permStarved){return true;}
            }
        }
        // Remove dead party members.
        SetFullParty();
        return false;
    }

    public bool StatusDamage(List<string> damagingStatuses)
    {
        bool permanentPartyDeath = false;
        // Subtract 1 health from everyone with certain statuses.
        for (int i = 0; i < allParties.Count; i++)
        {
            permanentPartyDeath = allParties[i].StatusChipDamage(damagingStatuses, i != 0);
            // This can only be true if i == 0
            if (permanentPartyDeath)
            {
                return true;
            }
        }
        // Remove dead party members.
        SetFullParty();
        return false;
    }

    public bool PartyMemberClassExists(string spriteName)
    {
        if (permanentPartyData.MemberExists(spriteName) || mainPartyData.MemberExists(spriteName)){ return true; }
        return false;
    }

    public void AddTempPartyMember(string name)
    {
        // Don't need stats, just grab base stats.
        tempPartyData.AddMember(name, actorStats.ReturnValue(name), name);
        SetFullParty();
    }

    public bool TempPartyMemberExists(string name)
    {
        return tempPartyData.MemberExists(name);
    }

    public void RemoveTempPartyMember(string name)
    {
        tempPartyData.RemoveMember(name);
        SetFullParty();
    }

    public bool OpenSlots()
    {
        // Default is 2 party members, plus 2 permanent for the classic 4?
        return mainPartyData.PartyCount() < guildCard.GetGuildRank() + 2;
    }

    public void HireMember(string spriteName, string stats, string personalName)
    {
        mainPartyData.AddMember(spriteName, stats, personalName);
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

    public int ReturnPartyMemberCurrentHealthFromIndex(int selected)
    {
        int permanentCount = permanentPartyData.PartyCount();
        int mainCount = mainPartyData.PartyCount();
        if (selected < permanentCount)
        {
            return permanentPartyData.GetCurrentHealthAtIndex(selected);
        }
        else if (selected < permanentCount + mainCount)
        {
            return mainPartyData.GetCurrentHealthAtIndex(selected - permanentCount);
        }
        else
        {
            return tempPartyData.GetCurrentHealthAtIndex(selected - permanentCount - mainCount);
        }
    }

    public string ReturnMainPartyEquipment(int selected)
    {
        return mainPartyData.partyEquipment[selected];
    }

    public int ReturnTotalPartyCount()
    {
        int count = 0;
        count += permanentPartyData.PartyCount();
        count += mainPartyData.PartyCount();
        count += tempPartyData.PartyCount();
        return count;
    }

    public TacticActor ReturnActorAtIndex(int index)
    {
        int permanentCount = permanentPartyData.PartyCount();
        int mainCount = mainPartyData.PartyCount();
        if (index < permanentCount)
        {
            return permanentPartyData.ReturnActorAtIndex(index);
        }
        else if (index < permanentCount + mainCount)
        {
            return mainPartyData.ReturnActorAtIndex(index - permanentCount);
        }
        else
        {
            return tempPartyData.ReturnActorAtIndex(index - permanentCount - mainCount);
        }
    }

    public void RenamePartyMember(string newInfo, int selected)
    {
        int permanentCount = permanentPartyData.PartyCount();
        int mainCount = mainPartyData.PartyCount();
        int tempCount = tempPartyData.PartyCount();
        if (selected < permanentCount)
        {
            permanentPartyData.ChangeName(newInfo, selected);
        }
        else if (selected < permanentCount + mainCount)
        {
            mainPartyData.ChangeName(newInfo, selected - permanentCount);
        }
        else
        {
            tempPartyData.ChangeName(newInfo, selected - permanentCount - mainCount);
        }
        SetFullParty();
    }

    public void AddSpellToPartyMember(string newInfo, int selected)
    {
        int permanentCount = permanentPartyData.PartyCount();
        int mainCount = mainPartyData.PartyCount();
        int tempCount = tempPartyData.PartyCount();
        if (selected < permanentCount)
        {
            permanentPartyData.MemberLearnsSpell(newInfo, selected);
        }
        else if (selected < permanentCount + mainCount)
        {
            mainPartyData.MemberLearnsSpell(newInfo, selected - permanentCount);
        }
        else
        {
            tempPartyData.MemberLearnsSpell(newInfo, selected - permanentCount - mainCount);
        }
    }

    public void UpdatePartyMember(TacticActor dummyActor, int selected)
    {
        int permanentCount = permanentPartyData.PartyCount();
        int mainCount = mainPartyData.PartyCount();
        int tempCount = tempPartyData.PartyCount();
        if (selected < permanentCount)
        {
            permanentPartyData.SetMemberStats(dummyActor, selected);
        }
        else if (selected < permanentCount + mainCount)
        {
            mainPartyData.SetMemberStats(dummyActor, selected - permanentCount);
        }
        else
        {
            tempPartyData.SetMemberStats(dummyActor, selected - permanentCount - mainCount);
        }
        SetFullParty();
    }

    public void RemovePartyMember(int selected)
    {
        int permanentCount = permanentPartyData.PartyCount();
        int mainCount = mainPartyData.PartyCount();
        int tempCount = tempPartyData.PartyCount();
        if (selected < permanentCount)
        {
            permanentPartyData.RemoveStatsAtIndex(selected);
        }
        else if (selected < permanentCount + mainCount)
        {
            mainPartyData.RemoveStatsAtIndex(selected - permanentCount);
        }
        else
        {
            tempPartyData.RemoveStatsAtIndex(selected - permanentCount - mainCount);
        }
        SetFullParty();
    }

    public void HealParty(bool full = true)
    {
        if (full)
        {
            permanentPartyData.ResetCurrentStats();
            mainPartyData.ResetCurrentStats();
            tempPartyData.ResetCurrentStats();
        }
        else
        {
            permanentPartyData.HalfRestore();
            mainPartyData.HalfRestore();
            tempPartyData.HalfRestore();
        }
        SetFullParty();
    }

    public void UpdatePartyAfterBattle(List<string> codeNames, List<string> spriteNames, List<string> stats)
    {
        for (int i = 0; i < allParties.Count; i++)
        {
            // Assume everyone dies at the end of every battle.
            allParties[i].ResetDefeatedMemberTracker();
        }
        // Match each code/spritename to an index.
        List<int> allIndices = new List<int>();
        // Remove the index after it have been taken.
        List<int> allPossibleIndices = new List<int>();
        for (int i = 0; i < ReturnTotalPartyCount(); i++)
        {
            allIndices.Add(-1);
            allPossibleIndices.Add(i);
        }
        // TODO: fix this in case some members have the same code name.
        // Issue if some members have both the same sprite and code name.
        // Need a mapping from stats to partyIndices.
        // For every code name.
        for (int i = 0; i < codeNames.Count; i++)
        {
            // Check if the code name and sprite name match with someone already in the party.
            // This will naturally exclude summons, assuming the summons don't have both the same code name and sprite name as a current actor.
            for (int j = allPossibleIndices.Count - 1; j >= 0; j--)
            {
                if (MatchCodeAndSpriteName(codeNames[i], spriteNames[i], allPossibleIndices[j]))
                {
                    allIndices[i] = allPossibleIndices[j];
                    allPossibleIndices.RemoveAt(j);
                    break;
                }
            }
        }
        for (int i = 0; i < Mathf.Min(ReturnTotalPartyCount(), codeNames.Count); i++)
        {
            UpdatePartyMemberAfterBattle(stats[i], allIndices[i]);
        }
        // Permanent Parties Members Survive With 1 HP, Main Character Power.
        permanentPartyData.ReviveDefeatedMembers();
        mainPartyData.RemoveDefeatedMembers();
        tempPartyData.RemoveDefeatedMembers();
        SetFullParty();
    }

    public void RemoveDeadPartyMembers()
    {
        mainPartyData.RemoveDeadMembers();
        tempPartyData.RemoveDeadMembers();
    }

    public string ReturnPartyMemberStatsAtIndex(int index)
    {
        if (index < 0) { return ""; }
        int permanentCount = permanentPartyData.PartyCount();
        int mainCount = mainPartyData.PartyCount();
        if (index < permanentCount)
        {
            return permanentPartyData.GetMemberStatsAtIndex(index);
        }
        else if (index < permanentCount + mainCount)
        {
            return mainPartyData.GetMemberStatsAtIndex(index - permanentCount);
        }
        else
        {
            return tempPartyData.GetMemberStatsAtIndex(index - permanentCount - mainCount);
        }
    }

    protected string CodeNameAtIndex(int index)
    {
        if (index < 0) { return ""; }
        int permanentCount = permanentPartyData.PartyCount();
        int mainCount = mainPartyData.PartyCount();
        if (index < permanentCount)
        {
            return permanentPartyData.GetNameAtIndex(index);
        }
        else if (index < permanentCount + mainCount)
        {
            return mainPartyData.GetNameAtIndex(index - permanentCount);
        }
        else
        {
            return tempPartyData.GetNameAtIndex(index - permanentCount - mainCount);
        }
    }

    protected string SpriteNameAtIndex(int index)
    {
        if (index < 0){ return ""; }
        int permanentCount = permanentPartyData.PartyCount();
        int mainCount = mainPartyData.PartyCount();
        if (index < permanentCount)
        {
            return permanentPartyData.GetSpriteNameAtIndex(index);
        }
        else if (index < permanentCount + mainCount)
        {
            return mainPartyData.GetSpriteNameAtIndex(index - permanentCount);
        }
        else
        {
            return tempPartyData.GetSpriteNameAtIndex(index - permanentCount - mainCount);
        }
    }

    protected bool MatchCodeAndSpriteName(string codeName, string spriteName, int index)
    {
        if (index < 0) { return false; }
        int permanentCount = permanentPartyData.PartyCount();
        int mainCount = mainPartyData.PartyCount();
        string testCodeName = "";
        string testSpriteName = "";
        if (index < permanentCount)
        {
            testCodeName = permanentPartyData.GetNameAtIndex(index);
            testSpriteName = permanentPartyData.GetSpriteNameAtIndex(index);
        }
        else if (index < permanentCount + mainCount)
        {
            testCodeName = mainPartyData.GetNameAtIndex(index - permanentCount);
            testSpriteName = mainPartyData.GetSpriteNameAtIndex(index - permanentCount);
        }
        else
        {
            testCodeName = tempPartyData.GetNameAtIndex(index - permanentCount - mainCount);
            testSpriteName = tempPartyData.GetSpriteNameAtIndex(index - permanentCount - mainCount);
        }
        return (testCodeName == codeName && testSpriteName == spriteName);
    }

    protected void UpdatePartyMemberAfterBattle(string stats, int index)
    {
        if (index < 0){ return; }
        int permanentCount = permanentPartyData.PartyCount();
        int mainCount = mainPartyData.PartyCount();
        int tempCount = tempPartyData.PartyCount();
        if (index < permanentCount)
        {
            permanentPartyData.SetCurrentStats(stats, index);
        }
        else if (index < permanentCount + mainCount)
        {
            mainPartyData.SetCurrentStats(stats, index - permanentCount);
        }
        else
        {
            tempPartyData.SetCurrentStats(stats, index - permanentCount - mainCount);
        }
    }

    public void PartyDefeated()
    {
        fullParty.ResetLists();
        permanentPartyData.ResetCurrentStats(true);
        mainPartyData.ClearAllStats();
        tempPartyData.ClearAllStats();
        Save();
        SetFullParty();
    }

    [ContextMenu("SetParty")]
    public void SetFullParty()
    {
        fullParty.ResetLists();
        fullParty.AddToParty(permanentPartyData.GetNames(), permanentPartyData.GetStats(), permanentPartyData.GetSpriteNames(), permanentPartyData.GetEquipmentStats());
        fullParty.AddToParty(mainPartyData.GetNames(), mainPartyData.GetStats(), mainPartyData.GetSpriteNames(), mainPartyData.GetEquipmentStats());
        fullParty.AddToParty(tempPartyData.GetNames(), tempPartyData.GetStats(), tempPartyData.GetSpriteNames(), tempPartyData.GetEquipmentStats());
    }
}
