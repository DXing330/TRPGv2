using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public DungeonBag dungeonBag;
    public List<string> keyValues;
    public List<StatTextText> currentInventoryStuff;
    public bool dungeon;
    public TMP_Text dungeonBagString;

    void Start()
    {
        UpdateKeyValues();
    }

    public void UpdateKeyValues()
    {
        for (int i = 0; i < currentInventoryStuff.Count; i++)
        {
            currentInventoryStuff[i].Reset();
            if (i >= keyValues.Count){break;}
            currentInventoryStuff[i].SetStatText(keyValues[i]);
            currentInventoryStuff[i].SetText(inventory.ReturnQuantityOfItem(keyValues[i]).ToString());
        }
        if (dungeon)
        {
            dungeonBagString.text = dungeonBag.ReturnBagLimitString();
        }
    }
}
