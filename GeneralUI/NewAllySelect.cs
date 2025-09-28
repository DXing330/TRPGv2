using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewAllySelect : MonoBehaviour
{
    public StSBattleRewards battleRewards;
    public StSEnemyTracker enemyTracker;
    public GameObject selectPanel;
    public SpriteContainer actorSprites;
    public StatDatabase actorStats;
    public ColorDictionary colors;
    public List<string> actorNames;
    public List<TMP_Text> actorNameTexts;
    public List<Image> actorImages;
    public int selected = -1;
    public void GetChoices(bool rare = false)
    {
        SetChoices(enemyTracker.ReturnCurrentChoices(rare));
    }
    public void SetChoices(List<string> newNames)
    {
        selected = -1;
        actorNames = new List<string>(newNames);
        for (int i = 0; i < Mathf.Min(actorNames.Count, actorNameTexts.Count); i++)
        {
            actorNameTexts[i].text = actorNames[i];
            actorImages[i].sprite = actorSprites.SpriteDictionary(actorNames[i]);
        }
        selectPanel.SetActive(true);
    }
    public void ResetHighlights()
    {
        for (int i = 0; i < actorNameTexts.Count; i++)
        {
            actorNameTexts[i].color = colors.defaultColor;
        }
    }
    public void Select(int index)
    {
        selected = index;
        ResetHighlights();
        actorNameTexts[selected].color = colors.GetColorByKey("Highlight");
    }
    public void ConfirmChoice()
    {
        if (selected < 0)
        {
            return;
        }
        battleRewards.AddAllyReward(actorNames[selected]);
        selectPanel.SetActive(false);
    }
}
