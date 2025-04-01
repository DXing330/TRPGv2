using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OverworldGen", menuName = "ScriptableObjects/Overworld/OverworldGen", order = 1)]
public class OverworldGenerator : ScriptableObject
{
    public GeneralUtility utility;
    public MapUtility mapUtility;
    public string defaultTile;
    public List<string> biomeTypes;
    public List<string> allowedShapes;
    public int biomeMinSize;
    public int biomeMaxSize;
    public int biomeCount;
    public int GetBiomeCount(){return biomeCount;}
    public List<string> possibleLuxuries;
    public int luxuryCount;
    public int size;
    public int GetSize(){return size;}
    public List<string> allTiles;
    public int RandomEmptyTile()
    {
        int tile = Random.Range(0, allTiles.Count);
        if (allTiles[tile] == defaultTile){return tile;}
        return RandomEmptyTile();
    }
    public string cityString = "City";
    public List<string> cityLayer;
    public List<string> luxuryLayer;
    public List<string> allBiomes;
    public List<string> allBiomeTiles;
    public void ResetTiles()
    {
        allTiles = new List<string>();
        allBiomes = new List<string>();
        allBiomeTiles = new List<string>();
        cityLayer = new List<string>();
        luxuryLayer = new List<string>();
        //allCities = new List<string>();
        //allCityTiles = new List<string>();
        //allLuxuries = new List<string>();
        //allLuxuryTiles = new List<string>();
    }

    public string ReturnOverworld()
    {
        string overworld = "";
        overworld += utility.ConvertListToString(allTiles, "#")+"@";
        overworld += utility.ConvertListToString(cityLayer, "#")+"@";
        overworld += utility.ConvertListToString(luxuryLayer, "#")+"@";
        overworld += utility.ConvertListToString(allBiomes, "#")+"@";
        overworld += utility.ConvertListToString(allBiomeTiles, "#")+"@";
        return overworld;
    }

    public void GenerateOverworld()
    {
        // default is all tiles are plains.
        ResetTiles();
        for (int i = 0; i < GetSize()*GetSize(); i++)
        {
            allTiles.Add(defaultTile);
            cityLayer.Add("");
            luxuryLayer.Add("");
        }
        for (int i = 0; i < biomeCount; i++)
        {
            GenerateRandomBiome();
        }
        for (int i = 0; i < luxuryCount; i++)
        {
            luxuryLayer[RandomEmptyTile()] = possibleLuxuries[Random.Range(0, possibleLuxuries.Count)];
            //allLuxuries.Add(possibleLuxuries[Random.Range(0, possibleLuxuries.Count)]);
            //allLuxuryTiles.Add(RandomEmptyTile().ToString());
        }
        cityLayer[RandomEmptyTile()] = cityString;
        //allCityTiles.Add(RandomEmptyTile().ToString());
    }

    public string GenerateZone(int zoneSize, string luxury, bool empty = false)
    {
        ResetTiles();
        for (int i = 0; i < zoneSize*zoneSize; i++)
        {
            allTiles.Add(defaultTile);
            cityLayer.Add("");
            luxuryLayer.Add("");
        }
        for (int i = 0; i < biomeCount; i++)
        {
            GenerateRandomBiome();
        }
        cityLayer[RandomEmptyTile()] = cityString;
        if (empty){return ReturnOverworld();}
        for (int i = 0; i < luxuryCount; i++)
        {
            luxuryLayer[RandomEmptyTile()] = possibleLuxuries[Random.Range(0, possibleLuxuries.Count)];
        }
        return ReturnOverworld();
    }

    public void TestGenerate(string biomeShape)
    {
        string biomeType = biomeTypes[Random.Range(0, biomeTypes.Count)];
        List<int> tiles = GetBiomeTiles(biomeShape);
        for (int i = 0; i < tiles.Count; i++)
        {
            allTiles[tiles[i]] = biomeType;
        }
    }

    public void GenerateRandomBiome()
    {
        // Should we add weights later?
        string biomeType = biomeTypes[Random.Range(0, biomeTypes.Count)];
        GenerateBiome(biomeType);
    }

    public void GenerateBiome(string biomeType)
    {
        // Should be based on biome type, different types have different allowable shapes.
        int indexOf = biomeTypes.IndexOf(biomeType);
        string[] allowed = allowedShapes[indexOf].Split("|");
        string biomeShape = allowed[Random.Range(0, allowed.Length)];
        //string biomeShape = biomeShapes[Random.Range(0, biomeShapes.Count)];
        allBiomes.Add(biomeType);
        List<int> tiles = GetBiomeTiles(biomeShape);
        allBiomeTiles.Add(utility.ConvertIntListToString(tiles, "|"));
        for (int i = 0; i < tiles.Count; i++)
        {
            allTiles[tiles[i]] = biomeType;
        }
    }

    protected List<int> GetBiomeTiles(string biomeShape)
    {
        List<int> biomeTiles = new List<int>();
        int startingTile = Random.Range(0, allTiles.Count);
        biomeTiles.Add(startingTile);
        int count = biomeTiles.Count;
        int direction = Random.Range(0, 6);
        int biomeSize = Random.Range(biomeMinSize, biomeMaxSize);
        int biomeSize2 = -1;
        switch (biomeShape)
        {
            // Requires length and width.
            case "Rectangle":
            // First add a line of tiles in the set direction.
            biomeTiles.AddRange(mapUtility.GetTilesInLineDirection(startingTile, direction, biomeSize, GetSize()));
            biomeSize2 = Random.Range(biomeMinSize, biomeMaxSize);
            count = biomeTiles.Count;
            // For each tile in the line, add tiles in the next direction.
            for (int i = 0; i < count; i++)
            {
                biomeTiles.AddRange(mapUtility.GetTilesInLineDirection(biomeTiles[i], (direction+1)%6, biomeSize2, GetSize()));
            }
            break;
            case "Square":
            biomeTiles.AddRange(mapUtility.GetTilesInLineDirection(startingTile, direction, biomeSize, GetSize()));
            count = biomeTiles.Count;
            for (int i = 0; i < count; i++)
            {
                biomeTiles.AddRange(mapUtility.GetTilesInLineDirection(biomeTiles[i], (direction+1)%6, biomeSize, GetSize()));
            }
            break;
            case "Circle":
            biomeTiles.AddRange(mapUtility.GetTilesInCircleShape(startingTile, biomeSize, GetSize()));
            break;
            case "Cone":
            int startTile = mapUtility.PointInDirection(startingTile, direction, GetSize());
            if (startTile == startingTile){break;}
            biomeTiles.AddRange(mapUtility.GetTilesInConeShape(startTile, biomeSize, startingTile, GetSize()));
            break;
            case "Ring":
            biomeTiles.AddRange(mapUtility.GetTileInRingShape(startingTile, biomeSize, GetSize()));
            biomeTiles.Remove(startingTile);
            break;
            case "River":
            break;
            case "Line":
            biomeTiles.AddRange(mapUtility.GetTilesInLineDirection(startingTile, direction, biomeSize, GetSize()));
            break;
        }
        return biomeTiles;
    }
}
