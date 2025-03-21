using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectList : MonoBehaviour
{
    public GeneralUtility utility;
    public int textSize;
    [ContextMenu("UpdateTextSize")]
    public void UpdateTextSize()
    {
        utility.SetTextSizes(textList, textSize);
    }
    public GameObject errorMsgPanel;
    public void ErrorMessage(bool show = true){errorMsgPanel.SetActive(show);}
    public TMP_Text errorText;
    public List<string> errorMessages;
    public List<string> selectable;
    public int selectedIndex = -1;
    public int GetSelected(){return selectedIndex;}
    public string selected;
    public TMP_Text pageDisplay;
    public void UpdatePageDisplay()
    {
        if (pageDisplay == null){return;}
        pageDisplay.text = (currentPage + 1)+"/"+(MaxPages()+1);
    }
    public int MaxPages()
    {
        if (selectable.Count < textObjects.Count){return 0;}
        return ((selectable.Count - 1) / textObjects.Count);
    }
    public TMP_Text selectedText;
    public void ShowSelected(){selectedText.text = selected;}
    public void SetSelectables(List<string> newList)
    {
        selectable = new List<string>(newList);
        currentPage = 0;
        UpdateCurrentPage(utility.GetCurrentPageStrings(currentPage, textObjects, selectable));
        if (newList.Count > textObjects.Count)
        {
            utility.EnableGameObjects(changePageObjects);
        }
        else{utility.DisableGameObjects(changePageObjects);}
    }
    public void UpdateSelectables(List<string> newList)
    {
        selectable = new List<string>(newList);
        UpdateCurrentPage(utility.GetCurrentPageStrings(currentPage, textObjects, selectable));
    }
    public List<GameObject> changePageObjects;
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

    protected virtual void ResetPage()
    {
        for (int i = 0; i < textObjects.Count; i++)
        {
            textList[i].text = "";
            textObjects[i].SetActive(false);
        }
    }

    public void StartingPage()
    {
        currentPage = 0;
        UpdateCurrentPage(utility.GetCurrentPageStrings(currentPage, textObjects, selectable));
    }

    protected void UpdateCurrentPage(List<string> newPageStrings)
    {
        ResetPage();
        UpdatePageDisplay();
        for (int i = 0; i < newPageStrings.Count; i++)
        {
            textObjects[i].SetActive(true);
            textList[i].text = newPageStrings[i];
        }
    }

    public virtual void Select(int index)
    {
        selectedIndex = (currentPage*textObjects.Count) + index;
        selected = selectable[currentPage*textObjects.Count + index];
    }
}
