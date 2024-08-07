using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatSheetUI : MonoBehaviour
{
    public List<TMP_Text> characterStats;

    public void UpdateStats(List<string> newStats)
    {
        for (int i = 0; i < Mathf.Min(characterStats.Count, newStats.Count); i++)
        {
            characterStats[i].text = newStats[i];
        }
    }
}
