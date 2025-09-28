using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmoryUI : MonoBehaviour
{
    public GeneralUtility utility;
    public Equipment dummyEquip;
    public EquipmentInventory equipmentInventory;
    public PartyDataManager partyData;
    public SelectStatTextList actorStats;
    public SelectStatTextList actorStatuses;
    public SelectStatTextList actorPassives;
    public SelectStatTextList actorActives;
    public SelectStatTextList actorEquipment;
    protected virtual void Start()
    {
        allActors.UpdateTextSize();
        actorStats.UpdateTextSize();
        actorPassives.UpdateTextSize();
        actorEquipment.UpdateTextSize();
        selectEquipment.UpdateTextSize();
    }
    public GameObject selectEquipObject;
    public SelectStatTextList selectEquipment;
    public ActorSpriteHPList allActors;
    public TacticActor selectedActor;
    public string selectedPassive;
    public string selectedPassiveLevel;
    public PassiveDetailViewer detailViewer;

    public virtual void ResetView()
    {
        actorStats.PublicResetPage();
        actorPassives.PublicResetPage();
        actorEquipment.ResetTextText();
    }

    protected void UpdateActorStats()
    {
        selectedActor.SetStatsFromString(allActors.allActorData[allActors.GetSelected()]);
        actorStats.UpdateActorStatTexts(selectedActor, true);
        actorPassives.UpdateActorPassiveTexts(selectedActor, partyData.ReturnPartyMemberEquipFromIndex(allActors.GetSelected()));
        actorEquipment.UpdateActorEquipmentTexts(partyData.ReturnPartyMemberEquipFromIndex(allActors.GetSelected()));
        actorEquipment.ResetSelected();
        actorStatuses.SetStatsAndData(selectedActor.GetUniqueStatuses(), selectedActor.GetUnqiueStatusStacks());
        UpdateActorActives();
    }
    
    protected void UpdateActorActives()
    {
        List<string> allActives = selectedActor.GetActiveSkills();
        // Go through all the passives.
        // For any that add actives at the start of battle, add those actives.
        List<string> allPassives = detailViewer.ReturnAllPassiveInfo(actorPassives.stats, actorPassives.data);
        for (int i = 0; i < allPassives.Count; i++)
        {
            string[] blocks = allPassives[i].Split("|");
            if (blocks.Length < 4){break;}
            if (blocks[4] == "Skill")
            {
                allActives.Add(blocks[5]);
            }
        }
        actorActives.SetStatsAndData(allActives);
    }

    public virtual void UpdateSelectedActor()
    {
        EndSelectingEquipment();
        detailViewer.DisablePanel();
        UpdateActorStats();
    }

    public virtual void UpdateSelectedActorWithCurrentHealth()
    {
        UpdateSelectedActor();
        actorStats.UpdateActorStatTexts(selectedActor, true);
    }

    public virtual void ViewPassiveDetails()
    {
        if (allActors.GetSelected() < 0) { return; }
        selectedPassive = actorPassives.statTexts[actorPassives.GetSelected()].GetStatText();
        selectedPassiveLevel = actorPassives.statTexts[actorPassives.GetSelected()].GetText();
        detailViewer.UpdatePassiveNames(selectedPassive, selectedPassiveLevel);
    }

    public virtual void BeginSelectingEquipment()
    {
        if (allActors.GetSelected() < 0){return;}
        if (actorEquipment.GetSelected() < 0){return;}
        switch (actorEquipment.GetSelected())
        {
            case 0:
            if (equipmentInventory.WeaponCount() <= 0){return;}
            selectEquipment.SetTitle("Weapons");
            selectEquipment.SetData(equipmentInventory.GetWeapons());
            break;
            case 1:
            if (equipmentInventory.ArmorCount() <= 0){return;}
            selectEquipment.SetTitle("Armor");
            selectEquipment.SetData(equipmentInventory.GetArmor());
            break;
            case 2:
            if (equipmentInventory.CharmCount() <= 0){return;}
            selectEquipment.SetTitle("Charms");
            selectEquipment.SetData(equipmentInventory.GetCharms());
            break;
        }
        selectEquipObject.SetActive(true);
        selectEquipment.UpdateEquipNames();
        selectEquipment.ResetSelected();
    }

    public virtual void EndSelectingEquipment()
    {
        selectEquipment.ResetSelected();
        selectEquipObject.SetActive(false);
        if (allActors.GetSelected() < 0){return;}
        selectedActor.SetStatsFromString(allActors.allActorData[allActors.GetSelected()]);
        actorStats.UpdateActorStatTexts(selectedActor, true);
        actorPassives.UpdateActorPassiveTexts(selectedActor, partyData.ReturnPartyMemberEquipFromIndex(allActors.GetSelected()));
        actorEquipment.UpdateActorEquipmentTexts(partyData.ReturnPartyMemberEquipFromIndex(allActors.GetSelected()));
        UpdateActorActives();
    }

    public virtual void PreviewEquippedPassives()
    {
        selectEquipment.ResetHighlights();
        selectEquipment.HighlightIndex(selectEquipment.GetSelected());
        selectedActor.SetStatsFromString(allActors.allActorData[allActors.GetSelected()]);
        actorPassives.UpdatePotentialPassives(selectedActor, partyData.ReturnPartyMemberEquipFromIndex(allActors.GetSelected()), selectEquipment.data[selectEquipment.GetSelected()]);
    }

    public virtual void ConfirmEquipSelection()
    {
        if (selectEquipment.GetSelected() < 0){return;}
        // take the selected index and pull the equipment from the equipment inventory
        string equipData = equipmentInventory.TakeEquipment(selectEquipment.GetSelected(), actorEquipment.GetSelected());
        // pass the new equip to the party data and equip it to the selected member
        string oldEquip = partyData.EquipToPartyMember(equipData, allActors.GetSelected(), dummyEquip);
        // get the old equip back and pass it to the equip inventory
        equipmentInventory.AddEquipmentByStats(oldEquip);
        // close the screen.
        EndSelectingEquipment();
    }

    public virtual void UnequipSelected()
    {
        if (allActors.GetSelected() < 0){return;}
        if (actorEquipment.GetSelected() < 0){return;}
        string slot = "";
        switch (actorEquipment.GetSelected())
        {
            case 0:
            slot = "Weapon";
            break;
            case 1:
            slot = "Armor";
            break;
            case 2:
            slot = "Charm";
            break;
        }
        string oldEquip = partyData.UnequipFromPartyMember(allActors.GetSelected(), slot, dummyEquip);
        equipmentInventory.AddEquipmentByStats(oldEquip);
        actorEquipment.ResetSelected();
        EndSelectingEquipment();
    }
}
