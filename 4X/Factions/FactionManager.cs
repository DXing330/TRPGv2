using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This will coordinate information between each faction and the map.
// Any function that requires knowledge of both a faction and the map will go through here.
public class FactionManager : MonoBehaviour
{
    public int factionCount;
    public FactionMap map;
    public FactionUnitManager unitManager;
    public FactionAI factionAI;
    public List<FactionData> factions;
    public int GetCapitalLocationOfFaction(string fName)
    {
        for (int i = 0; i < factions.Count; i++)
        {
            if (factions[i].GetFactionName() == fName)
            {
                return factions[i].GetCapitalLocation();
            }
        }
        return -1;
    }
    public List<int> GetOwnedTilesOfFaction(string fName)
    {
        for (int i = 0; i < factions.Count; i++)
        {
            if (factions[i].GetFactionName() == fName)
            {
                return factions[i].GetOwnedTiles();
            }
        }
        return new List<int>();
    }
    public void UnitDepositsInventory(FactionUnit unit, int location)
    {
        for (int i = 0; i < factions.Count; i++)
        {
            if (factions[i].GetCapitalLocation() == location && factions[i].GetFactionName() == unit.GetFaction())
            {
                factions[i].UnitDepositsInventory(unit);
            }
        }
    }
    public List<string> possibleFactionColors;
    public List<string> possibleFactionNames;
    public List<string> possibleFactionLeaders;
    public List<string> possibleFactionStartingTiles;
    //public List<string> possibleFactionPossibleUnits; // All factions have the same set of possible soldiers.
    public List<string> factionStartingPassives;
    public List<string> factionStartingPassiveLevels;

    // Cities override all other buildings.
    public void UpdateCityInfo()
    {
        for (int i = 0; i < factions.Count; i++)
        {
            map.MakeCapital(factions[i].GetCapitalLocation());
        }
    }
    
    // Show the units.
    public void UpdateUnitInfo()
    {
        for (int i = 0; i < factions.Count; i++)
        {
            // Show the workers.
            for (int j = 0; j < factions[i].unitData.unitData.Count; j++)
            {
                map.UpdateWorkerTile(factions[i].unitData.ReturnUnitLocationAtIndex(j), factions[i].unitData.GetUnitSpriteName());
                map.UpdateActorTile(factions[i].unitData.ReturnUnitLocationAtIndex(j), factions[i].unitData.GetUnitSpriteName());
            }
            // Override them with combat units.
            for (int j = 0; j < factions[i].unitData.combatUnitData.Count; j++)
            {
                map.UpdateSoldierTile(factions[i].unitData.ReturnCombatUnitLocationAtIndex(j), factions[i].unitData.GetCombatSpriteName());
                map.UpdateActorTile(factions[i].unitData.ReturnCombatUnitLocationAtIndex(j), factions[i].unitData.GetCombatSpriteName());
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

    public bool TryToClaimTile(int tileNumber, FactionData faction)
    {
        for (int i = 0; i < factions.Count; i++)
        {
            if (factions[i] == faction){continue;}
            if (factions[i].TileOwned(tileNumber))
            {
                factions[i].UpdateRelation(faction, -1);
                factions[i].LoseTile(tileNumber);
                return false;
            }
        }
        faction.GainTile(tileNumber);
        return true;
    }

    protected int GenerateCapitalLocation(int index)
    {
        // This naturally excludes luxury tiles.
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
            factions[i].GainTile(factions[i].GetCapitalLocation());
            // Add adjacent tiles to owned tiles.
            List<int> adjacentTiles = map.mapUtility.AdjacentTiles(factions[i].GetCapitalLocation(), map.mapSize);
            for (int j = 0; j < adjacentTiles.Count; j++)
            {
                if (!TileAlreadyOwned(adjacentTiles[j]))
                {
                    factions[i].GainTile(adjacentTiles[j]);
                }
            }
            factions[i].AddPassive(factionStartingPassives[i], factionStartingPassiveLevels[i]);
            // Make a starting worker.
            unitManager.FactionMakesWorker(factions[i]);
        }
        // Initialize other factions and relations.
        for (int i = 0; i < factions.Count; i++)
        {
            for (int j = 0; j < factions.Count; j++)
            {
                if (j == i){continue;}
                factions[i].otherFactions.Add(factions[j].GetFactionName());
                factions[i].otherFactionRelations.Add(0);
            }
            factions[i].Save();
        }
        UpdateCityInfo();
    }

    public void Save()
    {
        for (int i = 0; i < factions.Count; i++)
        {
            factions[i].Save();
        }
    }

    public void Load()
    {
        for (int i = 0; i < factions.Count; i++)
        {
            factions[i].Load();
        }
        UpdateCityInfo();
    }
}
