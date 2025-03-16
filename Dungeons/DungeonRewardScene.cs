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
    public TacticActor dummyActor;
    public Equipment dummyEquip;
    public StatTextList allItemRewards;
    public StatTextList allSkillUps;
    public List<string> actorNames;
    public List<string> skillUpNames;
    public void AddSkillUp(string actor, string skill)
    {
        int indexOf = actorNames.IndexOf(actor);
        if (indexOf < 0)
        {
            actorNames.Add(actor);
            skillUpNames.Add(skill+"+1");
        }
        else
        {
            skillUpNames[indexOf] += ", "+skill+"+1";
        }
    }
    public GameObject questRewardPanel;
    public TMP_Text questGoldReward;
    public TMP_Text hirelingFees;
    public TMP_Text finalReward;
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
            //partyData.inventory.AddItemQuantity("Gold", questGold);
            questRewardPanel.SetActive(true);
            questGoldReward.text = questGold.ToString();
            CalculateSkillUps(questGold/10);
            CalculateSkillUps(questGold/10, false);
            // If you are successful then you must pay your hirelings their fair share.
            List<string> fees = partyData.mainPartyData.GetBattleFees();
            int totalFees = 0;
            for (int i = 0; i < fees.Count; i++)
            {
                totalFees += int.Parse(fees[i]);
            }
            questGold -= Mathf.Min(totalFees, questGold);
            hirelingFees.text = totalFees.ToString();
            finalReward.text = questGold.ToString();
            partyData.inventory.AddItemQuantity("Gold", questGold);
        }
        partyData.guildCard.GainGuildExp((questGold*questGold/100));
        partyData.guildCard.RefreshAll();
    }

    protected void CalculateSkillUps(int questDifficulty, bool main = true)
    {
        actorNames.Clear();
        skillUpNames.Clear();
        List<string> spriteNames = partyData.mainPartyData.GetSpriteNames();
        List<string> names = partyData.mainPartyData.GetNames();
        List<string> baseStats = partyData.mainPartyData.GetBaseStats();
        List<string> equipment = partyData.mainPartyData.GetEquipment();
        if (!main)
        {
            spriteNames = partyData.permanentPartyData.GetSpriteNames();
            names = partyData.permanentPartyData.GetNames();
            baseStats = partyData.permanentPartyData.GetBaseStats();
            equipment = partyData.permanentPartyData.GetEquipment();
        }
        for (int i = 0; i < spriteNames.Count; i++)
        {
            dummyActor.SetStatsFromString(baseStats[i]);
            dummyActor.ResetWeaponType();
            string[] equipData = equipment[i].Split("@");
            for (int j = 0; j < equipData.Length; j++)
            {
                dummyEquip.SetAllStats(equipData[j]);
                dummyEquip.EquipWeapon(dummyActor);
            }
            int passiveLevel = dummyActor.GetLevelFromPassive(spriteNames[i]);
            if (passiveLevel > 0 && passiveLevel < 4)
            {
                int RNG = Random.Range(0, (passiveLevel+1)*(passiveLevel+1)*(passiveLevel+1));
                Debug.Log(names[i]+";"+spriteNames[i]+", Current Level: "+passiveLevel+", Roll: "+RNG+"/"+((passiveLevel+1)*(passiveLevel+1))*(passiveLevel+1));
                if (RNG <= questDifficulty)
                {
                    dummyActor.SetLevelOfPassive(spriteNames[i], passiveLevel+1);
                    dummyActor.ReloadPassives();
                    baseStats[i] = dummyActor.GetStats();
                    AddSkillUp(names[i], spriteNames[i]);
                }
            }
            string weaponType = dummyActor.GetWeaponType()+" User";
            if (weaponType == " User"){continue;} // If no weapon is equipped then continue.
            passiveLevel = dummyActor.GetLevelFromPassive(weaponType);
            if (passiveLevel <= 0)
            {
                dummyActor.AddPassiveSkill(weaponType, "1");
                dummyActor.ReloadPassives();
                baseStats[i] = dummyActor.GetStats();
                AddSkillUp(names[i], weaponType);
            }
            else if (passiveLevel < 4)
            {
                int RNG = Random.Range(0, (passiveLevel+1)*(passiveLevel+1)*(passiveLevel+1));
                Debug.Log(names[i]+";"+weaponType+", Current Level: "+passiveLevel+", Roll: "+RNG+"/"+((passiveLevel+1)*(passiveLevel+1)*(passiveLevel+1)));
                if (RNG <= questDifficulty)
                {
                    dummyActor.SetLevelOfPassive(weaponType, passiveLevel+1);
                    dummyActor.ReloadPassives();
                    baseStats[i] = dummyActor.GetStats();
                    AddSkillUp(names[i], weaponType);
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
