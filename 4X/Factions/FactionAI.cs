using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionAI : MonoBehaviour
{
    // Can build workers (cheapest), combat units (cheap), buildings (medium), expand (medium) or research (expensive).
    public FactionMap map;
    public FactionManager factionManager;
    public FactionUnitManager unitManager;
    public List<string> factionActions;

    protected void FactionsTurn(FactionData faction)
    {
        // Make workers.
        int workerCount = faction.unitData.WorkerUnitCount();
        if (workerCount < map.CountBuildingsOnTiles(faction.GetOwnedTiles()) - 1)
        {
            // Workers are free but cost food/gold for upkeep.
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

    protected void FactionUpkeepCost(FactionData faction)
    {
        faction.CollectTaxes();
        // Feed your units.
        int hunger = faction.FeedUnits();
        if (hunger > 0)
        {
            unitManager.ChangeUnitMorale(faction.unitData, -1);
        }
        // Pay your units.
        int debt = faction.PayUnits();
        if (debt > 0)
        {
            unitManager.ChangeUnitMorale(faction.unitData, -1);
        }
        // If paid and full then they're happy.
        if (debt <= 0 && hunger <= 0)
        {
            unitManager.ChangeUnitMorale(faction.unitData, 1);
        }
        // Maintain your city/buildings.
        if (!faction.MaintainBuildings(map.CountBuildingsOnTiles(faction.GetOwnedTiles())))
        {
            // Lose a random building.
            map.DestroyRandomBuilding(faction.GetOwnedTiles());
        }
        // Check if any units desert due to low morale or have died.
        unitManager.UnitDeathsAndDesertions(faction.unitData);
    }

    public void AllTurns(List<FactionData> factions)
    {
        for (int i = 0; i < factions.Count; i++)
        {
            FactionUpkeepCost(factions[i]);
            FactionsTurn(factions[i]);
        }
    }
}
