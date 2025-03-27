using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OverworldGenTester : MonoBehaviour
{
    void Start()
    {
        UpdateDisplay();
    }
    public OverworldGenerator overworldGenerator;
    public OverworldMap overworldMap;
    [ContextMenu("Test Map")]
    public void TestMap()
    {
        overworldMap.SetData();
        overworldMap.PublicUpdateMap();
    }
    public string overworldData;
    public List<string> toStitchOverworldPieces;
    public List<string> overworldTiles;
    public List<TMP_Text> zoneDisplays;
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
                string luxury = overworldGenerator.luxuryLayer[i];
                string city = overworldGenerator.cityLayer[i];
                if (city.Length >= 2)
                {
                    overworldTileDisplay.text += "X";
                }
                else if (luxury.Length >= 2)
                {
                    overworldTileDisplay.text += overworldGenerator.luxuryLayer[i][0];
                }
                else{overworldTileDisplay.text += " ";}
            }
            else{overworldTileDisplay.text += overworldTiles[i][0];}
            if ((i+1)%overworldGenerator.GetSize() == 0){overworldTileDisplay.text += "\n";}
        }
    }

    [ContextMenu("Test Stitching")]
    public void TestStitching()
    {
        int size = 99;
        int zoneCount = 9;
        int zoneSizeDivisor  = 3;
        List<string> terrainLayer = new List<string>();
        for (int i = 0; i < size*size; i++)
        {
            terrainLayer.Add("");
        }
        List<string> zones = new List<string>();
        // Generate the zones.
        for (int i = 0; i < zoneCount; i++)
        {
            // The middle zone has no city/luxury.
            if (i == zoneCount/2)
            {
                zones.Add(overworldGenerator.GenerateZone(size/zoneSizeDivisor, "", true));
                continue;
            }
            // Pick a random luxury for each zone.
            zones.Add(overworldGenerator.GenerateZone(size/zoneSizeDivisor, ""));
        }
        // Stitch them together.
        int extZoneRow = 0;
        int extZoneCol = 0;
        for (int i = 0; i < zones.Count; i++)
        {
            int intZoneRow = 0;
            int intZoneCol = 0;
            string[] zoneInfo = zones[i].Split("@");
            List<string> zoneTerrain = new List<string>(zoneInfo[0].Split("#").ToList());
            for (int j = 0; j < zoneTerrain.Count; j++)
            {
                int tileNumber = (((extZoneRow*(size/zoneSizeDivisor))+(intZoneRow))*(size))+((extZoneCol*(size/zoneSizeDivisor))+(intZoneCol));
                //Debug.Log(tileNumber);
                terrainLayer[tileNumber] = zoneTerrain[j];
                intZoneCol++;
                if (intZoneCol >= size/zoneSizeDivisor)
                {
                    intZoneCol = 0;
                    intZoneRow++;
                }
            }
            extZoneCol++;
            if (extZoneCol >= zoneSizeDivisor)
            {
                extZoneCol = 0;
                extZoneRow++;
            }
        }
        UpdateFullDisplay(terrainLayer, 99);
        UpdateZoneDisplays(zones, 33);
    }

    protected void UpdateFullDisplay(List<string> stitched, int size)
    {
        overworldTileDisplay.text = "";
        for (int i = 0; i < stitched.Count; i++)
        {
            if (stitched[i] == overworldGenerator.defaultTile)
            {
                overworldTileDisplay.text += " ";
            }
            else{overworldTileDisplay.text += stitched[i][0];}
            if ((i+1)%size == 0){overworldTileDisplay.text += "\n";}
        }
    }

    protected void UpdateZoneDisplays(List<string> zones, int size)
    {
        for (int i = 0; i < zoneDisplays.Count; i++)
        {
            zoneDisplays[i].text = "";
            List<string> zoneInfo = zones[i].Split("@")[0].Split("#").ToList();
            Debug.Log(zoneInfo.Count);
            for (int j = 0; j < zoneInfo.Count; j++)
            {
                if (zoneInfo[j] == overworldGenerator.defaultTile)
                {
                    zoneDisplays[i].text += " ";
                }
                else{zoneDisplays[i].text += zoneInfo[j][0];}
                if ((j+1)%size == 0){zoneDisplays[i].text += "\n";}
            }
        }
    }
}
