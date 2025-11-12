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
    }

    protected void CalculateRewards()
    {
        treasureChestManager.OpenAllChests();
        // Escape orb means you don't complete any quests.
        CalculateQuestRewards();
        if(questSuccessChecker.StoryQuestSuccessful(partyData, dungeon))
        {
            dungeon.mainStory.CompleteChapter();
            partyData.Save();
        }
    }

    protected void CalculateQuestRewards()
    {
        questGold = 0;
        questRewardPanel.SetActive(false);
        // Check which quests were active in the dungeon.
        List<int> indices = partyData.guildCard.GetQuestIndicesAtLocation(dungeon.GetDungeonName());
        for (int i = indices.Count - 1; i >= 0; i--)
        {
            if (questSuccessChecker.QuestSuccessful(partyData, i, dungeon))
            {
                questRewardPanel.SetActive(true);
                questGold += partyData.guildCard.GetQuestRewards()[i];
                questGoldReward.text = questGold.ToString();
                partyData.guildCard.CompleteRequest(i);
            }
        }
        if (questGold > 0)
        {
            partyData.inventory.AddItemQuantity("Gold", questGold);
            partyData.guildCard.GainGuildExp((questGold*questGold/100));
        }
    }

    protected void DisplayRewards()
    {
        allItemRewards.SetStatsAndData(treasureChestManager.GetItemsFound(), treasureChestManager.GetQuantitiesFound());
        allEquipmentRewards.SetStatsAndData(treasureChestManager.GetEquipmentFound());
    }
}
