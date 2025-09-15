using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "Utility", menuName = "ScriptableObjects/Utility/GeneralUtility", order = 1)]
public class GeneralUtility : ScriptableObject
{
    public int ChangeIndex(int currentIndex, bool right, int maxIndex, int minIndex = 0)
    {
        if (right)
        {
            if (currentIndex >= maxIndex) { return minIndex; }
            else { return currentIndex + 1; }
        }
        else
        {
            if (currentIndex > minIndex) { return currentIndex - 1; }
            else { return maxIndex; }
        }
    }

    public string GetNextItemInList(List<string> allItems, string currentItem, bool increase = true)
    {
        if (allItems.Count <= 0){ return ""; }
        int indexOf = allItems.IndexOf(currentItem);
        if (indexOf < 0) { return allItems[0]; }
        if (increase)
        {
            indexOf = (indexOf + 1) % allItems.Count;
        }
        else
        {
            indexOf = (indexOf + allItems.Count - 1) % allItems.Count;
        }
        return allItems[indexOf];
    }

    public int ChangePage(int currentPage, bool right, List<GameObject> pageLength, List<string> dataList)
    {
        int maxPage = dataList.Count / pageLength.Count;
        if (dataList.Count % pageLength.Count == 0) { maxPage--; }
        if (right)
        {
            if (currentPage < maxPage) { currentPage++; }
            else { currentPage = 0; }
        }
        else
        {
            if (currentPage > 0) { currentPage--; }
            else { currentPage = maxPage; }
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

    public List<int> GetCurrentPageIndices(int currentPage, List<GameObject> pageLength, List<string> dataList)
    {
        List<int> indices = new List<int>();
        int start = currentPage * pageLength.Count;
        for (int i = start; i < Mathf.Min(start + pageLength.Count, dataList.Count); i++)
        {
            indices.Add(i);
        }
        return indices;
    }

    public void DisableGameObjects(List<GameObject> objects)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].SetActive(false);
        }
    }

    public void EnableGameObjects(List<GameObject> objects)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].SetActive(true);
        }
    }

    public void SetTextSizes(List<TMP_Text> texts, int size)
    {
        for (int i = 0; i < texts.Count; i++)
        {
            SetTextSize(texts[i], size);
        }
    }

    public void SetTextSize(TMP_Text text, int size)
    {
        text.fontSize = size;
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

    public List<string> QuickSortIntStringList(List<string> intStrings, int left, int right)
    {
        int i = left;
        int j = right;
        int pivot = int.Parse(intStrings[left]);
        while (i <= j)
        {
            while (int.Parse(intStrings[i]) > pivot)
            {
                i++;
            }
            while (int.Parse(intStrings[j]) < pivot)
            {
                j--;
            }
            if (i <= j)
            {
                string temp = (intStrings[i]);
                intStrings[i] = intStrings[j];
                intStrings[j] = temp;
                /*TacticActor tempActor = actors[i];
                actors[i] = actors[j];
                actors[j] = tempActor;*/
                i++;
                j--;
            }
        }
        if (left < j)
        {
            QuickSortIntStringList(intStrings, left, j);
        }
        if (i < right)
        {
            QuickSortIntStringList(intStrings, i, right);
        }
        return intStrings;
    }

    public List<string> QuickSortByIntStringList(List<string> toSort, List<string> intStrings, int left, int right)
    {
        int i = left;
        int j = right;
        int pivot = int.Parse(intStrings[left]);
        while (i <= j)
        {
            while (int.Parse(intStrings[i]) > pivot)
            {
                i++;
            }
            while (int.Parse(intStrings[j]) < pivot)
            {
                j--;
            }
            if (i <= j)
            {
                string temp = (intStrings[i]);
                intStrings[i] = intStrings[j];
                intStrings[j] = temp;
                temp = toSort[i];
                toSort[i] = toSort[j];
                toSort[j] = temp;
                i++;
                j--;
            }
        }
        if (left < j)
        {
            QuickSortByIntStringList(toSort, intStrings, left, j);
        }
        if (i < right)
        {
            QuickSortByIntStringList(toSort, intStrings, i, right);
        }
        return toSort;
    }

    public string ConvertIntListToString(List<int> int_list, string delimiter = "|")
    {
        List<string> string_list = new List<string>();
        for (int i = 0; i < int_list.Count; i++)
        {
            string_list.Add(int_list[i].ToString());
        }
        return ConvertListToString(string_list, delimiter);
    }

    public List<int> ConvertStringListToIntList(List<string> string_list)
    {
        List<int> int_list = new List<int>();
        for (int i = 0; i < string_list.Count; i++)
        {
            int_list.Add(SafeParseInt(string_list[i]));
        }
        return int_list;
    }

    public string ConvertListToString(List<string> string_list, string delimiter = "|")
    {
        return String.Join(delimiter, string_list);
    }

    public string ConvertArrayToString(string[] string_array, string delimiter = "|")
    {
        List<string> string_list = string_array.ToList();
        return ConvertListToString(string_list, delimiter);
    }

    public int CountStringsInList(List<string> list, string specifics)
    {
        int count = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == specifics) { count++; }
        }
        return count;
    }

    public int CountCharactersInString(string dummyString, char specifics = ' ')
    {
        int count = 0;
        for (int i = 0; i < dummyString.Length; i++)
        {
            if (dummyString[i] == specifics)
            {
                count++;
            }
        }
        return count;
    }

    public int SafeParseInt(string intString, int defaultValue = 0)
    {
        try
        {
            return int.Parse(intString);
        }
        catch
        {
            return defaultValue;
        }
    }

    public int SumDescending(int lastValue)
    {
        return (lastValue) * (lastValue + 1) / 2;
    }
}
