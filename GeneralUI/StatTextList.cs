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
    public List<StatTextText> statTexts;
    protected override void UpdateCurrentPage(List<int> newPageIndexes)
    {
        ResetPage();
        for (int i = 0; i < newPageIndexes.Count; i++)
        {
            objects[i].SetActive(true);
            statTexts[i].SetStatText(stats[i]);
            statTexts[i].SetText(data[i]);
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
