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
        selectedActor.SetStatsFromString(allActors.actorData[allActors.GetSelected()]);
        actorStats.UpdateActorStatTexts(selectedActor);
        actorPassives.UpdateActorPassiveTexts(selectedActor, partyData.ReturnPartyMemberEquipFromIndex(allActors.GetSelected()));
        actorEquipment.UpdateActorEquipmentTexts(partyData.ReturnPartyMemberEquipFromIndex(allActors.GetSelected()));
    }

    public void ViewPassiveDetails()
    {
        if (allActors.GetSelected() < 0){return;}
        selectedPassive = actorPassives.stats[actorPassives.GetSelected()];
        selectedPassiveLevel = actorPassives.data[actorPassives.GetSelected()];
        detailViewer.UpdatePassiveNames(selectedPassive, selectedPassiveLevel);
    }

    public void BeginSelectingEquipment()
    {
        if (allActors.GetSelected() < 0){return;}
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
    }

    public void PreviewEquippedPassives()
    {

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
}
