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
    public StatTextList allItemRewards;
    public StatTextList allSkillUps;
    public List<string> actorNames;
    public List<string> skillUpNames;
    public GameObject questRewardPanel;
    public TMP_Text questGoldReward;
    public QuestSuccessChecker questSuccessChecker;
    public int questGold;
    public List<string> itemRewards;
    public List<int> rewardQuantities;

    void Start()
    {
        CalculateRewards();
        DisplayItemRewards();
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
        CalculateQuestRewards();
    }

    protected void CalculateQuestRewards()
    {
        questGold = 0;
        questRewardPanel.SetActive(false);
        if (questSuccessChecker.QuestSuccessful(dungeon, partyData))
        {
            questGold = dungeon.GetQuestReward();
            partyData.inventory.AddItemQuantity("Gold", questGold);
            questRewardPanel.SetActive(true);
            questGoldReward.text = questGold.ToString();
            CalculateSkillUps(questGold/10);
        }
        partyData.guildCard.GainGuildExp((questGold*questGold/100));
        partyData.guildCard.RefreshAll();
    }

    protected void CalculateSkillUps(int questDifficulty)
    {
        actorNames.Clear();
        skillUpNames.Clear();
        List<string> spriteNames = partyData.mainPartyData.GetSpriteNames();
        List<string> names = partyData.mainPartyData.GetNames();
        List<string> baseStats = partyData.mainPartyData.GetBaseStats();
        // Skip the main party for now, they don't have specific class levels.
        for (int i = 0; i < spriteNames.Count; i++)
        {
            string[] allBaseStats = baseStats[i].Split("|");
            // Just hardcode it for now, I doubt the indices will change.
            string[] allPassives = allBaseStats[9].Split(",");
            string[] allPassiveLevels = allBaseStats[10].Split(",");
            for (int j = 0; j < allPassives.Length; j++)
            {
                // For now just try to level the class passives.
                if (allPassives[j] != spriteNames[i]){continue;}
                // All class passives have a cap of 4.
                int passiveLevel = int.Parse(allPassiveLevels[j]);
                if (passiveLevel >= 4){continue;}
                // Use RNG to determine if the passive levels up.
                int RNG = Random.Range(0, passiveLevel*passiveLevel);
                Debug.Log(names[i]+";"+spriteNames[i]+", Current Level: "+passiveLevel+", Roll: "+RNG+"/"+(passiveLevel*passiveLevel));
                // Higher quest difficulty means more likely to level up.
                if (RNG <= questDifficulty)
                {
                    allPassiveLevels[j] = (passiveLevel+1).ToString();
                    actorNames.Add(names[i]);
                    skillUpNames.Add(allPassives[j]+"++");
                    // Roll it all back up.
                    allBaseStats[10] = utility.ConvertArrayToString(allPassiveLevels, ",");
                    baseStats[i] = utility.ConvertArrayToString(allBaseStats);
                    break;
                }
            }
        }
        allSkillUps.SetStatsAndData(actorNames, skillUpNames);
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
