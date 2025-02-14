using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public List<string> keyValues;
    public List<StatTextText> currentInventoryStuff;

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
    }
}
