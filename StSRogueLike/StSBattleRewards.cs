using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StSBattleRewards : MonoBehaviour
{
    public PartyDataManager partyData;
    public StSState mapState;
    public StatDatabase actorStats;
    public StatDatabase allEquipmentRewards;
    public Equipment dummyEquip;
    public int baseRarity = 2;
    public int rareRarity = 3;
    public int bossRarity = 4;
    public int baseGoldReward = 20; // For regular battles.
    public int rareGoldReward = 50; // For bosses.
    public int goldVariance = 6;
    public string basicAlly = "Grunt";
    public List<string> possibleAllies;
    public List<string> rareAllies;
    public int goldReward;
    public int GetGoldReward()
    {
        return goldReward;
    }
    public List<string> allyRewards;
    public List<string> GetAllyRewards()
    {
        return allyRewards;
    }
    public List<string> equipmentRewards;
    public List<string> GetEquipmentRewards()
    {
        return equipmentRewards;
    }
    public List<string> GetEquipmentRewardNames()
    {
        List<string> names = new List<string>();
        for (int i = 0; i < equipmentRewards.Count; i++)
        {
            dummyEquip.SetAllStats(equipmentRewards[i]);
            names.Add(dummyEquip.GetName());
        }
        return names;
    }

    public void ResetRewards()
    {
        goldReward = 0;
        allyRewards = new List<string>();
        equipmentRewards = new List<string>();
    }

    protected void ApplyRewards()
    {
        for (int i = 0; i < allyRewards.Count; i++)
        {
            partyData.HireMember(allyRewards[i], actorStats.ReturnValue(allyRewards[i]), allyRewards[i] + " " + Random.Range(0, 10000));
        }
        for (int i = 0; i < equipmentRewards.Count; i++)
        {
            partyData.equipmentInventory.AddEquipmentByStats(equipmentRewards[i]);
        }
        partyData.inventory.GainGold(goldReward);
    }

    public void GenerateRewards(string allRewards)
    {
        ResetRewards();
        string[] blocks = allRewards.Split("|");
        for (int i = 0; i < blocks.Length; i++)
        {
            GenerateReward(blocks[i]);
        }
        ApplyRewards();
    }

    public void GenerateReward(string specifics)
    {
        switch (specifics)
        {
            case "Gold":
                goldReward += baseGoldReward + Random.Range(-goldVariance, goldVariance * 2);
                break;
            case "Platinum":
                goldReward += rareGoldReward + Random.Range(-goldVariance, goldVariance * 2);
                break;
            case "Basic Ally":
                allyRewards.Add(basicAlly);
                break;
            case "Random Ally":
                allyRewards.Add(possibleAllies[Random.Range(0, possibleAllies.Count)]);
                break;
            case "Rare Ally":
                allyRewards.Add(rareAllies[Random.Range(0, rareAllies.Count)]);
                break;
            case "Equipment":
                equipmentRewards.Add(GetRandomEquipment(baseRarity));
                break;
            case "Rare Equipment":
                equipmentRewards.Add(GetRandomEquipment(rareRarity));
                break;
        }
    }

    public string GetRandomEquipment(int mininmumRarity = -1)
    {
        string rEquip = allEquipmentRewards.ReturnRandomValue();
        dummyEquip.SetAllStats(rEquip);
        if (dummyEquip.GetRarity() < mininmumRarity)
        {
            return GetRandomEquipment(mininmumRarity);
        }
        return rEquip;
    }
}
