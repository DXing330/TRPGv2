using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CaravanStore : MonoBehaviour
{
    public TMP_Text gold;
    public TMP_Text totalWeight;
    public TMP_Text totalPull;
    public void UpdateCaravanStats()
    {
        gold.text = partyData.inventory.ReturnGold().ToString();
        totalWeight.text = partyData.caravan.GetCargoWeight()+" / "+partyData.caravan.GetMaxCarryWeight();
        totalPull.text = partyData.caravan.GetMaxPullWeight().ToString();
    }
    public List<string> suppliesSold;
    public SelectList supplySelectList;
    public List<StatTextText> supplyInfo;
    public TMP_Text ownedSupply;
    public List<string> mulesSold;
    public List<string> mulePrices;
    public SelectList muleSelectList;
    public List<StatTextText> muleInfo;
    public List<string> wagonsSold;
    public List<string> wagonPrices;
    public SelectList wagonSelectList;
    public List<StatTextText> wagonInfo;
    public StatDatabase supplyData;
    public StatDatabase muleData;
    public StatDatabase wagonData;
    public PartyDataManager partyData;
    public List<GameObject> storePanels;
    public int state = -1;
    public void ChangeState(int newState)
    {
        if (state == newState){state = -1;}
        else{state = newState;}
        UpdateStatePanels();
    }

    void Start()
    {
        suppliesSold = new List<string>(supplyData.keys);
        supplySelectList.SetSelectables(suppliesSold);
        muleSelectList.SetSelectables(mulesSold);
        wagonSelectList.SetSelectables(wagonsSold);
        UpdateCaravanStats();
    }

    public void BuyButton()
    {
        switch (state)
        {
            case -1:
            return;
            case 0:
            TryToBuySupply();
            break;
            case 1:
            TryToBuyMule();
            break;
            case 2:
            TryToBuyWagon();
            break;
        }
    }

    protected void UpdateStatePanels()
    {
        for (int i = 0; i < storePanels.Count; i++)
        {
            storePanels[i].SetActive(false);
        }
        if (state < 0){return;}
        storePanels[state].SetActive(true);
        switch (state)
        {
            case 0:
            supplySelectList.StartingPage();
            for (int i = 0; i < supplyInfo.Count; i++){supplyInfo[i].ResetText();}
            break;
            case 1:
            muleSelectList.StartingPage();
            for (int i = 0; i < muleInfo.Count; i++){muleInfo[i].ResetText();}
            break;
            case 2:
            wagonSelectList.StartingPage();
            for (int i = 0; i < wagonInfo.Count; i++){wagonInfo[i].ResetText();}
            break;
        }
    }

    public void SelectSupply()
    {
        int selected = supplySelectList.GetSelected();
        List<string> allStats = new List<string>(supplyData.ReturnStats(suppliesSold[selected]));
        for (int i = 0; i < supplyInfo.Count; i++){supplyInfo[i].SetText(allStats[i]);}
    }
    
    public void SelectMule()
    {
        int selected = muleSelectList.GetSelected();
        muleInfo[0].SetText(mulePrices[selected].ToString());
        List<string> allMuleStats = new List<string>(muleData.ReturnStats(mulesSold[selected]));
        for (int i = 1; i < muleInfo.Count; i++)
        {
            muleInfo[i].SetText(allMuleStats[i-1]);
        }
    }

    public void SelectWagon()
    {
        int selected = wagonSelectList.GetSelected();
        wagonInfo[0].SetText(wagonPrices[selected].ToString());
        List<string> allMuleStats = new List<string>(wagonData.ReturnStats(wagonsSold[selected]));
        for (int i = 1; i < wagonInfo.Count; i++)
        {
            wagonInfo[i].SetText(allMuleStats[i-1]);
        }
    }

    public bool EnoughMoney(int price)
    {
        if (!partyData.inventory.QuantityExists(price)){return false;}
        partyData.inventory.RemoveItemQuantity(price);
        return true;
    }

    public void TryToBuySupply()
    {
        // Check if enough money.
        // Add supply to cargo.
        // Update weight.
        int selected = supplySelectList.GetSelected();
        if (selected < 0){return;}
        int price = int.Parse(supplyInfo[0].text.text);
        if (EnoughMoney(price))
        {
            partyData.caravan.AddCargo(suppliesSold[selected], int.Parse(supplyInfo[2].text.text));
            UpdateCaravanStats();
        }
    }
    
    public void TryToBuyMule()
    {
        int selected = muleSelectList.GetSelected();
        if (selected < 0){return;}
        int price = int.Parse(muleInfo[0].text.text);
        if (EnoughMoney(price))
        {
            partyData.caravan.AddMule(muleData.ReturnValue(mulesSold[selected]));
            UpdateCaravanStats();
        }
    }

    public void TryToBuyWagon()
    {
        int selected = wagonSelectList.GetSelected();
        if (selected < 0){return;}
        int price = int.Parse(wagonInfo[0].text.text);
        if (EnoughMoney(price))
        {
            partyData.caravan.AddWagon(wagonData.ReturnValue(wagonsSold[selected]));
            UpdateCaravanStats();
        }
    }
}
