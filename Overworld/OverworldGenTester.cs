using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OverworldGenTester : MonoBehaviour
{
    public OverworldGenerator overworldGenerator;
    public string overworldData;
    public List<string> overworldTiles;
    public TMP_Text overworldTileDisplay;
    public string testBiomeShape;
    public string testBiomeType;

    [ContextMenu("Test Biome Shape")]
    public void GenerateBiomeShape()
    {
        overworldGenerator.GenerateOverworld();
        overworldGenerator.TestGenerate(testBiomeShape);
        UpdateDisplay();
    }

    [ContextMenu("Test Biome Type")]
    public void GenerateBiomeType()
    {
        overworldGenerator.GenerateOverworld();
        overworldGenerator.GenerateBiome(testBiomeType);
        UpdateDisplay();
    }

    [ContextMenu("TestGenerate")]
    public void TestGenerate()
    {
        overworldGenerator.GenerateOverworld();
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        overworldData = overworldGenerator.ReturnOverworld();
        overworldTiles = overworldData.Split("@")[0].Split("#").ToList();
        overworldTileDisplay.text = "";
        for (int i = 0; i < overworldTiles.Count; i++)
        {
            if (overworldTiles[i] == overworldGenerator.defaultTile)
            {
                overworldTileDisplay.text += " ";
            }
            else{overworldTileDisplay.text += overworldTiles[i][0];}
            if ((i+1)%overworldGenerator.GetSize() == 0){overworldTileDisplay.text += "\n";}
        }
    }
}
