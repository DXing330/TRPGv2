using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// For selecting a skillbook.
public class RewardSelectMenu : MonoBehaviour
{
    public StSRewardSaveData rewardData;
    public List<string> rewardOptions;
    public List<SkillDisplay> skillDisplays;

    [ContextMenu("Test Reward Display")]
    public void TestDisplays()
    {
        rewardOptions = rewardData.GenerateSkillBookChoices();
        for (int i = 0; i < rewardOptions.Count; i++)
        {
            string[] blocks = rewardOptions[i].Split("_");
            skillDisplays[i].SetSkill(blocks[1], blocks[0]);
        }
    }
    [ContextMenu("Test Rare Reward")]
    public void TestRareDisplays()
    {
        rewardOptions = rewardData.GenerateSkillBookChoices(3, true);
        for (int i = 0; i < rewardOptions.Count; i++)
        {
            string[] blocks = rewardOptions[i].Split("_");
            skillDisplays[i].SetSkill(blocks[1], blocks[0]);
        }
    }
}
