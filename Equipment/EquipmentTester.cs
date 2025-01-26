using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentTester : MonoBehaviour
{
    public Equipment dummyEquip;
    public EquipmentInventory dummyInventory;
    public StatDatabase equipData;

    [ContextMenu("Gain Equipment")]
    public void GainEquipment(string equipName)
    {
        dummyInventory.AddEquipmentByName(equipName);
    }
}
