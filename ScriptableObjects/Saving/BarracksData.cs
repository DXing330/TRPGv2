using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Barracks", menuName = "ScriptableObjects/DataContainers/SavedData/Barracks", order = 1)]
public class BarracksData : PartyData
{
    public void AddFromParty(int index, PartyDataManager partyDataManager)
    {
        // Get the data from the PartyDataManager.mainPartyData
        List<string> allStats = partyDataManager.mainPartyData.GetStatsAtIndex(index);
        AddToBarracks(allStats[0],allStats[1],allStats[2],allStats[3],allStats[4],allStats[5]);
        partyDataManager.mainPartyData.RemoveStatsAtIndex(index);
        partyDataManager.Save();
        Save();
    }
    protected void AddToBarracks(string personalName, string spriteName, string baseStats, string equipment, string currentStats, string fee)
    {
        partyNames.Add(personalName);
        partySpriteNames.Add(spriteName);
        partyBaseStats.Add(baseStats);
        partyEquipment.Add(equipment);
        partyCurrentStats.Add(currentStats);
        battleFees.Add(fee);
    }

    public void AddFromBarracks(int index, PartyDataManager partyDataManager)
    {
        partyDataManager.mainPartyData.AddAllStats(partyNames[index], partySpriteNames[index], partyBaseStats[index], partyEquipment[index], partyCurrentStats[index], battleFees[index]);
        partyDataManager.Save();
        Save();
    }
}
