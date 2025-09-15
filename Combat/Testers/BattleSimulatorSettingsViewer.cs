using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleSimulatorSettingsViewer : MonoBehaviour
{
    public BattleSimulatorState simulatorState;
    public void UpdateViewer()
    {
        UpdatemultiBattleSettings();

    }
    public TMP_Text multiBattleEnabledText;
    public TMP_Text multiBattleCountText;
    public void UpdatemultiBattleSettings()
    {
        if (simulatorState.multiBattle == 0)
        {
            multiBattleEnabledText.text = "False";
        }
        else
        {
            multiBattleEnabledText.text = "True";
        }
        multiBattleCountText.text = simulatorState.multiBattleCount.ToString();
    }
}
