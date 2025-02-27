using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Items are basically single use actives that always take one action.
// ex. Health potion is 
public class ItemSelectList : SelectList
{
    void Start()
    {
        UpdateUseableItems();
    }
    public BattleManager battle;
    public ActiveManager activeManager;
    public StatDatabase activeData;
    public Inventory inventory;
    public List<string> useableItems;
    public List<int> useableQuantities;
    // Go through all the inventory items, determine which ones are active skills.
    public void UpdateUseableItems()
    {
        useableItems.Clear();
        useableQuantities.Clear();
        for (int i = 0; i < inventory.items.Count; i++)
        {
            if (activeData.KeyExists(inventory.items[i]))
            {
                useableItems.Add(inventory.items[i]);
                useableQuantities.Add(int.Parse(inventory.quantities[i]));
            }
        }
        for (int i = useableItems.Count -1; i >= 0; i--)
        {
            if (useableQuantities[i] <= 0)
            {
                useableItems.RemoveAt(i);
                useableQuantities.RemoveAt(i);
            }
        }
    }
}
