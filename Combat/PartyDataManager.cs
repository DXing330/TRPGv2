using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyDataManager : MonoBehaviour
{
    public CharacterList party;
    public TacticActor dummyActor;
    public SavedData partyData;
    public SavedData playerData;
    public SavedData familiarData;

    public void UpdatePartyAfterBattle(List<TacticActor> actors)
    {

    }

    public void PartyDefeated()
    {
        party.ResetLists();
        // Add the player and familiar back to the party.

    }
}
