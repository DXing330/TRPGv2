using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Store : MonoBehaviour
{
    protected virtual void Start(){LoadStore();}
    public string storeName;
    public int baseEquipStock;
    public int baseItemStock;
    public int equipStockPerRank;
    public int itemStockPerRank;
    public PartyDataManager partyData;
    public InventoryUI inventoryUI;
    public ItemDetailViewer itemDetailViewer;
    public StatDatabase storeData;
    protected virtual void LoadStore()
    {
        string[] blocks = storeData.ReturnValue(storeName).Split("|");
        if (blocks.Length < 4){return;}
        equipmentSold = blocks[0].Split(",").ToList();
        equipmentPrices = blocks[1].Split(",").ToList();
        itemsSolds = blocks[2].Split(",").ToList();
        itemsPrices = blocks[3].Split(",").ToList();
        TrimStock();
        UpdateDisplay();
    }
    protected void TrimStock()
    {
        int rank = partyData.guildCard.GetGuildRank();
        for (int i = equipmentSold.Count - 1; i >= 0; i--)
        {
            if (i > baseEquipStock + (rank * equipStockPerRank))
            {
                equipmentSold.RemoveAt(i);
                equipmentPrices.RemoveAt(i);
                continue;
            }
            break;
        }
        for (int i = itemsSolds.Count - 1; i >= 0; i--)
        {
            if (i >= baseItemStock + (rank * itemStockPerRank))
            {
                itemsSolds.RemoveAt(i);
                itemsPrices.RemoveAt(i);
                continue;
            }
            break;
        }
    }
    public List<string> equipmentSold;
    public List<string> equipmentPrices;
    public List<string> itemsSolds;
    public List<string> itemsPrices;
    public List<TMP_Text> itemsOwned;
    public List<TMP_Text> equipmentOwned;
    public SelectStatTextList equipmentDisplay;
    public SelectStatTextList itemsDisplay;
    public bool buyingEquipment = false;
    public void ClickOnEquipment()
    {
        buyingEquipment = true;
        buyingItems = false;
        itemDetailViewer.ViewEquip();
        itemDetailViewer.ShowInfo(equipmentDisplay.GetSelectedStat());
        itemsDisplay.ResetSelected();
    }
    public bool buyingItems = false;
    public void ClickOnItem()
    {
        buyingEquipment = false;
        buyingItems = true;
        itemDetailViewer.ViewItem();
        itemDetailViewer.ShowInfo(itemsDisplay.GetSelectedStat());
        equipmentDisplay.ResetSelected();
    }
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
    public void UpdateQuantityOwned()
    {
        ResetQuantityOwned();
        int quantity = 0;
        // Needs to be based on the SelectStatTextList
        for (int i = 0; i < itemsDisplay.GetListLength(); i++)
        {
            string itemName = itemsDisplay.GetCurrentPageStat(i);
            if (itemName == ""){break;}
            quantity = partyData.inventory.ReturnQuantityOfItem(itemName);
            itemsOwned[i].text = quantity.ToString()+"X";
        }
        for (int i = 0; i < equipmentDisplay.GetListLength(); i++)
        {
            string equipName = equipmentDisplay.GetCurrentPageStat(i);
            if (equipName == "") { break; }
            quantity = partyData.equipmentInventory.ReturnEquipmentQuantity(equipName);
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

    public void TryToBuy()
    {
        if (buyingEquipment)
        {
            TryToBuyEquipment();
        }
        else if (buyingItems)
        {
            TryToBuyItem();
        }
    }
}
