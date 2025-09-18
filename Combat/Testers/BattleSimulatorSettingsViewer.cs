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
        UpdateBattleSettings();
        UpdateSelectedTerrain();
        UpdateSelectedWeather();
        UpdateSelectedTime();
    }
    public TMP_Text multiBattleEnabledText;
    public TMP_Text multiBattleCountText;
    public TMP_Text autoBattleEnabledText;
    public TMP_Text controlAIEnabledText;
    public void UpdateBattleSettings()
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
        if (simulatorState.autoBattle == 0)
        {
            autoBattleEnabledText.text = "False";
        }
        else
        {
            autoBattleEnabledText.text = "True";
        }
        if (simulatorState.controlAI == 0)
        {
            controlAIEnabledText.text = "False";
        }
        else
        {
            controlAIEnabledText.text = "True";
        }
    }
    public SelectStatTextList terrainSelect;
    public void SelectTerrain()
    {
        simulatorState.SelectTerrainType(terrainSelect.GetSelected());
        UpdateViewer();
    }
    public void UpdateSelectedTerrain()
    {
        List<string> all = new List<string>(simulatorState.allTerrainTypes);
        List<string> active = new List<string>();
        for (int i = 0; i < all.Count; i++)
        {
            if (simulatorState.selectedTerrainTypes.Contains(all[i]))
            {
                active.Add("Allowed");
            }
            else
            {
                active.Add("Not Allowed");
            }
        }
        int page = terrainSelect.GetPage();
        terrainSelect.SetStatsAndData(all, active);
        terrainSelect.SetPage(page);
    }
    public SelectStatTextList weatherSelect;
    public void SelectWeather()
    {
        simulatorState.SelectWeather(weatherSelect.GetSelected());
        UpdateViewer();
    }
    public void UpdateSelectedWeather()
    {
        List<string> all = new List<string>(simulatorState.allWeathers);
        List<string> active = new List<string>();
        for (int i = 0; i < all.Count; i++)
        {
            if (simulatorState.selectedWeathers.Contains(all[i]))
            {
                active.Add("Allowed");
            }
            else
            {
                active.Add("Not Allowed");
            }
        }
        int page = weatherSelect.GetPage();
        weatherSelect.SetStatsAndData(all, active);
        weatherSelect.SetPage(page);
    }
    public SelectStatTextList timeSelect;
    public void SelectTime()
    {
        simulatorState.SelectTime(timeSelect.GetSelected());
        UpdateViewer();
    }
    public void UpdateSelectedTime()
    {
        List<string> all = new List<string>(simulatorState.allTimes);
        List<string> active = new List<string>();
        for (int i = 0; i < all.Count; i++)
        {
            if (simulatorState.selectedTimes.Contains(all[i]))
            {
                active.Add("Allowed");
            }
            else
            {
                active.Add("Not Allowed");
            }
        }
        int page = timeSelect.GetPage();
        timeSelect.SetStatsAndData(all, active);
        timeSelect.SetPage(page);
    }
}
