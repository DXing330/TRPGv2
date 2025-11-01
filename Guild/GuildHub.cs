using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildHub : MonoBehaviour
{
    public PartyDataManager partyData;
    public References references;
    public ActorSpriteHPList actorSpriteHPList;

    public void Start()
    {
        partyData.Save();
        RemoveChests();
        partyData.SetFullParty();
        actorSpriteHPList.RefreshData();
    }

    // Don't let them keep the chests if they don't complete the dungeon.
    protected void RemoveChests()
    {
        partyData.dungeonBag.ReturnAllChests();
    }
}
