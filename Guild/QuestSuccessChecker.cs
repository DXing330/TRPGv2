using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSuccessChecker : MonoBehaviour
{
    public bool QuestSuccessful(Dungeon dungeon, PartyDataManager partyData)
    {
        switch (dungeon.GetQuestGoal())
        {
            case "":
            return false;
            case "Search":
            return dungeon.questGoalsCompleted > 0;
            case "Escort":
            return partyData.tempPartyData.PartyMemberIncluded("Noble");
            case "Rescue":
            return partyData.tempPartyData.PartyMemberIncluded("Noble");
        }
        // This only happens when the dungeon is completed, so default is true since if you cleared the dungeon you have defeated all required enemies.
        return true;
    }
}
