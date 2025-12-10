using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterFactionUnitManager : MonoBehaviour
{
    public FactionMap map;
    public FactionManager factionManager;
    // Knows all faction/enemy units.
    public List<FactionUnitDataManager> factionUnits;
    // This should always load and save the first unit on the tile of the first faction which has a unit on that tile.
    public void LoadWorkerOnTile(int tile)
    {
        for (int i = 0; i < factionUnits.Count; i++)
        {
            if (factionUnits[i].UnitAtLocation(tile))
            {
                dummyUnit.LoadStats(factionUnits[i].ReturnUnitAtLocation(tile));
                break;
            }
        }
    }
    public void UpdateWorkerOnTile(int tile)
    {
        for (int i = 0; i < factionUnits.Count; i++)
        {
            if (factionUnits[i].UnitAtLocation(tile))
            {
                factionUnits[i].UpdateUnitByLocation(dummyUnit);
                break;
            }
        }
    }
    public List<FactionEnemyUnitData> enemyUnits;
    public FactionUnit dummyUnit;
    // Need two combat units incase they fight.
    public UnitCombatManager combatManager;
    public CombatUnit turnCombatUnit;
    public CombatUnit otherCombatUnit;
    // Helper things.
    public List<int> path;

    public void AllTurns()
    {
        // Factions act first.
        for (int i = 0; i < factionUnits.Count; i++)
        {
            FactionsTurn(factionUnits[i]);
        }
        // Then enemies act.
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            EnemiesTurn(enemyUnits[i]);
        }
    }

    protected void MoveAlongPath(List<int> path, FactionUnit unit, bool extraMoveUsed = false)
    {
        int nextTile = path[path.Count - 1];
        path.RemoveAt(path.Count - 1);
        int moveCost = map.GetMoveCost(nextTile);
        if (unit.movement >= moveCost)
        {
            unit.SetLocation(nextTile);
            unit.UseMovement(moveCost);
        }
        // If you can't afford to move there.
        else if (unit.movement < moveCost && !extraMoveUsed)
        {
            extraMoveUsed = true;
            unit.MoveFaster();
            unit.SetLocation(nextTile);
            unit.UseMovement(moveCost);
        }
        else if (unit.movement < moveCost && extraMoveUsed)
        {
            return;
        }
        // Arrived
        if (path.Count <= 0){return;}
        // If you can afford it, then keep moving.
        if (unit.movement + unit.baseSpeed >= map.GetMoveCost(path[path.Count - 1]))
        {
            MoveAlongPath(path, unit, extraMoveUsed);
        }
    }

    protected void FactionsTurn(FactionUnitDataManager unitData)
    {
        for (int i = 0; i < unitData.unitData.Count; i++)
        {
            UnitTurn(unitData, i);
        }
        for (int i = 0; i < unitData.combatUnitData.Count; i++)
        {
            CombatUnitTurn(unitData, i);
        }
        factionManager.UnitDeathsAndDesertions(unitData);
    }

    protected void UnitTurn(FactionUnitDataManager faction, int index)
    {
        dummyUnit.LoadStats(faction.GetUnitDataAtIndex(index));
        if (dummyUnit.Dead()){return;}
        if (dummyUnit.energy <= 0)
        {
            dummyUnit.Relax();
            faction.UpdateUnitAtIndex(dummyUnit, index);
            return;
        }
        // If at a city, and injured then rest to heal.
        else if (dummyUnit.GetLocation() == factionManager.GetCapitalLocationOfFaction(dummyUnit.GetFaction()) && dummyUnit.Hurt())
        {
            dummyUnit.Rest();
            faction.UpdateUnitAtIndex(dummyUnit, index);
            return;
        }
        // If inventory full and at capital, then drop off items and update goal.
        else if (dummyUnit.InventoryFull() && dummyUnit.GetLocation() == factionManager.GetCapitalLocationOfFaction(dummyUnit.GetFaction()))
        {
            if (dummyUnit.CompletedGoal())
            {
                dummyUnit.GainExp(1);
            }
            factionManager.UnitDepositsInventory(dummyUnit, dummyUnit.GetLocation());
            faction.UpdateUnitAtIndex(dummyUnit, index);
            return;
        }
        // If inventory full and not at the capital, path towards capital/village.
        else if (dummyUnit.InventoryFull() && dummyUnit.GetLocation() != factionManager.GetCapitalLocationOfFaction(dummyUnit.GetFaction()))
        {
            List<int> path = map.pathfinder.BasicPathToTile(dummyUnit.GetLocation(), factionManager.GetCapitalLocationOfFaction(dummyUnit.GetFaction()));
            MoveAlongPath(path, dummyUnit);
            faction.UpdateUnitAtIndex(dummyUnit, index);
            return;
        }
        int targetTile = map.ReturnUnoccupiedTileWithLargestOutput(factionManager.GetOwnedTilesOfFaction(dummyUnit.GetFaction()), dummyUnit.GetGoalSpecifics(), dummyUnit.GetLocation());
        // Try again but get the closest tile outside of the borders.
        if (targetTile < 0)
        {
            targetTile = map.ReturnClosestTileWithOutput(dummyUnit.GetLocation(), dummyUnit.GetGoalSpecifics());
        }
        if (targetTile < 0)
        {
            // Literally nothing to do. Pass the turn.
            faction.UpdateUnitAtIndex(dummyUnit, index);
            return;
        }
        else if (!dummyUnit.InventoryFull() && dummyUnit.GetLocation() == targetTile)
        {
            dummyUnit.UseEnergy();
            dummyUnit.GainItems(map.ReturnTileOutput(targetTile).Split("+").ToList());
        }
        // If not on a gathering spot, move toward gathering spot.
        else
        {
            List<int> path = map.pathfinder.BasicPathToTile(dummyUnit.GetLocation(), targetTile);
            MoveAlongPath(path, dummyUnit);
            faction.UpdateUnitLocation(index, dummyUnit.GetLocation());
        }
        // You can rob or help these units when encountering them on the map, simplest way to change reputation with a faction.
        faction.UpdateUnitAtIndex(dummyUnit, index);
    }

    protected void CombatUnitTurn(FactionUnitDataManager faction, int index)
    {
        // Act based on goals.
    }

    protected int TargetOnTile(int tileNumber, string except)
    {
        // Need to except soldiers of the same type.
        if (map.SoldierOnTile(tileNumber, except)){return 0;}
        else if (map.WorkerOnTile(tileNumber)){return 1;}
        else if (map.BuildingOnTile(tileNumber)){return 2;}
        return 0;
    }

    protected void AttackTarget(int targetType, int targetLocation)
    {
        turnCombatUnit.GainExp();
        switch (targetType)
        {
            default:
            break;
            case 1:
            // Attack Worker.
            LoadWorkerOnTile(targetLocation);
            // Get the worker on the tile.
            dummyUnit.TakeDamage(1);
            if (dummyUnit.Dead())
            {
                // If you kill the worker then steal their stuff.
                turnCombatUnit.GainItems(dummyUnit.inventory);
            }
            UpdateWorkerOnTile(targetLocation);
            break;
            case 2:
            // Attack Building.
            map.DestroyBuilding(targetLocation);
            turnCombatUnit.FillInventory();
            break;
        }
    }

    protected void EnemiesTurn(FactionEnemyUnitData enemy)
    {
        enemy.Load();
        for (int i = 0; i < enemy.unitData.Count; i++)
        {
            EnemyTurn(enemy, i);
        }
        enemy.Save();
    }

    protected void EnemyTurn(FactionEnemyUnitData enemy, int index)
    {
        turnCombatUnit.LoadStats(enemy.GetUnitDataAtIndex(index));
        int turnLocation = turnCombatUnit.GetLocation();
        if (turnCombatUnit.InventoryFull())
        {
            // If on a base.
            if (enemy.SpawnPointOnTile(turnLocation))
            {
                enemy.CollectTreasure(turnCombatUnit);
                enemy.UpdateUnitAtIndex(turnCombatUnit, index);
                return;
            }
            // Path towards a base.
            else
            {
                // If no bases this will make you stand still.
                path = map.pathfinder.BasicPathToTile(turnLocation, map.mapUtility.ReturnClosestTile(turnLocation, enemy.spawnPoints, map.mapSize));
                MoveAlongPath(path, turnCombatUnit);
                enemy.UpdateUnitAtIndex(turnCombatUnit, index);
                return;
            }
        }
        int targetLocation = -1;
        int target = TargetOnTile(turnLocation, enemy.GetUnitSpriteName());
        if (target > 0)
        {
            // Attack the target/building.
            AttackTarget(target, turnLocation);
            enemy.UpdateUnitAtIndex(turnCombatUnit, index);
            return;
        }
        else if (target == 0)
        {
            // Check adjacent tiles for a building or worker.
            List<int> adjacentTiles = map.mapUtility.AdjacentTiles(turnLocation, map.mapSize);
            for (int i = 0; i < adjacentTiles.Count; i++)
            {
                target = TargetOnTile(adjacentTiles[i], enemy.GetUnitSpriteName());
                if (target > 0)
                {
                    targetLocation = adjacentTiles[i];
                    AttackTarget(target, targetLocation);
                    enemy.UpdateUnitAtIndex(turnCombatUnit, index);
                    return;
                }
            }
        }
        // Start pathing towards the nearest building/worker.
        targetLocation = map.ReturnClosestBuildingTile(turnLocation);
        path = map.pathfinder.BasicPathToTile(turnLocation, targetLocation);
        MoveAlongPath(path, turnCombatUnit);
        enemy.UpdateUnitAtIndex(turnCombatUnit, index);
    }
}
