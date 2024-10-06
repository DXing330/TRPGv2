using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectList : MonoBehaviour
{
    public GeneralUtility utility;
    public List<string> selectable;
    public void SetSelectables(List<string> newList)
    {
        selectable = new List<string>(newList);
        currentPage = 0;
        UpdateCurrentPage(utility.GetCurrentPageStrings(currentPage, textObjects, selectable));
    }
    public List<GameObject> textObjects;
    public List<TMP_Text> textList;
    public int currentPage;
    [ContextMenu("Right")]
    public void ChangeRight(){ChangePage();}
    [ContextMenu("Left")]
    public void ChangeLeft(){ChangePage(false);}
    public void ChangePage(bool right = true)
    {
        currentPage = utility.ChangePage(currentPage, right, textObjects, selectable);
        UpdateCurrentPage(utility.GetCurrentPageStrings(currentPage, textObjects, selectable));
    }

    protected void ResetPage()
    {
        for (int i = 0; i < textObjects.Count; i++)
        {
            textList[i].text = "";
            textObjects[i].SetActive(false);
        }
    }

    protected void UpdateCurrentPage(List<string> newPageStrings)
    {
        ResetPage();
        for (int i = 0; i < newPageStrings.Count; i++)
        {
            textObjects[i].SetActive(true);
            textList[i].text = newPageStrings[i];
        }
    }

    public virtual void Select(int index)
    {

    }
}
