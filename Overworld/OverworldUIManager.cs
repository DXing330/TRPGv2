using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OverworldUIManager : MonoBehaviour
{
    void Start()
    {
        cargoList.UpdateTextSize();
        dumpCargoList.UpdateTextSize();
    }
    public GeneralUtility utility;
    public PartyDataManager partyData;
    public SavedCaravan caravan;
    public OverworldState overworldState;
    public OverworldMap overworldMap;
    public int state = 0;
    public RequestDisplay requestDisplay;
    public List<string> requestTypes;
    public List<string> requestCompletedStrings;
    public TMP_Text requestCompletedText;
    public GameObject questCompletedPanel;
    public void UpdateRequestCompletedText(string requestType)
    {
        int indexOf = requestTypes.IndexOf(requestType);
        if (indexOf == -1) { return; }
        questCompletedPanel.SetActive(true);
        requestCompletedText.text = requestCompletedStrings[indexOf];
    }
    public StatTextList cargoList;
    public void UpdateCargoListWeight()
    {
        cargoList.SetStatsAndData(caravan.GetAllCargo(), caravan.GetAllCargoWeights());
        cargoList.SetTitle("Weight");
    }
    public SelectStatTextList dumpCargoList;
    public void Dump()
    {
        int selected = dumpCargoList.GetSelected();
        caravan.DumpCargo(selected);
        UpdateDumpCargoList();
    }
    public void UpdateDumpCargoList()
    {
        dumpCargoList.SetStatsAndData(caravan.GetAllCargo(), caravan.GetAllCargoQuantities());
        dumpCargoList.SetTitle("Quantity");
    }
    public StatDisplayList muleStats;
    public void UpdateMuleStats()
    {
        muleStats.SetStatsToDisplay(caravan.mules);
    }
    public StatDisplayList wagonStats;
    public void UpdateWagonStats()
    {
        wagonStats.SetStatsToDisplay(caravan.wagons);
    }
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
    public TMP_Text cargoPullText;
    public void UpdateMoveSpeed()
    {
        int hourlySpeed = partyData.caravan.GetCurrentSpeed();
        string speedText = "";
        for (int i = 0; i < moveSpeedTexts.Count; i++)
        {
            if (hourlySpeed <= 0)
            {
                moveSpeedTexts[i].SetText("\u221E" + " hours");
                continue;
            }
            speedText = "~" + (overworldMap.moveManager.ReturnMoveCostByIndex(i) / hourlySpeed) + " hours";
            moveSpeedTexts[i].SetText(speedText);
        }
    }
    public void UpdateCargoWeightAndPull()
    {
        cargoWeightText.text = partyData.caravan.GetCargoWeight() + " / " + partyData.caravan.GetMaxCarryWeight();
        cargoPullText.text = partyData.caravan.GetMaxPullWeight().ToString();
    }
    public void UpdateCaravanPanel()
    {
        UpdateCargoWeightAndPull();
        UpdateMoveSpeed();
        UpdateCargoListWeight();
        UpdateDumpCargoList();
        UpdateMuleStats();
        UpdateWagonStats();
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
            case 2:
                break;
            case 3:
                requestDisplay.ResetSelectedQuest();
                requestDisplay.DisplayQuest();
                break;
        }
    }
    // Party stats.
    // Quest stats.
}
