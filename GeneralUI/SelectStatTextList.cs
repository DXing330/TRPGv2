using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectStatTextList : StatTextList
{
    public Equipment equipment;
    public int selectedIndex;
    public void ResetSelected(){selectedIndex = -1;}
    public void Select(int index)
    {
        selectedIndex = index + (page*objects.Count);
    }
    public int GetSelected(){return selectedIndex;}
    // This is more specialized but placed here for now.
    public void UpdateActorStatTexts(TacticActor actor)
    {
        DisableChangePage();
        page = 0;
        stats.Clear();
        data.Clear();
        stats.Add("Health");
        stats.Add("Attack");
        stats.Add("Defense");
        stats.Add("Move Speed");
        data.Add(actor.GetBaseHealth().ToString());
        data.Add(actor.GetBaseAttack().ToString());
        data.Add(actor.GetBaseDefense().ToString());
        data.Add(actor.GetMoveSpeed().ToString());
        for (int i = 0; i < 4; i++)
        {
            objects[i].SetActive(true);
            statTexts[i].SetStatText(stats[i]);
            statTexts[i].SetText(data[i]);
        }
    }

    public void UpdateActorPassiveTexts(TacticActor actor, string currentlyEquipped)
    {
        page = 0;
        stats.Clear();
        data.Clear();
        stats = new List<string>(actor.GetPassiveSkills());
        data = new List<string>(actor.GetPassiveLevels());
        for (int i = 0; i < stats.Count; i++)
        {
            objects[i].SetActive(true);
            statTexts[i].SetStatText(stats[i]);
            statTexts[i].SetText(data[i]);
        }
        if (stats.Count > objects.Count){EnableChangePage();}
    }

    public void UpdateActorEquipmentTexts(string selectedActorEquipment)
    {
        page = 0;
        data.Clear();
        // 3 types of equipment, weapon, armor, charm
        for (int i = 0; i < 3; i++)
        {
            statTexts[i].SetText("None");
        }
        string[] dataBlocks = selectedActorEquipment.Split("@");
        for (int i = 0; i < dataBlocks.Length; i++)
        {
            equipment.SetAllStats(dataBlocks[i]);
            switch (equipment.GetSlot())
            {
                case "Weapon":
                statTexts[0].SetText(equipment.GetName());
                break;
                case "Armor":
                statTexts[1].SetText(equipment.GetName());
                break;
                case "Charm":
                statTexts[2].SetText(equipment.GetName());
                break;
            }
        }
    }

    // raw data is full equip stats, just extract the names.
    public void UpdateEquipNames()
    {
        stats.Clear();
        for (int i = 0; i < data.Count; i++)
        {
            string[] splitData = data[i].Split("|");
            stats.Add(splitData[0]);
        }
        page = 0;
        UpdateCurrentPage();
    }
}
