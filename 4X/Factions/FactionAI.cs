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
        int workerCount = faction.unitData.UnitCount();
        if (workerCount < faction.GetOwnedTiles().Count / 3)
        {
            // Workers are free but cost food for upkeep.
            factionManager.unitManager.FactionMakesWorker(faction);
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
                map.TryToBuildOnTile(buildables[i], tilesWithoutBuildings[i]);
                return;
            }
        }
        // Expand to the adjacent tile with largest output.
        List<int> borderTiles = map.mapUtility.AdjacentBorders(faction.GetOwnedTiles(), map.mapSize);
        int targetTile = map.ReturnTileWithLargestOutput(borderTiles);
        if (targetTile >= 0)
        {
            // Pay cost.
            // Great place to add grudges/intrigue.
            // TODO: IMPLEMENT FACTION RELATIONSHIPS IN FACTION MANAGER.
            factionManager.TryToClaimTile(targetTile, faction);
            return;
        }
        int soldierCount = faction.unitData.combatUnitData.Count;
        if (soldierCount < workerCount / 2)
        {

        }
    }

    protected void FactionUpkeepCost(FactionData faction)
    {

    }

    public void AllTurns(List<FactionData> factions)
    {
        for (int i = 0; i < factions.Count; i++)
        {
            FactionsTurn(factions[i]);
        }
    }
}
