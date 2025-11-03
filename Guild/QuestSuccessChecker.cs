using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSuccessChecker : MonoBehaviour
{
    public bool QuestSuccessful(PartyDataManager partyData, int index, Dungeon dungeon)
    {
        string goal = partyData.guildCard.GetQuestGoals()[index];
        string questItem = dungeon.GetSearchName();
        string escortName = dungeon.GetEscortName();
        switch (goal)
        {
            case "":
            return false;
            case "Search":
            // Check if your inventory includes the quest item.
            if (partyData.dungeonBag.ItemExists(questItem))
            {
                partyData.dungeonBag.UseItem(questItem);
                return true;
            }
            return false;
            case "Escort":
            if (dungeon.escaped){return false;}
            if (partyData.tempPartyData.PartyMemberIncluded(escortName))
            {
                partyData.RemoveTempPartyMember(escortName);
                return true;
            }
            return false;
            case "Rescue":
            if (partyData.tempPartyData.PartyMemberIncluded(escortName))
            {
                partyData.RemoveTempPartyMember(escortName);
                return true;
            }
            return false;
        }
        // This only happens when the dungeon is completed, so default is true since if you cleared the dungeon you have defeated all required enemies.
        if (dungeon.escaped){return false;}
        return true;
    }
}
