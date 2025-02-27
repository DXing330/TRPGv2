using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildHub : MonoBehaviour
{
    public PartyDataManager partyDataManager;
    public ActorSpriteHPList actorSpriteHPList;

    public void Start()
    {
        partyDataManager.Save();
        partyDataManager.SetFullParty();
        actorSpriteHPList.RefreshData();
    }
}
