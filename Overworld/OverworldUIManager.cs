using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OverworldUIManager : MonoBehaviour
{
    public PartyDataManager partyData;
    public OverworldState overworldState;
    public OverworldMap overworldMap;
    public int state = 0;
    public List<string> stateNames;
    public List<int> statePanelIndexes;
    public void ChangeState(int newState)
    {
        if (state == newState){return;}
        panels[state].SetActive(false);
        panels[newState].SetActive(true);
        state = newState;
        // Update the panel that was just activated.
    }
    public List<GameObject> panels;
    // Caravan stats.
    public TMP_Text moveSpeedText;
    public List<StatImageText> moveSpeedTexts;
    public TMP_Text cargoWeightText;
    public void UpdateMoveSpeed()
    {
        int hourlySpeed = partyData.caravan.GetCurrentSpeed();
        string speedText = "";
        for (int i = 0; i < moveSpeedTexts.Count; i++)
        {
            speedText = "~"+(overworldMap.moveManager.ReturnMoveCostByIndex(i)/hourlySpeed)+" hours";
            moveSpeedTexts[i].SetText(speedText);
        }
    }
    public void UpdateCargoWeight()
    {
        cargoWeightText.text = partyData.caravan.GetCargoWeight()+" / "+partyData.caravan.GetMaxCarryWeight();
    }
    public void UpdateCaravanPanel()
    {
        UpdateCargoWeight();
        UpdateMoveSpeed();
    }
    public void UpdateCurrentPanel()
    {
        switch (state)
        {
            case 0:
            return;
            case 1:
            UpdateCaravanPanel();
            break;
        }
    }
    // Party stats.
    // Quest stats.
}
