using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StSTreasureScene : MonoBehaviour
{
    public PartyDataManager partyData;
    public StatDatabase rareEquipmentData;
    // If you beat a boss then you are guaranteed a very rare equipment.
    public int minimumRarity = 0;
    // How do we know if it is a treasure room or a boss treasure?
    public SavedData stsState;
    //stsState.ReturnCurrentTile()
    public Equipment dummyEquip;

    public string GetRandomEquipment()
    {
        string rEquip = rareEquipmentData.ReturnRandomValue();
        dummyEquip.SetAllStats(rEquip);
        if (dummyEquip.GetRarity() < minimumRarity)
        {
            return GetRandomEquipment();
        }
        return rEquip;
    }
}
