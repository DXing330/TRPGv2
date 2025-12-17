using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityManager : MonoBehaviour
{
    public FactionMap map;
    public int minimumCityDist = 6;
    public FactionCityData allCities;
    public FactionCity dummyCity;
    public List<string> possibleFactionColors;
    public List<string> possibleFactionNames;
    public List<string> possibleFactionStartingTiles;
    protected int GenerateCapitalLocation(int index)
    {
        // This naturally excludes luxury tiles.
        int potentialCapital = map.RandomTileOfType(possibleFactionStartingTiles[index]);
        if (map.ClosestCityDistance(potentialCapital) < minimumCityDist)
        {
            return GenerateCapitalLocation(index);
        }
        if (map.mapUtility.BorderTile(potentialCapital, map.mapSize))
        {
            return GenerateCapitalLocation(index);
        }
        return potentialCapital;
    }
    public void GenerateStartingCities()
    {
        allCities.NewGame();
        for (int i = 0; i < possibleFactionColors.Count; i++)
        {
            dummyCity.ResetStats();
            dummyCity.SetFaction(possibleFactionNames[i]);
            dummyCity.SetColor(possibleFactionColors[i]);
            int capital = GenerateCapitalLocation(i);
            dummyCity.SetLocation(capital);
            dummyCity.AddTile(capital);
            map.MakeCity(capital);
            map.UpdateHighlightedTile(capital, possibleFactionColors[i]);
            List<int> adjacentTiles = map.mapUtility.AdjacentTiles(capital, map.mapSize);
            for (int j = 0; j < adjacentTiles.Count; j++)
            {
                dummyCity.AddTile(adjacentTiles[j]);
                map.UpdateHighlightedTile(adjacentTiles[j], possibleFactionColors[i]);
            }
            allCities.AddCity(dummyCity.GetStats(), capital);
        }
        allCities.Save();
    }
}
