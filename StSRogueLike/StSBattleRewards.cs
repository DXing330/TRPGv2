using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StSBattleRewards : MonoBehaviour
{
    public PartyDataManager partyData;
    public NewAllySelect allySelect;
    public StSState mapState;
    public StatDatabase stsRewardData;
    public StSEnemyTracker enemyTracker;
    public StatDatabase actorStats;
    public StatDatabase allEquipmentRewards;
    public StatDatabase bossEquipment;
    public Equipment dummyEquip;
    public List<string> allyRewardRarity;
    public List<TMP_Text> allyRewardTexts;
    public void UpdateAllyRewardTexts()
    {
        for (int i = 0; i < allyRewards.Count; i++)
        {
            allyRewardTexts[i].text = allyRewards[i];
        }
    }
    public List<GameObject> allySelectButtons;
    public void BeginSelectingAlly(int index)
    {
        if (allyRewardRarity[index] == "")
        {
            allySelect.GetChoices();
        }
        else
        {
            allySelect.GetChoices(true);
        }
        allySelectButtons[index].SetActive(false);
    }
    public List<GameObject> equipmentSelectButtons;
    public int baseRarity = 2;
    public int rareRarity = 3;
    public int bossRarity = 4;
    public int baseGoldReward = 20; // For regular battles.
    public int rareGoldReward = 50; // For bosses.
    public int goldVariance = 6;
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

    public void ApplyRewards()
    {
        for (int i = 0; i < allyRewards.Count; i++)
        {
            partyData.HireMember(allyRewards[i], actorStats.ReturnValue(allyRewards[i]), allyRewards[i] + " " + Random.Range(0, 1000));
        }
        for (int i = 0; i < equipmentRewards.Count; i++)
        {
            partyData.equipmentInventory.AddEquipmentByStats(equipmentRewards[i]);
        }
        partyData.inventory.GainGold(goldReward);
    }

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

    public void GenerateRewards()
    {
        ResetRewards();
        string battleType = mapState.ReturnCurrentTile();
        string allRewards = GenerateRewardsByDifficulty(mapState.ReturnCurrentTile(), mapState.settings.GetDifficulty());
        string[] blocks = allRewards.Split("|");
        for (int i = 0; i < blocks.Length; i++)
        {
            GenerateReward(blocks[i]);
        }
        //ApplyRewards();
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
                allyRewardRarity.Add("");
                allySelectButtons[allyRewardRarity.Count - 1].SetActive(true);
                break;
            case "Random Ally":
                allyRewardRarity.Add("");
                allySelectButtons[allyRewardRarity.Count - 1].SetActive(true);
                break;
            case "Rare Ally":
                allyRewardRarity.Add("Rare");
                allySelectButtons[allyRewardRarity.Count - 1].SetActive(true);
                break;
            case "Equipment":
                equipmentRewards.Add(GetRandomEquipment(baseRarity));
                break;
            case "Rare Equipment":
                equipmentRewards.Add(GetRandomEquipment(rareRarity));
                break;
            case "Boss Equipment":
                equipmentRewards.Add(bossEquipment.ReturnRandomValue());
                break;
        }
    }

    public void AddAllyReward(string newAlly)
    {
        allyRewards.Add(newAlly);
        UpdateAllyRewardTexts();
    }

    public void AddEquipmentReward(string newEquip)
    {
        equipmentRewards.Add(newEquip);
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
