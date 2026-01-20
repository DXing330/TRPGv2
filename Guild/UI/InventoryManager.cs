using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public PartyDataManager partyData;
    public Inventory inventory;
    // Keep track of how many items can be assigned to each party member here.
    // Default is 2 (hands), but some feats can add more.
}