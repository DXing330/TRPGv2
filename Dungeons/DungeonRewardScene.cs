using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRewardScene : MonoBehaviour
{
    // Check treasures obtained, enemies defeated and quests completed.
    public Dungeon dungeon;
    // Obtain gold/items.
    public Inventory inventory;
    // Obtain equipment.
    public EquipmentInventory equipmentInventory;
    // Obtain skills.
    public PartyDataManager partyData;
    public StatTextList allItemRewards;
    public List<string> itemRewards;
    public List<int> rewardQuantities;

    void Start()
    {
        CalculateRewards();
        DisplayItemRewards();
        // Add the items to the inventory.
        //DisplayQuestRewards();
        //DisplayCombatRewards();
        ClaimRewards();
    }

    protected void DisplayItemRewards()
    {
        List<string> quantityStrings = new List<string>();
        for (int i = 0; i < rewardQuantities.Count; i++)
        {
            quantityStrings.Add(rewardQuantities[i].ToString());
        }
        allItemRewards.SetStatsAndData(itemRewards, quantityStrings);
    }

    protected void CalculateRewards()
    {
        itemRewards.Clear();
        rewardQuantities.Clear();
        for (int i = 0; i < dungeon.GetTreasuresAcquired(); i++)
        {
            int rewardType = Random.Range(0, dungeon.treasures.Count);
            int rewardAmount = Random.Range(1, int.Parse(dungeon.maxPossibleTreasureQuantities[rewardType])+1);
            AddItemReward(dungeon.treasures[rewardType], rewardAmount);
        }
    }

    protected void AddItemReward(string item, int amount)
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

    protected void ClaimRewards()
    {
        for (int i = 0; i < itemRewards.Count; i++)
        {
            inventory.AddItemQuantity(itemRewards[i], rewardQuantities[i]);
        }
    }

}
