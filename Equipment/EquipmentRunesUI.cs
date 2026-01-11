using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentRunesUI : MonoBehaviour
{
    public PassiveDetailViewer detailViewer;
    public List<EquipmentRunes> runeGrid;
    public string testRuneGridStrings;
    [ContextMenu("Test Rune Grid Display")]
    public void TestRuneGridDisplay()
    {
        string[] blocks = testRuneGridStrings.Split("#");
        int index = 0;
        for (int i = 0; i < runeGrid.Count; i++)
        {
            if (index >= blocks.Length){break;}
            runeGrid[i].runeNames.Clear();
            for (int j = 0; j < runeGrid[i].runeImages.Count; j++)
            {
                if (index >= blocks.Length){break;}
                runeGrid[i].runeNames.Add(blocks[index]);
                index++;
            }
        }
        UpdateGridImages();
    }
    [ContextMenu("Update Grid Images")]
    public void UpdateGridImages()
    {
        for (int i = 0; i < runeGrid.Count; i++)
        {
            runeGrid[i].UpdateRuneImages();
        }
    }

    // Load the current actor equipment.
    public void UpdateRuneGrid(string allEquipment)
    {
        string[] blocks = allEquipment.Split("@");
        for (int i = 0; i < runeGrid.Count; i++)
        {
            for (int j = 0; j < blocks.Length; j++)
            {
                runeGrid[i].SetEquipmentStats(blocks[j]);
            }
        }
    }

    public void ViewRune(string runeName)
    {
        if (runeName.Length <= 1){return;}
        detailViewer.ViewRunePassive(runeName);
    }
}
