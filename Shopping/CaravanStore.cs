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
        gold.text = partyData.inventory.GetGold().ToString();
        totalWeight.text = partyData.caravan.GetCargoWeight()+" / "+partyData.caravan.GetMaxCarryWeight();
        totalPull.text = partyData.caravan.GetMaxPullWeight().ToString();
    }
    public List<string> suppliesSold;
    public SelectList supplySelectList;
    public List<StatTextText> supplyInfo;
    public TMP_Text ownedSupply;
    public List<string> luxuriesSold;
    public SelectList luxurySelectList;
    public List<StatTextText> luxuryInfo;
    public TMP_Text ownedLuxury;
    public List<string> mulesSold;
    public StatDatabase mulePrices;
    public SelectList muleSelectList;
    public List<StatTextText> muleInfo;
    public List<string> wagonsSold;
    public StatDatabase wagonPrices;
    public SelectList wagonSelectList;
    public List<StatTextText> wagonInfo;
    public StatDatabase supplyData;
    public StatDatabase luxuryData;
    public SavedOverworld overworld; // Need overworld to determine increased/decreased prices for luxuries.
    public OverworldState overworldState; // Need state to determine city.
    public string suppliedLuxury = "";
    public string demandedLuxury = "";
    public int discountedPercentage = 80;
    public int premiumPercentage = 120;
    public int unitPrice;
    public void ResetSDLuxuries()
    {
        suppliedLuxury = "";
        demandedLuxury = "";
    }
    public void SetSDLuxuries()
    {
        ResetSDLuxuries();
        int currentLocation = overworldState.GetLocation();
        int cityIndex = overworld.cityLocations.IndexOf(currentLocation.ToString());
        if (cityIndex < 0) { return; }
        suppliedLuxury = overworld.cityLuxurySupplys[cityIndex];
        demandedLuxury = overworld.cityLuxuryDemands[cityIndex];
    }
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
        luxuriesSold = new List<string>(luxuryData.keys);
        mulesSold = new List<string>(mulePrices.keys);
        wagonsSold = new List<string>(wagonPrices.keys);
        supplySelectList.SetSelectables(suppliesSold);
        luxurySelectList.SetSelectables(luxuriesSold);
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
            case 3:
                TryToBuyLuxury();
                break;
        }
    }

    public void Sell(int state)
    {
        int selected = luxurySelectList.GetSelected();
        if (selected < 0){ return; }
        int currentAmount = int.Parse(ownedLuxury.text);
        if (currentAmount <= 0){ return;}
        string selectedLuxury = luxuriesSold[selected];
        List<string> allStats = new List<string>(luxuryData.ReturnStats(selectedLuxury));
        int soldAmount = 0;
        int unitPrice = int.Parse(allStats[0])/int.Parse(allStats[2]);
        switch (state)
        {
            case 0:
                soldAmount = 1;
                break;
            case 1:
                soldAmount = currentAmount / 2;
                break;
            case 2:
                soldAmount = currentAmount;
                break;
        }
        partyData.caravan.UnloadCargo(selectedLuxury, soldAmount);
        partyData.inventory.GainGold(soldAmount * unitPrice);
        UpdateCaravanStats();
        UpdateOwnedLuxury();
    }

    protected void UpdateStatePanels()
    {
        for (int i = 0; i < storePanels.Count; i++)
        {
            storePanels[i].SetActive(false);
        }
        if (state < 0) { return; }
        storePanels[state].SetActive(true);
        switch (state)
        {
            case 0:
                ownedSupply.text = "";
                supplySelectList.StartingPage();
                for (int i = 0; i < supplyInfo.Count; i++) { supplyInfo[i].ResetText(); }
                break;
            case 1:
                muleSelectList.StartingPage();
                for (int i = 0; i < muleInfo.Count; i++) { muleInfo[i].ResetText(); }
                break;
            case 2:
                wagonSelectList.StartingPage();
                for (int i = 0; i < wagonInfo.Count; i++) { wagonInfo[i].ResetText(); }
                break;
            case 3:
                ownedLuxury.text = "";
                luxurySelectList.StartingPage();
                for (int i = 0; i < luxuryInfo.Count; i++) { luxuryInfo[i].ResetText(); }
                break;
        }
    }

    public void SelectSupply()
    {
        int selected = supplySelectList.GetSelected();
        List<string> allStats = new List<string>(supplyData.ReturnStats(suppliesSold[selected]));
        for (int i = 0; i < supplyInfo.Count; i++) { supplyInfo[i].SetText(allStats[i]); }
        ownedSupply.text = partyData.caravan.ReturnItemQuantity(suppliesSold[selected]).ToString();
    }

    public void UpdateOwnedSupply()
    {
        int selected = supplySelectList.GetSelected();
        ownedSupply.text = partyData.caravan.ReturnItemQuantity(suppliesSold[selected]).ToString();
    }

    public void SelectLuxury()
    {
        int selected = luxurySelectList.GetSelected();
        string selectedLuxury = luxuriesSold[selected];
        List<string> allStats = new List<string>(luxuryData.ReturnStats(selectedLuxury));
        int price = int.Parse(allStats[0]);
        if (selectedLuxury == suppliedLuxury)
        {
            price = price * discountedPercentage / 100;
        }
        else if (selectedLuxury == demandedLuxury)
        {
            price = price * premiumPercentage / 100;
        }
        for (int i = 0; i < luxuryInfo.Count; i++) { luxuryInfo[i].SetText(allStats[i]); }
        luxuryInfo[0].SetText(price.ToString());
        ownedLuxury.text = partyData.caravan.ReturnItemQuantity(selectedLuxury).ToString();
    }

    public void UpdateOwnedLuxury()
    {
        int selected = luxurySelectList.GetSelected();
        string selectedLuxury = luxuriesSold[selected];
        ownedLuxury.text = partyData.caravan.ReturnItemQuantity(selectedLuxury).ToString();
    }
    
    public void SelectMule()
    {
        int selected = muleSelectList.GetSelected();
        muleInfo[0].SetText(mulePrices.ReturnValue(mulesSold[selected]));
        List<string> allMuleStats = new List<string>(muleData.ReturnStats(mulesSold[selected]));
        for (int i = 1; i < muleInfo.Count; i++)
        {
            muleInfo[i].SetText(allMuleStats[i - 1]);
        }
    }

    public void SelectWagon()
    {
        int selected = wagonSelectList.GetSelected();
        wagonInfo[0].SetText(wagonPrices.ReturnValue(wagonsSold[selected]));
        List<string> allMuleStats = new List<string>(wagonData.ReturnStats(wagonsSold[selected]));
        for (int i = 1; i < wagonInfo.Count; i++)
        {
            wagonInfo[i].SetText(allMuleStats[i-1]);
        }
    }

    public bool EnoughMoney(int price)
    {
        if (!partyData.inventory.EnoughGold(price)){return false;}
        partyData.inventory.SpendGold(price);
        return true;
    }

    public void TryToBuySupply()
    {
        int selected = supplySelectList.GetSelected();
        if (selected < 0){return;}
        int price = int.Parse(supplyInfo[0].text.text);
        if (EnoughMoney(price))
        {
            partyData.caravan.AddCargo(suppliesSold[selected], int.Parse(supplyInfo[2].text.text));
            UpdateCaravanStats();
            UpdateOwnedSupply();
        }
    }
    
    public void TryToBuyLuxury()
    {
        int selected = luxurySelectList.GetSelected();
        if (selected < 0){return;}
        int price = int.Parse(luxuryInfo[0].text.text);
        if (EnoughMoney(price))
        {
            partyData.caravan.AddCargo(luxuriesSold[selected], int.Parse(luxuryInfo[2].text.text));
            UpdateCaravanStats();
            UpdateOwnedLuxury();
        }
    }
    
    public void TryToBuyMule()
    {
        int selected = muleSelectList.GetSelected();
        if (selected < 0) { return; }
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
