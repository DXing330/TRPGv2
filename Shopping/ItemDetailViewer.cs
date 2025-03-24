using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemDetailViewer : MonoBehaviour
{
    public TMP_Text itemName;
    public TMP_Text itemInfo;
    // Either view an equipment or an item.
    public bool equipment = true;
    public void ViewEquip(){equipment = true;}
    public void ViewItem(){equipment = false;}
    public StatDatabase equipData;
    public StatDatabase itemData;
    public ActiveSkill active;
    public ActiveDescriptionViewer itemDescriptions;

    public void ResetView()
    {
        itemName.text = "";
        itemInfo.text = "";
    }
    public void ShowInfo(string newItem)
    {
        string data = "";
        itemName.text = newItem;
        if (equipment)
        {
            data = equipData.ReturnValue(newItem);
        }
        else
        {
            data = itemData.ReturnValue(newItem);
            active.LoadSkillFromString(data);
            itemInfo.text = itemDescriptions.ReturnActiveDescription(active);
        }
    }
}
