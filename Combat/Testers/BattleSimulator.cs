using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSimulator : MonoBehaviour
{
    void Start()
    {
        partyOneList.ResetLists();
        partyTwoList.ResetLists();
        simulatorState.Load();
        partyOneSelect.RefreshData();
        partyTwoSelect.RefreshData();
        selectedActorName = "";
        actorSelect.SetData(actorStats.ReturnAllKeys(), actorStats.ReturnAllKeys(), actorStats.values);
    }
    public BattleSimulatorState simulatorState;
    // Determine the characters.
    public StatDatabase actorStats;
    public ActorSpriteHPList actorSelect;
    public string selectedActorName;
    public ActorSpriteHPList partyOneSelect;
    public ActorSpriteHPList partyTwoSelect;
    public CharacterList partyOneList;
    public CharacterList partyTwoList;
    public BattleManager battleManager;
    public GameObject battleManagerObject;
    public GameObject simulatorPanel;
    public void StartBattle()
    {
        // Don't start unless there are members on both sides.
        simulatorState.Save();
        if (partyOneList.characters.Count <= 0 || partyTwoList.characters.Count <= 0)
        {
            return;
        }
        simulatorPanel.SetActive(false);
        simulatorState.SetTerrainType();
        battleManagerObject.SetActive(true);
    }
    public void RemoveFromPartyOne()
    {
        if (partyOneSelect.GetSelected() < 0)
        {
            return;
        }
        partyOneList.RemoveFromParty(partyOneSelect.GetSelected());
        partyOneSelect.RefreshData();
    }
    public void ClearParty(int index)
    {
        if (index == 0)
        {
            partyOneList.ResetLists();
            partyOneSelect.RefreshData();
        }
        else
        {
            partyTwoList.ResetLists();
            partyTwoSelect.RefreshData();
        }
    }
    public void RemoveFromPartyTwo()
    {
        if (partyTwoSelect.GetSelected() < 0)
        {
            return;
        }
        partyTwoList.RemoveFromParty(partyTwoSelect.GetSelected());
        partyTwoSelect.RefreshData();
        // Disable what is needed.
    }
    public void SelectActorToAdd()
    {
        if (actorSelect.GetSelected() < 0)
        {
            return;
        }
        selectedActorName = actorStats.ReturnKeyAtIndex(actorSelect.GetSelected());
    }
    public void AddToPartyOne()
    {
        if (selectedActorName.Length <= 0)
        {
            return;
        }
        string stats = actorStats.ReturnValue(selectedActorName);
        partyOneList.AddMemberToParty(selectedActorName + " " + Random.Range(1, 999), stats, selectedActorName);
        partyOneSelect.RefreshData();
    }
    public void AddToPartyTwo()
    {
        if (selectedActorName.Length <= 0)
        {
            return;
        }
        string stats = actorStats.ReturnValue(selectedActorName);
        partyTwoList.AddMemberToParty(selectedActorName + " " + Random.Range(1, 999), stats, selectedActorName);
        partyTwoSelect.RefreshData();
    }
}
