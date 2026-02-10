using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StSBattleRewards : MonoBehaviour
{
    public PartyDataManager partyData;
    public StSState mapState;
    public RNGUtility rewardRNGSeed;
    public StatDatabase stsRewardData;
    public StatDatabase skillBookRewards;
    public int baseGoldReward = 20; // For regular battles.
    public int rareGoldReward = 50; // For bosses.
    public int goldVariance = 6;
    protected void ResetRewards()
    {
        rewards.Clear();
        rewardOptions.Clear();
    }
    public SelectList claimRewardList;
    public void RefreshClaimableRewards()
    {
        // TODO determine the claim names based on the reward types.
        List<string> rewardNames = new List<string>();
        for (int i = 0; i < rewards.Count; i++)
        {
            rewardNames.Add(ReturnRewardName(rewards[i], rewardOptions[i]));
        }
        claimRewardList.SetSelectables(rewardNames);
    }
    protected string ReturnRewardName(string rewardType, string rewardSpecifics)
    {
        switch (rewardType)
        {
            default:
            return rewardType + " (" + rewardSpecifics + ")";
            case "Gold":
            return "Gold (" + rewardSpecifics + ")";
            case "SkillBook":
            return "Choose Skillbook";
            case "Ally":
            return "Choose New Ally";
        }
    }
    public RewardSelectMenu rewardSelect;
    public void ClickOnReward()
    {
        int index = claimRewardList.GetSelected();
        if (index < 0 || index >= rewards.Count){return;}
        switch (rewards[index])
        {
            default:
            ClaimReward(rewards[index], rewardOptions[index]);
            break;
            case "SkillBook":
            // TODO skill select pop up.
            break;
            case "Ally":
            // TODO ally select pop up.
            break;
        }
    }
    public void ClaimReward(string rewardType, string rewardChoice)
    {
        switch (rewardType)
        {
            case "Gold":
            partyData.inventory.GainGold(int.Parse(rewardChoice));
            break;
            case "Consumable":
            partyData.inventory.AddItemQuantity(rewardChoice);
            break;
            case "Ally":
            partyData.HireMemberBySpriteName(rewardChoice);
            break;
            case "SkillBook":
            partyData.spellBook.GainBook(rewardChoice);
            break;
            case "Relic":
            partyData.dungeonBag.GainItem(rewardChoice);
            break;
        }
        RemoveRewardAtIndex();
    }
    protected void RemoveRewardAtIndex()
    {
        // Selecting a reward removes the selection button.
        int index = claimRewardList.GetSelected();
        if (index < 0){return;}
        rewards.RemoveAt(index);
        rewardOptions.RemoveAt(index);
        RefreshClaimableRewards();
    }
    // Display all the rewards.
    public List<string> rewards;
    // Click a reward to claim it or select an option.
    public List<string> rewardOptions;

    public string GenerateRewardsByDifficulty(string battleType, int difficulty)
    {
        for (int i = difficulty; i >= 0; i--)
        {
            if (stsRewardData.KeyExists(battleType + "-" + i))
            {
                return stsRewardData.ReturnValue(battleType + "-" + i);
            }
        }
        return stsRewardData.ReturnValue(battleType);
    }

    public void GenerateRewards(string battleType)
    {
        string allRewards = GenerateRewardsByDifficulty(mapState.ReturnCurrentTile(), mapState.settings.GetDifficulty());
        string[] blocks = allRewards.Split("|");
        for (int i = 0; i < blocks.Length; i++)
        {
            GenerateReward(blocks[i]);
        }
        RefreshClaimableRewards();
    }

    public void GenerateReward(string specifics)
    {
        switch (specifics)
        {
            case "SkillBook":
                rewards.Add("SkillBook");
                rewardOptions.Add("TODO");
                break;
            case "RareSkillBook":
                rewards.Add("SkillBook");
                rewardOptions.Add("TODO");
                break;
            case "ConsumableChance":
                // ~ 30% chance for consumable?
                break;
            case "Consumable":
                rewards.Add("Consumable");
                rewardOptions.Add("TODO");
                break;
            case "Relic":
                rewards.Add("Relic");
                rewardOptions.Add("TODO");
                break;
            case "Gold":
                rewards.Add("Gold");
                rewardOptions.Add((baseGoldReward + rewardRNGSeed.Range(-goldVariance, goldVariance * 2)).ToString());
                break;
            case "Platinum":
                rewards.Add("Gold");
                rewardOptions.Add((rareGoldReward + rewardRNGSeed.Range(-goldVariance, goldVariance * 2)).ToString());
                break;
            case "Ally":
                break;
        }
    }
}
