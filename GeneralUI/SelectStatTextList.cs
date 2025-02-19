using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectStatTextList : StatTextList
{
    public Equipment equipment;
    public ColorDictionary colors;
    protected override void ResetPage()
    {
        base.ResetPage();
        ResetHighlights();
    }

    public void ResetHighlights()
    {
        for (int i = 0; i < statTexts.Count; i++)
        {
            statTexts[i].SetColor(colors.GetColor("Default"));
        }
    }
    public void HighlightIndex(int index, string color = "Highlight")
    {
        statTexts[index].SetColor(colors.GetColor(color));
    }
    public void HighlightSelected(string color = "Highlight")
    {
        ResetHighlights();
        if (GetSelected() < 0){return;}
        statTexts[GetSelected()].SetColor(colors.GetColor(color));
    }
    public int selectedIndex;
    public void ResetSelected()
    {
        selectedIndex = -1;
        ResetHighlights();
    }
    public void Select(int index)
    {
        selectedIndex = index + (page*objects.Count);
        HighlightSelected();
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
        stats.Add("Energy");
        stats.Add("Move Speed");
        data.Add(actor.GetBaseHealth().ToString());
        data.Add(actor.GetBaseAttack().ToString());
        data.Add(actor.GetBaseDefense().ToString());
        data.Add(actor.GetBaseEnergy().ToString());
        data.Add(actor.GetMoveSpeed().ToString());
        for (int i = 0; i < 5; i++)
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
        ResetPage();
        string[] allEquipped = currentlyEquipped.Split("@");
        for (int i = 0; i < allEquipped.Length; i++)
        {
            equipment.SetAllStats(allEquipped[i]);
            equipment.EquipToActor(actor);
        }
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

    public void UpdatePotentialPassives(TacticActor actor, string currentEquipment, string newEquipment)
    {
        // Keep track of the base actor passive stats.
        List<string> basePassives = new List<string>(actor.GetPassiveSkills());
        List<string> baseLevels = new List<string>(actor.GetPassiveLevels());
        // Equipped the current equipment.
        string[] allEquipped = currentEquipment.Split("@");
        for (int i = 0; i < allEquipped.Length; i++)
        {
            equipment.SetAllStats(allEquipped[i]);
            equipment.EquipToActor(actor);
        }
        // Keep track of current stats.
        List<string> currentPassives = new List<string>(actor.GetPassiveSkills());
        List<string> currentLevels = new List<string>(actor.GetPassiveLevels());
        // Reset to base stats.
        actor.SetPassiveSkills(basePassives);
        actor.SetPassiveLevels(baseLevels);
        equipment.SetAllStats(newEquipment);
        equipment.EquipToActor(actor);
        string slot = equipment.GetSlot();
        // Replace the equipment in the specified slot with the new equipment.
        for (int i = 0; i < allEquipped.Length; i++)
        {
            equipment.SetAllStats(allEquipped[i]);
            if (slot == equipment.GetSlot())
            {
                continue;
            }
            equipment.EquipToActor(actor);
        }
        List<string> potentialPassives = new List<string>(actor.GetPassiveSkills());
        List<string> potentialLevels = new List<string>(actor.GetPassiveLevels());
        // Generate a list will all skills.
        List<string> allPassives = new List<string>(potentialPassives);
        allPassives.AddRange(currentPassives.Except(potentialPassives));
        List<string> allPassiveLevels = new List<string>();
        string passiveName = "";
        int indexOf = -1;
        for (int i = 0; i < allPassives.Count; i++)
        {
            passiveName = allPassives[i];
            indexOf = potentialPassives.IndexOf(passiveName);
            if (indexOf < 0)
            {
                allPassiveLevels.Add("0");
                continue;
            }
            allPassiveLevels.Add(potentialLevels[indexOf]);
        }
        // Sort the list by passive levels.
        allPassives = utility.QuickSortByIntStringList(allPassives, allPassiveLevels, 0, allPassives.Count - 1);
        //allPassiveLevels = utility.QuickSortIntStringList(allPassiveLevels, 0, allPassiveLevels.Count - 1);
        // Compare the levels to the previous levels.
        List<string> comparisons = new List<string>();
        int potentialLevel = 0;
        int currentLevel = 0;
        for (int i = 0; i < allPassives.Count; i++)
        {
            passiveName = allPassives[i];
            indexOf = currentPassives.IndexOf(passiveName);
            if ((indexOf < 0))
            {
                comparisons.Add("Increase");
                continue;
            }
            potentialLevel = int.Parse(allPassiveLevels[i]);
            currentLevel = int.Parse(currentLevels[indexOf]);
            if (potentialLevel == currentLevel)
            {
                comparisons.Add("Default");
            }
            else if (potentialLevel > currentLevel)
            {
                comparisons.Add("Increase");
            }
            else
            {
                comparisons.Add("Decrease");
            }
        }
        // Display the levels.
        ResetPage();
        for (int i = 0; i < allPassives.Count; i++)
        {            
            objects[i].SetActive(true);
            statTexts[i].SetStatText(allPassives[i]);
            statTexts[i].SetText(allPassiveLevels[i]);
            statTexts[i].SetColor(colors.GetColor(comparisons[i]));
        }
    }
}
