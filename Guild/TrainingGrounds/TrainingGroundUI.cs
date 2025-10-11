using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Select an actor, view their stats and passives.
// Feats will be available all priced the same, feats you've already max leveled will no longer appear for selection.
// Select a feat to learn.
// Show what the feat will do for you, based on the level of the feat you will learn.
// Confirm that you want to learn the feat and pay the price.
// Learn the feat and save the game.
// Update the passive listing with the new feat.
public class TrainingGroundUI : MonoBehaviour
{
    public GeneralUtility utility;
    public PartyDataManager partyData;
    public ActorSpriteHPList allActors;
    public TacticActor selectedActor;
    public StatDatabase featData;
    public SelectStatTextList actorStats;
    public SelectStatTextList actorPassives;
    public PassiveDetailViewer detailViewer;
    public PopUpMessage errorPopUp;
    public PassiveDetailViewer featDetailViewer;
    public SelectStatTextList featSelect;
    public void UpdateAvailableFeats()
    {
        List<string> featNames = featData.GetAllKeys();
        List<string> featLevels = featData.GetAllValues();
        selectedActor.RemoveMaxLevelPassives(featNames, featLevels);
        featSelect.ResetSelected();
        featSelect.SetStatsAndData(featNames, featLevels);
    }
    public TMP_Text partyGold;
    public void UpdatePartyGold()
    {
        partyGold.text = partyData.inventory.ReturnGold().ToString();
    }
    public TMP_Text newFeatPrice;
    public void UpdateFeatPrice()
    {
        int featCount = 1 + selectedActor.GetTotalPassiveLevelsOfPassiveGroup(featData.GetAllKeys());
        newFeatPrice.text = (featCount * pricePerFeat).ToString();
    }
    public int pricePerFeat = 50;

    protected void UpdateActorStats()
    {
        selectedActor.SetStatsFromString(partyData.ReturnPartyMemberStatsAtIndex(allActors.GetSelected()));
        actorStats.UpdateActorStatTexts(selectedActor);
        actorPassives.UpdateActorPassiveTexts(selectedActor);
    }

    public virtual void UpdateSelectedActor()
    {
        detailViewer.DisablePanel();
        featDetailViewer.DisablePanel();
        UpdateActorStats();
        UpdatePartyGold();
        UpdateFeatPrice();
        UpdateAvailableFeats();
    }

    public virtual void ViewPassiveDetails()
    {
        if (allActors.GetSelected() < 0) { return; }
        string selectedPassive = actorPassives.statTexts[actorPassives.GetSelected()].GetStatText();
        string selectedPassiveLevel = actorPassives.statTexts[actorPassives.GetSelected()].GetText();
        detailViewer.UpdatePassiveNames(selectedPassive, selectedPassiveLevel);
    }

    public void ViewFeatDetails()
    {
        if (allActors.GetSelected() < 0) { return; }
        // If not enough gold then give error message.
        if (!partyData.inventory.QuantityExists(int.Parse(newFeatPrice.text)))
        {
            errorPopUp.SetMessage("Not enough gold to afford this training.");
            return;
        }
        featDetailViewer.UpdatePassiveNames(featSelect.GetSelectedStat(), featSelect.GetSelectedData());
    }

    public void LearnFeat()
    {
        string newFeat = featSelect.GetSelectedStat();
        int selectedIndex = allActors.GetSelected();
        selectedActor.SetStatsFromString(partyData.ReturnPartyMemberStatsAtIndex(allActors.GetSelected()));
        selectedActor.AddPassiveSkill(newFeat, "1");
        partyData.UpdatePartyMember(selectedActor, selectedIndex);
        partyData.inventory.RemoveItemQuantity(int.Parse(newFeatPrice.text));
        partyData.Save();
        UpdateSelectedActor();
    }
}
