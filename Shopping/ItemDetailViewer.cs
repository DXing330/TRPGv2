using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemDetailViewer : MonoBehaviour
{
    public TMP_Text itemName;
    public TMP_Text itemInfo;
    // Either view an equipment or an item.
    public bool viewingEquipment = true;
    public void ViewEquip(){viewingEquipment = true;}
    public void ViewItem(){viewingEquipment = false;}
    public StatDatabase equipData;
    public Equipment equipment;
    public StatDatabase itemData;
    public ActiveSkill active;
    public ActiveDescriptionViewer itemDescriptions;
    public SelectStatTextList passiveSelect;
    public PassiveDetailViewer passiveDetails;

    public void ResetView()
    {
        itemName.text = "";
        itemInfo.text = "";
    }

    public void ShowEquipmentInfo(string newInfo)
    {
        equipment.SetAllStats(newInfo);
        itemName.text = equipment.GetName();
        itemInfo.text = "Grants the user: ";
        passiveSelect.SetStatsAndData(equipment.GetPassives(), equipment.GetPassiveLevels());
    }

    public void ViewPassiveDetails()
    {
        int index = passiveSelect.GetSelected();
        if (index < 0) { return; }
        passiveDetails.UpdatePassiveNames(equipment.GetPassives()[index], equipment.GetPassiveLevels()[index]);
    }

    public void ShowInfo(string newItem)
    {
        string data = "";
        itemName.text = newItem;
        if (viewingEquipment)
        {
            data = equipData.ReturnValue(newItem);
            equipment.SetAllStats(data);
            itemInfo.text = "Grants the user: ";
            for (int i = 0; i < equipment.passives.Count; i++)
            {
                itemInfo.text += equipment.passives[i];
                if (i < equipment.passives.Count - 1)
                {
                    itemInfo.text += ", ";
                }
                else
                {
                    itemInfo.text += ".";
                }
            }
        }
        else
        {
            data = itemData.ReturnValue(newItem);
            active.LoadSkillFromString(data);
            itemInfo.text = itemDescriptions.ReturnActiveDescription(active);
        }
    }
}
