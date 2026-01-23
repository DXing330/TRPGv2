using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    void Start()
    {
        itemSelectList.SetSelectables(inventory.GetItems());
    }
    public PartyDataManager partyData;
    public void SetPartyData(PartyDataManager newParty)
    {
        partyData = newParty;
    }
    public Inventory inventory;
    public ItemDetailViewer itemDetails;
    public PopUpMessage popUp;
    // Keep track of how many items can be assigned to each party member here.
    protected int maxAssignedItems = 2;
    // Default is 2 (hands), but some feats can add more.
    //public List<string> extraItemSlotPassives; // Is there only one or a list?
    public int selectedActorID;
    public void SetSelectedID(int newID)
    {
        selectedActorID = newID;
    }
    public int selectedItemIndex;
    public void UnassignItem()
    {
        if (itemSelectList.GetSelected() < 0){return;}
        inventory.UnassignItem(selectedItemIndex);
        UpdateSelectedItem();
    }
    public void AssignToActor()
    {
        // If the actor already has the max assigned, then give an error message.
        if (itemSelectList.GetSelected() < 0){return;}
        int assignedCount = inventory.AssignedToIDCount(selectedActorID);
        if (assignedCount >= maxAssignedItems)
        {
            popUp.SetMessage("Cannot hold any more items at this time.");
            return;
        }
        inventory.AssignToActor(selectedItemIndex, selectedActorID);
        UpdateSelectedItem();
    }
    public SelectList itemSelectList;
    public void SelectItem()
    {
        if (itemSelectList.GetSelected() < 0){return;}
        selectedItemIndex = itemSelectList.GetSelected();
        UpdateSelectedItem();
    }
    public TMP_Text assignedToName;
    public void UpdateSelectedItem()
    {
        if (itemSelectList.GetSelected() < 0){return;}
        assignedToName.text = "";
        itemDetails.ShowInfo(itemSelectList.GetSelectedString());
        string ID = inventory.GetAssignedActorIDFromIndex(itemSelectList.GetSelected());
        if (ID == ""){return;}
        TacticActor assignedActor = partyData.ReturnActorFromID(int.Parse(ID));
        if (assignedActor == null){return;}
        assignedToName.text = assignedActor.GetPersonalName();
    }
}