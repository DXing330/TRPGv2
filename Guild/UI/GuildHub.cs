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

    public void Start()
    {
        partyData.Save();
        RemoveDungeonItems();
        partyData.SetFullParty();
        actorSpriteHPList.RefreshData();
        guildRank.text = partyData.guildCard.GetGuildRankName();
    }

    // Don't let them keep the chests if they don't complete the dungeon.
    protected void RemoveDungeonItems()
    {
        partyData.dungeonBag.ReturnAllChests();
        for (int i = 0; i < questItems.Count; i++)
        {
            partyData.dungeonBag.RemoveAllItemsOfType(questItems[i]);
        }
    }
}
