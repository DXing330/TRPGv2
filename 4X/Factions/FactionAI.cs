using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionAI : MonoBehaviour
{
    // Can build workers (cheapest), combat units (cheap), buildings (medium), expand (medium) or research (expensive).
    public FactionMap map;
    public FactionManager factionManager;
    public List<string> factionActions;

    protected void FactionsTurn(FactionData faction)
    {
        // Make workers.
        int workerCount = faction.unitData.WorkerUnitCount();
        if (workerCount < map.CountBuildingsOnTiles(faction.GetOwnedTiles()) - 1)
        {
            // Workers are free but cost food/gold for upkeep.
            factionManager.FactionMakesWorker(faction);
            return;
        }
        // Make buildings.
        // Get all owned tiles and filter for those without buildings.
        List<int> tilesWithoutBuildings = map.FilterTilesWithoutBuildings(faction.GetOwnedTiles());
        if (tilesWithoutBuildings.Count > 0)
        {
            // Add the first building, later can be updated to make the greedy choice.
            List<string> buildables = map.BuildablesOnTiles(tilesWithoutBuildings);
            for (int i = 0; i < buildables.Count; i++)
            {
                if (buildables[i] == ""){continue;}
                // Check the cost.
                if (faction.PayBuildingCost())
                {
                    map.TryToBuildOnTile(buildables[i], tilesWithoutBuildings[i]);
                    return;
                }
                else{break;}
            }
        }
        // Expand to the adjacent tile with largest output.
        List<int> borderTiles = map.mapUtility.AdjacentBorders(faction.GetOwnedTiles(), map.mapSize);
        int targetTile = map.ReturnTileWithLargestOutput(borderTiles);
        if (targetTile >= 0 && faction.PayExpandCost())
        {
            factionManager.TryToClaimTile(targetTile, faction);
            return;
        }
        int soldierCount = faction.unitData.combatUnitData.Count;
        if (soldierCount < workerCount / 2)
        {

        }
    }

    public void AllTurns(List<FactionData> factions)
    {
        for (int i = 0; i < factions.Count; i++)
        {
            FactionsTurn(factions[i]);
        }
    }
}
