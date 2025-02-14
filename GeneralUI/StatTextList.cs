using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatTextList : GameObjectTextList
{
    public List<GameObject> changePageObjects;
    public void EnableChangePage()
    {
        for (int i = 0; i < changePageObjects.Count; i++)
        {
            changePageObjects[i].SetActive(true);
        }
    }
    public void DisableChangePage()
    {
        utility.DisableGameObjects(changePageObjects);
    }
    public List<string> stats;
    public TMP_Text title;
    public void SetTitle(string newTitle){title.text = newTitle;}
    public List<StatTextText> statTexts;

    public void SetStatsAndData(List<string> newStats, List<string> newData)
    {
        stats = newStats;
        data = newData;
        page = 0;
        if (stats.Count > statTexts.Count)
        {
            EnableChangePage();
        }
        UpdateCurrentPageStatTexts();
    }

    protected void UpdateCurrentPageStatTexts()
    {
        ResetPage();
        List<int> newPageIndices = new List<int>(utility.GetCurrentPageIndices(page, objects, data));
        for (int i = 0; i < newPageIndices.Count; i++)
        {
            objects[i].SetActive(true);
            statTexts[i].SetStatText(stats[newPageIndices[i]]);
            statTexts[i].SetText(data[newPageIndices[i]]);
        }
    }

    protected override void UpdateCurrentPage()
    {
        ResetPage();
        List<int> newPageIndices = new List<int>(utility.GetCurrentPageIndices(page, objects, data));
        for (int i = 0; i < newPageIndices.Count; i++)
        {
            objects[i].SetActive(true);
            statTexts[i].SetStatText(stats[newPageIndices[i]]);

        }
    }
    protected override void ResetPage()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            statTexts[i].Reset();
            objects[i].SetActive(false);
        }
    }
}
