using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This will coordinate information between each faction and the map.
// Any function that requires knowledge of both a faction and the map will go through here.
public class FactionManager : MonoBehaviour
{
    public FactionMap map;
    public List<FactionData> factions;
    public List<string> possibleFactionColors;
    public List<string> possibleFactionNames;
    public List<string> possibleFactionLeaders;
    public List<string> possibleFactionStartingTiles;
    public List<string> possibleFactionPossibleUnits;
    public List<string> possibleFactionBattleModifiers;

    // Cities override all other buildings.
    public void UpdateCityInfo()
    {
        for (int i = 0; i < factions.Count; i++)
        {
            map.tileBuildings[factions[i].GetCapitalLocation()] = "City";
            for (int j = 0; j < factions[i].cityLocations.Count; j++)
            {
                map.tileBuildings[factions[i].cityLocations[j]] = "City";
            }
        }
    }

    public bool TileAlreadyOwned(int tileNumber)
    {
        for (int i = 0; i < factions.Count; i++)
        {
            if (factions[i].TileOwned(tileNumber))
            {
                return true;
            }
        }
        return false;
    }

    protected int GenerateCapitalLocation(int index)
    {
        int potentialCapital = map.ReturnRandomTileOfTileTypes(possibleFactionStartingTiles[index].Split("|").ToList());
        for (int i = 0; i < factions.Count; i++)
        {
            // Make sure no two capitals are next to each other.
            if (map.mapUtility.DistanceBetweenTiles(potentialCapital, factions[i].GetCapitalLocation(), map.mapSize) < 2)
            {
                return GenerateCapitalLocation(index);
            }
            // Make sure no capitals are on the border.
            else if (map.mapUtility.BorderTile(potentialCapital, map.mapSize))
            {
                return GenerateCapitalLocation(index);
            }
        }
        return potentialCapital;
    }

    [ContextMenu("Generate Factions")]
    public void GenerateFactions()
    {
        List<string> copiedColors = new List<string>(possibleFactionColors);
        List<string> copiedNames = new List<string>(possibleFactionNames);
        for (int i = 0; i < factions.Count; i++)
        {
            factions[i].NewGame();
        }
        for (int i = 0; i < factions.Count; i++)
        {
            // Set name.
            factions[i].SetFactionName(possibleFactionNames[i]);
            // Set color.
            factions[i].SetFactionColor(possibleFactionColors[i]);
            // Set leader.
            factions[i].SetFactionLeader(possibleFactionLeaders[i]);
            // Set capitals.
            factions[i].SetCapitalLocation(GenerateCapitalLocation(i));
            // Add adjacent tiles to owned tiles.
            List<int> adjacentTiles = map.mapUtility.AdjacentTiles(factions[i].GetCapitalLocation(), map.mapSize);
            for (int j = 0; j < adjacentTiles.Count; j++)
            {
                if (!TileAlreadyOwned(adjacentTiles[j]))
                {
                    factions[i].GainTile(adjacentTiles[j]);
                }
            }
            // Initialize other factions and relations.
            factions[i].Save();
        }
        UpdateCityInfo();
    }
}
