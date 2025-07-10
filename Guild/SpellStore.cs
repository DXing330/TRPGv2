using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpellStore : MonoBehaviour
{
    void Start()
    {
        UpdateCurrency();
    }
    public PartyDataManager partyData;
    public TMP_Text tomes;
    public string tomeString = "Tome";
    public TMP_Text mana;
    public string manaString = "Mana";
    public void UpdateCurrency()
    {
        tomes.text = partyData.inventory.ReturnQuantityOfItem(tomeString).ToString();
        mana.text = partyData.inventory.ReturnQuantityOfItem(manaString).ToString();
    }
}
