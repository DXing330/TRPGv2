using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSuccessChecker : MonoBehaviour
{
    public bool QuestSuccessful(string questType, string specifics, PartyDataManager partyData)
    {
        switch (questType)
        {
            case "Find":
            return partyData.inventory.ItemExists(specifics);
        }
        // This only happens when the dungeon is completed, so default is true since if you cleared the dungeon you have defeated all required enemies.
        return true;
    }
}
