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
        references.Save();
        partyData.SetFullParty();
        actorSpriteHPList.RefreshData();
    }
}
