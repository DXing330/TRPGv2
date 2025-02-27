using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrackManager : MonoBehaviour
{
    public CharacterList mainParty;
    public PartyDataManager partyDataManager;
    public PartyData mainPartyData;
    public CharacterList barracksParty;
    public BarracksData barracksData;
    void Start()
    {
        mainPartyData = partyDataManager.mainPartyData;
        UpdateLists();
    }
    protected void UpdateLists()
    {
        mainParty.UpdateBasedOnPartyData(mainPartyData);
        barracksData.Load();
        barracksParty.UpdateBasedOnPartyData(barracksData);
        barracksActors.RefreshData();
        mainActors.RefreshData();
    }
    public TacticActor selectedActor;
    public ActorSpriteHPList mainActors;
    public ActorSpriteHPList barracksActors;
    public SelectStatTextList actorStats;
    public SelectStatTextList actorPassives;

    public void UpdateSelectedBarrackActor()
    {
        mainActors.ResetSelected();
        actorStats.ResetSelected();
        actorPassives.ResetSelected();
        selectedActor.SetStatsFromString(barracksActors.allActorData[barracksActors.GetSelected()]);
        actorStats.UpdateActorStatTexts(selectedActor);
        actorPassives.UpdateActorPassiveTexts(selectedActor, barracksData.partyEquipment[barracksActors.GetSelected()]);
    }

    public void UpdateSelectedMainActor()
    {
        barracksActors.ResetSelected();
        actorStats.ResetSelected();
        actorPassives.ResetSelected();
        selectedActor.SetStatsFromString(mainActors.allActorData[mainActors.GetSelected()]);
        actorStats.UpdateActorStatTexts(selectedActor);
        actorPassives.UpdateActorPassiveTexts(selectedActor, partyDataManager.ReturnMainPartyEquipment(mainActors.GetSelected()));
    }

    public void MoveToBarracks()
    {
        if (mainActors.GetSelected() < 0){return;}
        barracksData.AddFromParty(mainActors.GetSelected(), partyDataManager);
        UpdateLists();
        mainActors.ResetSelected();
        actorStats.ResetSelected();
        actorPassives.ResetSelected();
        barracksActors.RefreshData();
        mainActors.RefreshData();
    }

    public void AddToMainParty()
    {
        if (barracksActors.GetSelected() < 0){return;}
        // Make sure you don't make the party size larger than its allowed to be.
        if (!partyDataManager.OpenSlots()){return;}
        barracksData.AddFromBarracks(barracksActors.GetSelected(), partyDataManager);
        UpdateLists();
        barracksActors.ResetSelected();
        actorStats.ResetSelected();
        actorPassives.ResetSelected();
        barracksActors.RefreshData();
        mainActors.RefreshData();
    }
}
