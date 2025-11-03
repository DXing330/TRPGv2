using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GuildHub : MonoBehaviour
{
    public PartyDataManager partyData;
    public References references;
    public ActorSpriteHPList actorSpriteHPList;
    public TMP_Text guildRank;
    public List<string> questItems;
    public List<string> questTempMembers;

    public void Start()
    {
        RemoveDungeonData();
        partyData.SetFullParty();
        partyData.Save();
        actorSpriteHPList.RefreshData();
        guildRank.text = partyData.guildCard.GetGuildRankName();
    }

    // Don't let them keep the chests if they don't complete the dungeon.
    protected void RemoveDungeonData()
    {
        partyData.dungeonBag.ReturnAllChests();
        for (int i = 0; i < questItems.Count; i++)
        {
            partyData.dungeonBag.RemoveAllItemsOfType(questItems[i]);
        }
        for (int i = 0; i < questTempMembers.Count; i++)
        {
            partyData.RemoveAllTempPartyMember(questTempMembers[i]);
        }
    }
}
