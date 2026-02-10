using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RewardSelectMenu : MonoBehaviour
{
    public GameObject thisObject;
    public StSBattleRewards battleRewards;
    public string rewardType;
    public List<string> rewardChoices;
    public TMP_Text rewardTypeText;
    public void StartSelecting(string newType, List<string> newOptions)
    {
        rewardType = newType;
        rewardTypeText.text = "Select " + newType;
        rewardChoices = newOptions;
        thisObject.SetActive(true);
    }
    public void SelectReward(int choice)
    {

    }
    public void StopSelecting()
    {
        thisObject.SetActive(false);
    }
}
