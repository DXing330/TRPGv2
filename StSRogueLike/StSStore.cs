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
            int price = rarity * (basePricePerRarity + Random.Range(-priceVariance, priceVariance + 1));
            availablePrices.Add(price.ToString());
        }
        rareEquipmentDisplay.SetStatsAndData(availableEquipmentNames, availablePrices);
    }
    // You get to hire a random mystery actor once per store.
    public InventoryUI rareItemInventoryUI;
    public ItemDetailViewer rareItemViewer;
    public List<string> hireableActors;
    public bool hiredActor = false;
    public int hiringCost = 60;
    public TMP_Text hireActorText;
    public StatDatabase actorStats;
    // You can buy as many regular equipment as you want.
    public SelectStatTextList rareEquipmentDisplay;
    public bool buyingRareEquipment = false;
    public List<string> availableEquipment;
    public List<string> availableEquipmentNames;
    public List<string> availablePrices;
    // You get to buy up to three rare equipment.
    public int limitedEquipmentCount = 3;
    public int basePricePerRarity = 20;
    public int priceVariance = 6;
    public StatDatabase equipmentStats;
    public Equipment dummyEquipment;
    // You get to buy a random two part spell once per store.
    // Or not, maybe make this an event.
    public StatDatabase spellStats;
    public bool learntSpell = false;
    public int spellCost = 30;
    public TMP_Text learntSpellText;

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
        if (hiredActor){ return; }
        // Check the cost.
        // Generate the actor.
        // Add them to the party.
        // Change the text and icon to reveal what you recruited.
        hiredActor = true;
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
}
