using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectTextList : MonoBehaviour
{
    public int page = 0;
    public GeneralUtility utility;
    public List<GameObject> objects;
    public List<string> data;
    public void SetData(List<string> newData){data = new List<string>(newData);}
    [ContextMenu("Right")]
    public void ChangeRight(){ChangePage();}
    [ContextMenu("Left")]
    public void ChangeLeft(){ChangePage(false);}
    public void ChangePage(bool right = true)
    {
        page = utility.ChangePage(page, right, objects, data);
        UpdateCurrentPage(utility.GetCurrentPageIndices(page, objects, data));
    }
    protected virtual void UpdateCurrentPage(List<int> newPageIndexes)
    {
        ResetPage();
        for (int i = 0; i < newPageIndexes.Count; i++)
        {
            objects[i].SetActive(true);
        }
    }
    protected virtual void ResetPage()
    {
        utility.DisableGameObjects(objects);
    }
}
