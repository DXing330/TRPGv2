using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmoryUI : MonoBehaviour
{
    public Equipment dummyEquip;
    public EquipmentInventory equipmentInventory;
    public PartyDataManager partyData;
    public SelectStatTextList actorStats;
    public SelectStatTextList actorPassives;
    public SelectStatTextList actorEquipment;
    void Start()
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

    public void UpdateSelectedActor()
    {
        EndSelectingEquipment();
        detailViewer.DisablePanel();
        selectedActor.SetStatsFromString(allActors.allActorData[allActors.GetSelected()]);
        actorStats.UpdateActorStatTexts(selectedActor);
        actorPassives.UpdateActorPassiveTexts(selectedActor, partyData.ReturnPartyMemberEquipFromIndex(allActors.GetSelected()));
        actorEquipment.UpdateActorEquipmentTexts(partyData.ReturnPartyMemberEquipFromIndex(allActors.GetSelected()));
        actorEquipment.ResetSelected();
    }

    public void ViewPassiveDetails()
    {
        // TODO: make this see the potential passives when selecting equipment
        if (allActors.GetSelected() < 0){return;}
        selectedPassive = actorPassives.statTexts[actorPassives.GetSelected()].GetStatText();
        selectedPassiveLevel = actorPassives.statTexts[actorPassives.GetSelected()].GetText();
        detailViewer.UpdatePassiveNames(selectedPassive, selectedPassiveLevel);
    }

    public void BeginSelectingEquipment()
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

    public void EndSelectingEquipment()
    {
        selectEquipment.ResetSelected();
        selectEquipObject.SetActive(false);
        if (allActors.GetSelected() < 0){return;}
        selectedActor.SetStatsFromString(allActors.allActorData[allActors.GetSelected()]);
        actorStats.UpdateActorStatTexts(selectedActor);
        actorPassives.UpdateActorPassiveTexts(selectedActor, partyData.ReturnPartyMemberEquipFromIndex(allActors.GetSelected()));
        actorEquipment.UpdateActorEquipmentTexts(partyData.ReturnPartyMemberEquipFromIndex(allActors.GetSelected()));
    }

    public void PreviewEquippedPassives()
    {
        selectEquipment.ResetHighlights();
        selectEquipment.HighlightIndex(selectEquipment.GetSelected());
        selectedActor.SetStatsFromString(allActors.allActorData[allActors.GetSelected()]);
        actorPassives.UpdatePotentialPassives(selectedActor, partyData.ReturnPartyMemberEquipFromIndex(allActors.GetSelected()), selectEquipment.data[selectEquipment.GetSelected()]);
    }

    public void ConfirmEquipSelection()
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

    public void UnequipSelected()
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
