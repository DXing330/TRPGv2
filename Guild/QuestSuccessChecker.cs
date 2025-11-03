using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSuccessChecker : MonoBehaviour
{
    public string escortName = "Noble";
    public bool QuestSuccessful(Dungeon dungeon, PartyDataManager partyData)
    {
        List<string> goals = dungeon.GetQuestGoals();
        for (int i = 0; i < goals.Count; i++)
        {
            switch (goals[i])
            {
                case "":
                return false;
                case "Search":
                // Check if your inventory includes the quest item.
                return false;
                case "Escort":
                return partyData.tempPartyData.PartyMemberIncluded(escortName);
                case "Rescue":
                return partyData.tempPartyData.PartyMemberIncluded(escortName);
            }
        }
        // This only happens when the dungeon is completed, so default is true since if you cleared the dungeon you have defeated all required enemies.
        return true;
    }
}
