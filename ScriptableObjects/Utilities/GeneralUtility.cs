using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Utility", menuName = "ScriptableObjects/Utility/GeneralUtility", order = 1)]
public class GeneralUtility : ScriptableObject
{
    public int ChangePage(int currentPage, bool right, List<GameObject> pageLength, List<string> dataList)
    {
        int maxPage = dataList.Count/pageLength.Count;
        if (dataList.Count%pageLength.Count == 0){maxPage--;}
        if (right)
        {
            if (currentPage < maxPage){currentPage++;}
            else{currentPage = 0;}
        }
        else
        {
            if (currentPage > 0){currentPage--;}
            else{currentPage = maxPage;}
        }
        return currentPage;
    }

    public List<string> GetCurrentPageStrings(int currentPage, List<GameObject> pageLength, List<string> dataList)
    {
        List<string> strings = new List<string>();
        int start = currentPage * pageLength.Count;
        for (int i = start; i < Mathf.Min(start + pageLength.Count, dataList.Count); i++)
        {
            strings.Add(dataList[i]);
        }
        return strings;
    }

    public void DisableGameObjects(List<GameObject> objects)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].SetActive(false);
        }
    }

    public List<string> RemoveEmptyListItems(List<string> stringList, int minLength = 0)
    {
        for (int i = stringList.Count - 1; i >= 0; i--)
        {
            if (stringList[i].Length <= minLength)
            {
                stringList.RemoveAt(i);
            }
        }
        return stringList;
    }
}
