using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionUnitManager : MonoBehaviour
{
    public FactionMap map;
    public FactionManager factionManager;
    public FactionUnit dummyUnit;
    public CombatUnit dummyCombatUnit;

    public void FactionMakesWorker(FactionData faction)
    {
        // Apply the cost.
        // Make the worker.
        dummyUnit.ResetStats();
        dummyUnit.SetFaction(faction.GetFactionName());
        dummyUnit.SetLocation(faction.GetCapitalLocation());
        dummyUnit.SetGoalSpecifics(faction.LowestResource());
        // Add them to the unit list.
        faction.unitData.AddUnit(dummyUnit.GetStats(), faction.GetCapitalLocation());
    }

    public void ChangeUnitMorale(FactionUnitDataManager faction, int amount)
    {
        List<string> unitData = faction.GetUnitData();
        for (int i = 0; i < unitData.Count; i++)
        {
            dummyUnit.LoadStats(unitData[i]);
            dummyUnit.AdjustLoyalty(amount);
            faction.UpdateUnitAtIndex(dummyUnit, i);
        }
        unitData = faction.GetCombatUnitData();
        for (int i = 0; i < unitData.Count; i++)
        {
            dummyCombatUnit.LoadStats(unitData[i]);
            dummyCombatUnit.AdjustLoyalty(amount);
            faction.UpdateCombatUnitAtIndex(dummyCombatUnit, i);
        }
    }

    public void UnitDeathsAndDesertions(FactionUnitDataManager faction)
    {
        List<string> unitData = faction.GetUnitData();
        for (int i = unitData.Count - 1; i >= 0; i--)
        {
            dummyUnit.LoadStats(unitData[i]);
            if (dummyUnit.GetLoyalty() <= 0 || dummyUnit.Dead())
            {
                faction.RemoveUnitAtIndex(i);
            }
        }
        unitData = faction.GetCombatUnitData();
        for (int i = unitData.Count - 1; i >= 0; i--)
        {
            dummyCombatUnit.LoadStats(unitData[i]);
            if (dummyCombatUnit.GetLoyalty() <= 0 || dummyUnit.Dead())
            {
                faction.RemoveCombatUnitAtIndex(i);
            }
        }
    }

    public void AllTurns(List<FactionData> factions)
    {
        for (int i = 0; i < factions.Count; i++)
        {
            FactionsTurn(factions[i]);
        }
    }

    protected void FactionsTurn(FactionData faction)
    {
        for (int i = 0; i < faction.unitData.unitData.Count; i++)
        {
            UnitTurn(faction.unitData, i);
        }
        for (int i = 0; i < faction.unitData.combatUnitData.Count; i++)
        {
            CombatUnitTurn(faction.unitData, i);
        }
        UnitDeathsAndDesertions(faction.unitData);
    }

    protected void UnitTurn(FactionUnitDataManager faction, int index)
    {
        // Load the unit.
        dummyUnit.LoadStats(faction.GetUnitDataAtIndex(index));
        if (dummyUnit.Dead()){return;}
        // If inventory full and at capital, then drop off items and update goal.
        if (dummyUnit.InventoryFull() && dummyUnit.GetLocation() == factionManager.GetCapitalLocationOfFaction(dummyUnit.GetFaction()))
        {
            factionManager.UnitDepositsInventory(dummyUnit, dummyUnit.GetLocation());
            faction.UpdateUnitAtIndex(dummyUnit, index);
            return;
        }
        // If inventory full and not at the capital, path towards capital/village.
        else if (dummyUnit.InventoryFull() && dummyUnit.GetLocation() != factionManager.GetCapitalLocationOfFaction(dummyUnit.GetFaction()))
        {
            List<int> path = map.pathfinder.BasicPathToTile(dummyUnit.GetLocation(), factionManager.GetCapitalLocationOfFaction(dummyUnit.GetFaction()));
            dummyUnit.SetLocation(path[path.Count - 1]);
            faction.UpdateUnitAtIndex(dummyUnit, index);
            return;
        }
        // Otherwise we'll check if they're on the best gathering spot possible.
        int targetTile = map.ReturnUnoccupiedTileWithLargestOutput(factionManager.GetOwnedTilesOfFaction(dummyUnit.GetFaction()), dummyUnit.GetGoalSpecifics(), dummyUnit.GetLocation());
        // Try again but get the closest tile outside of the borders.
        if (targetTile < 0)
        {
            targetTile = map.ReturnClosestTileWithOutput(dummyUnit.GetLocation(), dummyUnit.GetGoalSpecifics());
        }
        if (targetTile < 0)
        {
            targetTile = map.ReturnUnoccupiedTileWithLargestOutput(factionManager.GetOwnedTilesOfFaction(dummyUnit.GetFaction()), "Gold", dummyUnit.GetLocation());
        }
        if (targetTile < 0)
        {
            targetTile = map.ReturnUnoccupiedTileWithLargestOutput(factionManager.GetOwnedTilesOfFaction(dummyUnit.GetFaction()), "Materials", dummyUnit.GetLocation());
        }
        if (targetTile < 0)
        {
            targetTile = map.ReturnUnoccupiedTileWithLargestOutput(factionManager.GetOwnedTilesOfFaction(dummyUnit.GetFaction()), "Food", dummyUnit.GetLocation());
        }
        if (targetTile < 0)
        {
            // Literally nothing to do. Pass the turn.
            // Save the actor here.
            faction.UpdateUnitAtIndex(dummyUnit, index);
            return;
        }
        // If on an unoccupied gathering spot then gather.
        else if (!dummyUnit.InventoryFull() && dummyUnit.GetLocation() == targetTile)
        {
            // This leads to a concentration of workers at buildings.
            dummyUnit.GainItems(map.ReturnTileOutput(targetTile).Split("+").ToList());
            // All the better to rob and destroy.
        }
        // If not on a gathering spot, move toward gathering spot.
        else
        {
            List<int> path = map.pathfinder.BasicPathToTile(dummyUnit.GetLocation(), targetTile);
            dummyUnit.SetLocation(path[path.Count - 1]);
            faction.UpdateUnitLocation(index, dummyUnit.GetLocation());
        }
        // You can rob or help these units when encountering them on the map, simplest way to change reputation with a faction.
        faction.UpdateUnitAtIndex(dummyUnit, index);
    }

    protected void CombatUnitTurn(FactionUnitDataManager faction, int index)
    {
        // Act based on goals.
    }
}
