using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyDataManager : MonoBehaviour
{
    // This is the one that the battle will actually read.
    public CharacterList fullParty;
    public List<PartyData> allParties;
    public PartyData permanentPartyData;
    public PartyData mainPartyData;
    public PartyData tempPartyData;

    public void Save()
    {
        for (int i = 0; i < allParties.Count; i++){allParties[i].Save();}
    }

    public void Load()
    {
        for (int i = 0; i < allParties.Count; i++){allParties[i].Load();}
        SetFullParty();
    }

    public void NewGame()
    {
        for (int i = 0; i < allParties.Count; i++){allParties[i].NewGame();}
    }

    public void UpdatePartyAfterBattle(List<string> names, List<string> stats)
    {
        for (int i = 0; i < allParties.Count; i++){allParties[i].ClearCurrentStats();}
        int partyIndex = -1;
        int memberIndex = -1;
        for (int i = 0; i < names.Count; i++)
        {
            partyIndex = -1;
            memberIndex = -1;
            // Check which party they are in.
            for (int j = 0; j < allParties.Count; j++)
            {
                memberIndex = allParties[j].PartyMemberIndex(names[i]);
                Debug.Log(names[i]);
                Debug.Log(memberIndex);
                if (memberIndex >= 0)
                {
                    partyIndex = j;
                    break;
                }
            }
            // Update their health if applicable.
            if (partyIndex >= 0)
            {
                allParties[partyIndex].SetCurrentStats(stats[i], memberIndex);
            }
        }
        tempPartyData.RemoveDefeatedMembers();
    }

    public void PartyDefeated()
    {
        fullParty.ResetLists();
        permanentPartyData.ResetCurrentStats();
        mainPartyData.ResetCurrentStats();
        tempPartyData.ClearAllStats();
        SetFullParty();
    }

    public void SetFullParty()
    {
        fullParty.ResetLists();
        fullParty.AddToParty(permanentPartyData.GetNames(), permanentPartyData.GetStats(), permanentPartyData.GetSpriteNames());
        fullParty.AddToParty(mainPartyData.GetNames(), mainPartyData.GetStats(), mainPartyData.GetSpriteNames());
        fullParty.AddToParty(tempPartyData.GetNames(), tempPartyData.GetStats(), tempPartyData.GetSpriteNames());
    }
}
