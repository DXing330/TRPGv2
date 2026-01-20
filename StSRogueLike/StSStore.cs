using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// You can buy randomly spawning equipment, actors and spells.
public class StSStore : Store
{
    protected override void Start()
    {
        base.Start();
        // Generate the limited equipment and prices.
        for (int i = 0; i < limitedEquipmentCount; i++)
        {
            // Get the equipment.
            string equipmentInfo = equipmentStats.ReturnRandomValue();
            dummyEquipment.SetAllStats(equipmentInfo);
            availableEquipment.Add(equipmentInfo);
            availableEquipmentNames.Add(dummyEquipment.GetName());
            // Check rarity.
            int rarity = dummyEquipment.GetRarity();
            // Generate price.
            if (stsState.settings.GetDifficulty() >= highDifficultyStores)
            {
                availablePrices.Add((rarity * (highDifficultyPrice + Random.Range(-priceVariance, priceVariance + 1))).ToString());
            }
            else
            {
                availablePrices.Add((rarity * (basePricePerRarity + Random.Range(-priceVariance, priceVariance + 1))).ToString());
            }
        }
        UpdateRareEquipDisplay();
    }
    public StSState stsState;
    public int highDifficultyStores = 16;
    // You get to hire a random mystery actor once per store.
    public InventoryUI rareItemInventoryUI;
    public ItemDetailViewer rareItemViewer;
    public List<string> hireableActors;
    public bool hiredActor = false;
    public int hiringCost = 60;
    public SpriteContainer actorSprites;
    public Image hireActorImage;
    public TMP_Text hireActorText;
    public StatDatabase actorStats;
    // You can buy as many regular equipment as you want.
    public SelectStatTextList rareEquipmentDisplay;
    public void UpdateRareEquipDisplay()
    {
        rareEquipmentDisplay.SetStatsAndData(availableEquipmentNames, availablePrices);
        rareItemInventoryUI.UpdateKeyValues();
        rareEquipmentDisplay.ResetSelected();
    }
    public bool buyingRareEquipment = false;
    public List<string> availableEquipment;
    public List<string> availableEquipmentNames;
    public List<string> availablePrices;
    // You get to buy up to three rare equipment.
    public int limitedEquipmentCount = 3;
    public int basePricePerRarity = 20;
    public int highDifficultyPrice = 30;
    public int priceVariance = 6;
    public StatDatabase equipmentStats;
    public Equipment dummyEquipment;
    // You get to buy a random two part spell once per store.
    // Or not, maybe make this an event, or just no spells, sword&board only.
    public StatDatabase spellStats;
    public bool learntSpell = false;
    public int spellCost = 30;
    public TMP_Text learntSpellText;
    // You can sell 1 equipment per store?
    public bool sellingEquipment = false;
    public int markUpPercentage = 20;
    public List<string> sellableEquipment;
    public List<string> sellableEquipmentNames;
    public List<string> sellablePrices;
    public GameObject sellEquipmentButtonObject;
    public GameObject sellEquipmentObject;
    public SelectStatTextList selectEquipmentToSell;
    public void TryToSellEquipment()
    {
        int index = selectEquipmentToSell.GetSelected();
        if (index < 0 || sellableEquipment.Count <= 0) { return; }
        // Get the money.
        partyData.inventory.GainGold(int.Parse(sellablePrices[index]));
        // Remove the equipment.
        partyData.equipmentInventory.RemoveEquipment(index);
        sellEquipmentButtonObject.SetActive(false);
        sellEquipmentObject.SetActive(false);
        selectEquipmentToSell.ResetSelected();
        rareItemInventoryUI.UpdateKeyValues();
        rareItemViewer.ResetView();
    }
    public void UpdateEquipmentToSell()
    {
        if (sellingEquipment)
        {
            sellEquipmentButtonObject.SetActive(false);
            sellEquipmentObject.SetActive(false);
            sellingEquipment = false;
            return;
        }
        rareItemViewer.ResetView();
        sellingEquipment = true;
        sellEquipmentButtonObject.SetActive(true);
        sellEquipmentObject.SetActive(true);
        sellableEquipment.Clear();
        sellableEquipmentNames.Clear();
        sellablePrices.Clear();
        List<string> allEquipment = partyData.equipmentInventory.GetAllEquipment();
        for (int i = 0; i < allEquipment.Count; i++)
        {
            dummyEquipment.SetAllStats(allEquipment[i]);
            sellableEquipment.Add(allEquipment[i]);
            sellableEquipmentNames.Add(dummyEquipment.GetName());
            // Check rarity.
            int rarity = dummyEquipment.GetRarity();
            // Generate price.
            int price = rarity * basePricePerRarity * (100 - markUpPercentage) / 100;
            sellablePrices.Add(price.ToString());
        }
        selectEquipmentToSell.SetStatsAndData(sellableEquipmentNames, sellablePrices);
    }

    public void ClickOnEquipmentToSell()
    {
        rareItemViewer.ViewEquip();
        rareItemViewer.ShowEquipmentInfo(sellableEquipment[selectEquipmentToSell.GetSelected()]);
    }

    public void ClickOnRareEquipment()
    {
        buyingEquipment = false;
        buyingItems = false;
        buyingRareEquipment = true;
        rareItemViewer.ViewEquip();
        rareItemViewer.ShowEquipmentInfo(availableEquipment[rareEquipmentDisplay.GetSelected()]);
    }
    public void ClickOnMysteryRecruit()
    {
        if (hiredActor) { return; }
        // Check the cost.
        if (!partyData.inventory.EnoughGold(hiringCost)) { return; }
        // Generate the actor.
        partyData.inventory.SpendGold(hiringCost);
        string randomActor = hireableActors[Random.Range(0, hireableActors.Count)];
        // Add them to the party.
        partyData.HireMember(actorStats.ReturnValue(randomActor), randomActor + " " + Random.Range(0, 999));
        // Change the text and icon to reveal what you recruited.
        hireActorText.text = "Thank you for your purchase of one (1) " + randomActor + ". No refunds, returns or exchanges.";
        hireActorImage.sprite = actorSprites.SpriteDictionary(randomActor);
        hiredActor = true;
        rareItemInventoryUI.UpdateKeyValues();
    }
    public void ClickOnMysterSpell()
    {
        if (learntSpell) { return; }
        // Check the cost.
        // Popup the list of actors.
        // Select one to learn the spell.
        // Generate the random spell.
        // Change the text and icon to reveal what you learnt.
        // Add it to the actor.
        learntSpell = true;
    }

    public void TryToBuyRareEquipment()
    {
        // If no equipment left then stop.
        if (availableEquipment.Count <= 0) { return; }
        int index = rareEquipmentDisplay.GetSelected();
        if (index < 0) { return; }
        // Check the cost.
        if (!partyData.inventory.EnoughGold(int.Parse(availablePrices[index]))) { return; }
        // Add the equipment to the party.
        partyData.inventory.SpendGold(int.Parse(availablePrices[index]));
        partyData.equipmentInventory.AddEquipmentByStats(availableEquipment[index]);
        // Remove the equipment.
        availableEquipment.RemoveAt(index);
        availablePrices.RemoveAt(index);
        availableEquipmentNames.RemoveAt(index);
        // Update the display.
        UpdateRareEquipDisplay();
    }
}
