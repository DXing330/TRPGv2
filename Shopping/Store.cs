using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Store : MonoBehaviour
{
    void Start(){LoadStore();}
    public string storeName;
    public PartyDataManager partyData;
    public InventoryUI inventoryUI;
    public StatDatabase storeData;
    protected void LoadStore()
    {
        string[] blocks = storeData.ReturnValue(storeName).Split("|");
        if (blocks.Length < 4){return;}
        equipmentSold = blocks[0].Split(",").ToList();
        equipmentPrices = blocks[1].Split(",").ToList();
        itemsSolds = blocks[2].Split(",").ToList();
        itemsPrices = blocks[3].Split(",").ToList();
        UpdateDisplay();
    }
    public List<string> equipmentSold;
    public List<string> equipmentPrices;
    public List<string> itemsSolds;
    public List<string> itemsPrices;
    public List<TMP_Text> itemsOwned;
    public List<TMP_Text> equipmentOwned;
    public SelectStatTextList equipmentDisplay;
    public SelectStatTextList itemsDisplay;
    protected void UpdateDisplay()
    {
        equipmentDisplay.SetStatsAndData(equipmentSold, equipmentPrices);
        itemsDisplay.SetStatsAndData(itemsSolds, itemsPrices);
        UpdateQuantityOwned();
    }
    protected void ResetQuantityOwned()
    {
        for (int i = 0; i < itemsOwned.Count; i++)
        {
            itemsOwned[i].text = "";
        }
        for (int i = 0; i < equipmentOwned.Count; i++)
        {
            equipmentOwned[i].text = "";
        }
    }
    protected void UpdateQuantityOwned()
    {
        ResetQuantityOwned();
        int quantity = 0;
        for (int i = 0; i < itemsSolds.Count; i++)
        {
            quantity = partyData.inventory.ReturnQuantityOfItem(itemsSolds[i]);
            itemsOwned[i].text = quantity.ToString()+"X";
        }
        for (int i = 0; i < equipmentSold.Count; i++)
        {
            quantity = partyData.equipmentInventory.ReturnEquipmentQuantity(equipmentSold[i]);
            equipmentOwned[i].text = quantity.ToString()+"X";
        }
    }
    public void TryToBuyItem()
    {
        int index = itemsDisplay.GetSelected();
        if (index == -1 || index >= itemsSolds.Count){return;}
        // Check the price.
        int price = int.Parse(itemsPrices[index]);
        if (!partyData.inventory.QuantityExists(price)){return;}
        // Pay the price.
        partyData.inventory.RemoveItemQuantity(price);
        // Get the item.
        partyData.inventory.AddItemQuantity(itemsSolds[index]);
        UpdateQuantityOwned();
        inventoryUI.UpdateKeyValues();
    }
    public void TryToBuyEquipment()
    {
        int index = equipmentDisplay.GetSelected();
        if (index == -1 || index >= equipmentSold.Count){return;}
        int price = int.Parse(equipmentPrices[index]);
        if (!partyData.inventory.QuantityExists(price)){return;}
        partyData.inventory.RemoveItemQuantity(price);
        partyData.equipmentInventory.AddEquipmentByName(equipmentSold[index]);
        UpdateQuantityOwned();
        inventoryUI.UpdateKeyValues();
    }
}
