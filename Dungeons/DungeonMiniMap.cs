using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DungeonMiniMap : MonoBehaviour
{
    public Dungeon dungeon;
    public bool active = false;
    public bool copy = false;
    public DungeonMiniMap copiedMap;
    public string miniMapText;
    public GameObject miniMapObject;
    public TMP_Text text;

    public void ToggleState()
    {
        active = !active;
        UpdateState();
    }

    protected void UpdateState()
    {
        if (active)
        {
            miniMapObject.SetActive(true);
            if (copy)
            {
                miniMapText = copiedMap.miniMapText;
            }
            UpdateMiniMap();
        }
        else{miniMapObject.SetActive(false);}
    }

    public void UpdateMiniMap()
    {
        text.text = miniMapText;
    }

    public void UpdateMiniMapString(List<int> currentTiles = null)
    {
        miniMapText = "";
        int mapCount = 0;
        for (int i = 0; i < dungeon.viewedTiles.Count; i++)
        {
            if (dungeon.viewedTiles[i] == 0){miniMapText += "?";}
            else
            {
                if (i == dungeon.partyLocation){miniMapText += "<color=blue>P</color>";}
                else if (dungeon.EnemyLocation(i) && currentTiles.Contains(i)){miniMapText += "<color=red>E</color>";}
                else if (dungeon.StairsDownLocation(i)){miniMapText += "<color=green>S</color>";}
                else if (dungeon.GoalTile(i)){miniMapText += "<color=green>Q</color>";}
                else if (dungeon.TreasureLocation(i)){miniMapText += "<color=green>T</color>";}
                else if (dungeon.ItemLocation(i)){miniMapText += "<color=green>I</color>";}
                else if (dungeon.TilePassable(i)){miniMapText += " ";}
                else{miniMapText += "X";}
            }
            mapCount++;
            if (mapCount >= dungeon.GetDungeonSize())
            {
                miniMapText += "\n";
                mapCount = 0;
            }
        }
    }
}
