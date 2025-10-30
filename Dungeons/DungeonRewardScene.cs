using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DungeonRewardScene : MonoBehaviour
{
    public GeneralUtility utility;
    // Check treasures obtained, enemies defeated and quests completed.
    public Dungeon dungeon;
    // Obtain gold/items.
    public Inventory inventory;
    // Obtain equipment.
    public EquipmentInventory equipmentInventory;
    // Obtain skills.
    public PartyDataManager partyData;
    public StatDatabase rewardData;
    public StatTextList allItemRewards;
    public StatTextList allEquipmentRewards;
    public GameObject questRewardPanel;
    public TMP_Text questGoldReward;
    public QuestSuccessChecker questSuccessChecker;
    public int questGold;
    public List<string> equipmentRewards;
    public TreasureChestManager treasureChestManager;

    void Start()
    {
        CalculateRewards();
        DisplayRewards();
        //DisplayCombatRewards();
        //ClaimRewards();
    }

    protected void CalculateRewards()
    {
        /*itemRewards.Clear();
        rewardQuantities.Clear();
        for (int i = 0; i < dungeon.GetTreasuresAcquired(); i++)
        {
            int rewardType = Random.Range(0, dungeon.treasures.Count);
            int rewardAmount = Random.Range(1, int.Parse(dungeon.maxPossibleTreasureQuantities[rewardType])+1);
            AddItemReward(dungeon.treasures[rewardType], rewardAmount);
        }*/
        treasureChestManager.OpenAllChests();
        CalculateQuestRewards();
        // Do you still get equipment from clearing dungeons?
        //CalculateEquipmentRewards();
    }

    protected void CalculateQuestRewards()
    {
        questGold = 0;
        questRewardPanel.SetActive(false);
        if (questSuccessChecker.QuestSuccessful(dungeon, partyData))
        {
            questGold = dungeon.GetQuestReward();
            questRewardPanel.SetActive(true);
            questGoldReward.text = questGold.ToString();
            partyData.inventory.AddItemQuantity("Gold", questGold);
        }
        partyData.guildCard.GainGuildExp((questGold*questGold/100));
        partyData.guildCard.RefreshAll();
    }

    protected void DisplayRewards()
    {
        allItemRewards.SetStatsAndData(treasureChestManager.GetItemsFound(), treasureChestManager.GetQuantitiesFound());
        allEquipmentRewards.SetStatsAndData(treasureChestManager.GetEquipmentFound());
    }

    // Handled by the treasure chests.
    /*protected void AddItemReward(string item, int amount)
    {
        int indexOf = itemRewards.IndexOf(item);
        if (indexOf == -1)
        {
            itemRewards.Add(item);
            rewardQuantities.Add(amount);
        }
        else
        {
            rewardQuantities[indexOf] = rewardQuantities[indexOf] + amount;
        }
    }

    protected void CalculateEquipmentRewards()
    {
        string equipRewards = rewardData.ReturnValue(dungeon.GetDungeonName());
        string[] blocks = equipRewards.Split("|");
        if (blocks[0] == "0")
        {
            return;
        }
        int minRarity = int.Parse(blocks[1]);
        int maxRarity = int.Parse(blocks[2]);
        equipmentRewards = new List<string>();
        for (int i = 0; i < int.Parse(blocks[0]); i++)
        {
            equipmentRewards.Add(ReturnRandomEquipmentOfRarity(minRarity, maxRarity));
        }
    }

        protected string ReturnRandomEquipmentOfRarity(int minRarity = 0, int maxRarity = 4)
    {
        string equip = equipmentData.ReturnRandomValue();
        dummyEquip.SetAllStats(equip);
        if (dummyEquip.GetRarity() <= maxRarity && dummyEquip.GetRarity() >= minRarity)
        {
            return equip;
        }
        return ReturnRandomEquipmentOfRarity(minRarity, maxRarity);
    }

    protected void ClaimRewards()
    {
        for (int i = 0; i < itemRewards.Count; i++)
        {
            inventory.AddItemQuantity(itemRewards[i], rewardQuantities[i]);
        }
        for (int i = 0; i < equipmentRewards.Count; i++)
        {
            partyData.equipmentInventory.AddEquipmentByStats(equipmentRewards[i]);
        }
    }*/
}
