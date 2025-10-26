using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add a battle manager so the map doesn't get too bloated, the map is just the visualization of the battle map and anything that will result in a change of that visualization, a lot of logic will be handled somewhere else.
public class BattleMap : MapManager
{
    public string weather;
    public WeatherFilter weatherFilter;
    public void SetWeather(string newInfo)
    {
        weather = newInfo;
        weatherFilter.UpdateFilter(weather);
    }
    public int battleRound;
    public void SetRound(int newInfo)
    {
        battleRound = newInfo;
    }
    public int GetRound()
    {
        return battleRound;
    }
    public string GetWeather() { return weather; }
    public string time;
    public DayNightFilter timeFilter;
    public void SetTime(string newInfo)
    { 
        time = newInfo;
        timeFilter.SetTime(time);
    }
    public string GetTime(){ return time; }
    public CombatLog combatLog;
    public BattleStatsTracker damageTracker;
    public TerrainPassivesList terrainPassives;
    public string ReturnTerrainStartPassive(TacticActor actor)
    {
        string terrainType = mapInfo[actor.GetLocation()];
        if (terrainPassives.TerrainPassivesExist(terrainType))
        {
            return terrainPassives.ReturnStartPassive(terrainType);
        }
        return "";
    }
    public string ReturnTerrainEndPassive(TacticActor actor)
    {
        string terrainType = mapInfo[actor.GetLocation()];
        if (terrainPassives.TerrainPassivesExist(terrainType))
        {
            return terrainPassives.ReturnEndPassive(terrainType);
        }
        return "";
    }
    public string ReturnTileMovingPassive(TacticActor actor)
    {
        string terrainType = mapInfo[actor.GetLocation()];
        if (terrainPassives.TerrainPassivesExist(terrainType))
        {
            return terrainPassives.ReturnMovingPassive(terrainType);
        }
        return "";
    }
    public StatDatabase terrainEffectData;
    public StatDatabase terrainWeatherInteractions;
    public StatDatabase terrainTileInteractions;
    public StatDatabase tileWeatherInteractions;
    public StatDatabase passiveData;
    public PassiveSkill passiveEffect;
    [ContextMenu("ForceStart")]
    public void ForceStart()
    {
        InitializeEmptyList();
        terrainEffectTiles = new List<string>(emptyList);
        trappedTiles = new List<string>(emptyList);
    }
    protected override void Start()
    {
        InitializeEmptyList();
        // Don't start again if you already force started.
        if (terrainEffectTiles.Count < emptyList.Count)
        {
            terrainEffectTiles = new List<string>(emptyList);
            trappedTiles = new List<string>(emptyList);
        }
        //base.Start();
    }
    public BattleManager battleManager;
    public List<TacticActor> battlingActors;
    public TacticActor ReturnLatestActor()
    {
        return battlingActors[battlingActors.Count - 1];
    }
    public void AddActorToBattle(TacticActor actor)
    {
        battlingActors.Add(actor);
        UpdateMap();
    }
    public void SwitchActorLocations(TacticActor actor1, TacticActor actor2)
    {
        int temp = actor1.GetLocation();
        actor1.SetLocation(actor2.GetLocation());
        actor2.SetLocation(temp);
        UpdateMap();
    }
    // Should highlight all valid starting tiles to make this clear.
    public bool ValidStartingTile(int tileNumber)
    {
        int column = mapUtility.GetColumn(tileNumber, mapSize);
        if (column < (AllTeamMembers(0).Count / mapSize) + 2)
        {
            return true;
        }
        return false;
    }
    //
    public List<string> excludedStartingTiles;
    public void RandomEnemyStartingPositions()
    {
        List<TacticActor> enemyTeam = AllTeamMembers(1);
        for (int i = 0; i < enemyTeam.Count; i++)
        {
            enemyTeam[i].SetLocation(RandomEnemyStartingTile());
        }
        UpdateMap();
    }
    protected int RandomEnemyStartingTile()
    {
        int tile = Random.Range(0, mapSize * mapSize);
        if (ValidEnemyStartingTile(tile))
        {
            return tile;
        }
        return RandomEnemyStartingTile();
    }
    protected bool ValidEnemyStartingTile(int tileNumber)
    {
        int column = mapUtility.GetColumn(tileNumber, mapSize);
        if (column > mapSize - (AllTeamMembers(1).Count / mapSize) - 2 && !excludedStartingTiles.Contains(mapInfo[tileNumber]) && !TileNotEmpty(tileNumber))
        {
            return true;
        }
        return false;
    }
    public void ChangeActorsLocation(int startingTile, int newTile)
    {
        TacticActor actor = GetActorOnTile(startingTile);
        if (actor == null){return;}
        actor.SetLocation(newTile);
        UpdateMap();
    }
    public int RemoveActorsFromBattle(int turnNumber = -1)
    {
        int originalTurnNumber = turnNumber;
        for (int i = battlingActors.Count - 1; i >= 0; i--)
        {
            if (battlingActors[i].GetHealth() <= 0)
            {
                // Apply the death passives here.
                combatLog.UpdateNewestLog(battlingActors[i].GetPersonalName() + " is defeated.");
                battleManager.ActiveDeathPassives(battlingActors[i]);
                battlingActors.RemoveAt(i);
                // If someone whose turn already passed dies, then the turn count needs to be decremented to avoid skipping someones turn.
                if (i <= originalTurnNumber) { turnNumber--; }
            }
        }
        return turnNumber;
    }
    public List<TacticActor> AllTeamMembers(int team)
    {
        List<TacticActor> actors = new List<TacticActor>();
        for (int i = 0; i < battlingActors.Count; i++)
        {
            if (battlingActors[i].GetTeam() == team)
            {
                actors.Add(battlingActors[i]);
            }
        }
        return actors;
    }
    public List<TacticActor> AllAllies(TacticActor actor)
    {
        List<TacticActor> actors = new List<TacticActor>();
        for (int i = 0; i < battlingActors.Count; i++)
        {
            if (battlingActors[i].GetTeam() == actor.GetTeam())
            {
                actors.Add(battlingActors[i]);
            }
        }
        return actors;
    }
    public List<TacticActor> AllEnemies(TacticActor actor)
    {
        List<TacticActor> actors = new List<TacticActor>();
        for (int i = 0; i < battlingActors.Count; i++)
        {
            if (battlingActors[i].GetTeam() != actor.GetTeam())
            {
                actors.Add(battlingActors[i]);
            }
        }
        return actors;
    }
    public List<TacticActor> AllActorsBySprite(string spriteName)
    {
        List<TacticActor> actors = new List<TacticActor>();
        for (int i = 0; i < battlingActors.Count; i++)
        {
            if (battlingActors[i].GetSpriteName().Contains(spriteName))
            {
                actors.Add(battlingActors[i]);
            }
        }
        return actors;
    }
    public bool AllyAdjacentToActor(TacticActor actor)
    {
        List<TacticActor> adjacentActors = GetAdjacentActors(actor.GetLocation());
        for (int i = 0; i < adjacentActors.Count; i++)
        {
            if (adjacentActors[i].GetTeam() == actor.GetTeam())
            {
                return true;
            }
        }
        return false;
    }
    public bool AllyAdjacentWithSpriteName(TacticActor actor, string specificSprite)
    {
        List<TacticActor> adjacentActors = GetAdjacentActors(actor.GetLocation());
        int team = actor.GetTeam();
        for (int i = 0; i < adjacentActors.Count; i++)
        {
            if (adjacentActors[i].GetTeam() == team && adjacentActors[i].GetSpriteName() == specificSprite)
            {
                return true;
            }
        }
        return false;
    }
    // List of actor names on tiles.
    public List<string> actorTiles;
    public bool TileNotEmpty(int tileNumber)
    {
        // If someone's name is on the tile then it's not empty.
        return (actorTiles[tileNumber].Length > 1);
    }
    public List<int> ReturnEmptyTiles(List<int> newTiles)
    {
        for (int i = newTiles.Count - 1; i >= 0; i--)
        {
            if (TileNotEmpty(newTiles[i]))
            {
                newTiles.RemoveAt(i);
            }
        }
        return newTiles;
    }
    // List of actor directions on tiles.
    public List<string> actorDirections;
    public StatDatabase tileTileInteractions;
    // Called during some battle passives.
    public void ChangeTile(int tileNumber, string effect, string specifics)
    {
        switch (effect)
        {
            case "Tile":
                ChangeTerrain(tileNumber, specifics);
                break;
            case "TerrainEffect":
                ChangeTEffect(tileNumber, specifics);
                break;
        }
    }
    public void ChangeTerrain(int tileNumber, string change)
    {
        if (mapInfo[tileNumber] == change){return;}
        string originalRank = tileTileInteractions.ReturnValue(mapInfo[tileNumber]);
        string newRank = tileTileInteractions.ReturnValue(change);
        if (utility.SafeParseInt(newRank, -1) > utility.SafeParseInt(originalRank, -1))
        {
            return;
        }
        mapInfo[tileNumber] = change;
        // Update the elevation.
        Debug.Log("Old Elevation:"+mapElevations[tileNumber]);
        Debug.Log("New Tile:"+mapInfo[tileNumber]);
        mapElevations[tileNumber] = RandomElevation(mapInfo[tileNumber]);
        Debug.Log("New Elevation:"+mapElevations[tileNumber]);
        battleManager.moveManager.SetMapElevations(mapElevations);
        UpdateMap();
    }
    public List<string> terrainEffectTiles;
    public virtual void GetNewTerrainEffects(MapFeaturesList mapFeatures)
    {
        terrainEffectTiles = new List<string>(emptyList);
        for (int i = 0; i < mapFeatures.features.Count; i++)
        {
            terrainEffectTiles = mapMaker.AddFeature(terrainEffectTiles, mapFeatures.features[i], mapFeatures.patterns[i]);
        }
        UpdateMap();
    }
    public void ChangeTEffect(int tileNumber, string newEffect)
    {
        terrainEffectTiles[tileNumber] = newEffect;
        UpdateMap();
    }
    public void SwitchTerrainEffect(int tile1, int tile2)
    {
        string temp = terrainEffectTiles[tile1];
        terrainEffectTiles[tile1] = terrainEffectTiles[tile2];
        terrainEffectTiles[tile2] = temp;
        UpdateMap();
    }
    public void SpreadTerrainEffect(int tileNumber)
    {
        string tEffect = terrainEffectTiles[tileNumber];
        if (tEffect == "") { return; }
        List<int> adjacent = mapUtility.AdjacentTiles(tileNumber, mapSize);
        for (int i = 0; i < adjacent.Count; i++)
        {
            terrainEffectTiles[adjacent[i]] = tEffect;
        }
    }
    public void RandomlySpreadTerrainEffect(int tileNumber)
    {
        string tEffect = terrainEffectTiles[tileNumber];
        if (tEffect == "") { return; }
        List<int> adjacent = mapUtility.AdjacentTiles(tileNumber, mapSize);
        terrainEffectTiles[adjacent[Random.Range(0, adjacent.Count)]] = tEffect;
    }
    protected void UpdateTerrain()
    {
        mapDisplayers[2].DisplayCurrentTiles(mapTiles, terrainEffectTiles, currentTiles);
    }
    public List<string> trappedTiles;
    public List<string> stoppingTraps; // Traps that immediately stop movement.
    public void ChangeTrap(int tileNumber, string newTrap)
    {
        trappedTiles[tileNumber] = newTrap;
    }
    public void TriggerTrap(int tileNumber)
    {
        trappedTiles[tileNumber] = "";
    }
    // Just like traps, you can't see these.
    // Better remember well or write it down if it's an AOE.
    // Enemies will get around this by either being immune, or just accepting death.
    public StatDatabase delayedEffectData;
    public List<string> delayedEffects;
    public List<int> delayedEffectTiles;
    public List<int> delayedEffectTimers;
    public void AddDelayedEffect(string effect, int tile, int timer)
    {
        delayedEffects.Add(effect);
        delayedEffectTiles.Add(tile);
        delayedEffectTimers.Add(timer);
    }
    public void RemoveDelayedEffect(int index)
    {
        delayedEffects.RemoveAt(index);
        delayedEffectTiles.RemoveAt(index);
        delayedEffectTimers.RemoveAt(index);
    }
    public void IncrementDelayedEffects()
    {
        for (int i = delayedEffects.Count - 1; i >= 0; i--)
        {
            delayedEffectTimers[i] = delayedEffectTimers[i] - 1;
            if (delayedEffectTimers[i] <= 0)
            {
                // Apply the effect.
                ActivateDelayedEffect(delayedEffects[i], delayedEffectTiles[i]);
                RemoveDelayedEffect(i);
            }
        }
    }
    public void ActivateDelayedEffect(string effectKey, int tile)
    {
        string effectDetails = delayedEffectData.ReturnValue(effectKey);
        if (effectDetails == "")
        {
            ApplyDelayedEffect(effectKey, tile);
        }
        else
        {
            ApplyDelayedEffect(effectDetails, tile, true, effectKey);
        }
    }
    protected void ApplyDelayedEffect(string effect, int tile, bool setEffect = false, string effectName = "")
    {
        string[] blocks = effect.Split("|");
        string[] targets = blocks[0].Split(",");
        string[] effects = blocks[1].Split(",");
        string[] specifics = blocks[2].Split(",");
        for (int i = 0; i < targets.Length; i++)
        {
            switch (targets[i])
            {
                case "Actor":
                    // Get the actor on the tile.
                    TacticActor target = GetActorOnTile(tile);
                    if (target == null)
                    {
                        Debug.Log("No Actor Found");
                        break;
                    }
                    if (setEffect)
                    {
                        combatLog.UpdateNewestLog(effectName + " affects " + target.GetPersonalName());
                    }
                    passiveEffect.AffectActor(target, effects[i], specifics[i], 1, combatLog);
                    break;
                case "Tile":
                    ChangeTerrain(tile, effects[i]);
                    break;
                case "Time":
                    SetTime(effects[i]);
                    break;
                case "Weather":
                    SetWeather(effects[i]);
                    break;
                case "Ritual Summon":
                    TacticActor sacrifice = GetActorOnTile(tile);
                    if (sacrifice == null)
                    {
                        combatLog.UpdateNewestLog("Ritual summoning failed!");
                        break;
                    }
                    // Kill the target.
                    combatLog.UpdateNewestLog(sacrifice.GetPersonalName() + " is consumed by the ritual.");
                    sacrifice.SetCurrentHealth(-1);
                    battleManager.ActiveDeathPassives(sacrifice);
                    battlingActors.Remove(sacrifice);
                    // Summon an enemy.
                    battleManager.SpawnAndAddActor(tile, effects[i], sacrifice.GetTeam());
                    combatLog.UpdateNewestLog("By offering " + sacrifice.GetPersonalName() + " a " + effects[i] + " is summoned.");
                    break;
            }
        }
    }
    public List<string> highlightedTiles;
    public ColorDictionary colorDictionary;

    protected virtual void GetActorTiles()
    {
        if (emptyList == null || emptyList.Count < mapSize * mapSize) { InitializeEmptyList(); }
        actorTiles = new List<string>(emptyList);
        actorDirections = new List<string>(emptyList);
        for (int i = 0; i < battlingActors.Count; i++)
        {
            actorTiles[battlingActors[i].location] = battlingActors[i].GetSpriteName();
            actorDirections[battlingActors[i].location] = battlingActors[i].GetDirection().ToString();
        }
    }

    [ContextMenu("Clear Actors")]
    public void ClearActors()
    {
        for (int i = 0; i < battlingActors.Count; i++)
        {
            if (battlingActors[i] == null) { continue; }
            battlingActors[i].DestroyActor();
        }
        battlingActors.Clear();
    }

    public override void UpdateMap()
    {
        base.UpdateMap();
        UpdateActors();
        UpdateTerrain();
    }

    public void UpdateActors()
    {
        GetActorTiles();
        mapDisplayers[1].DisplayCurrentTiles(mapTiles, actorTiles, currentTiles, true, actorDirections);
    }

    public void UpdateMovingHighlights(TacticActor selectedActor, MoveCostManager moveManager, bool current = true)
    {
        if (emptyList.Count < mapSize * mapSize) { InitializeEmptyList(); }
        highlightedTiles = new List<string>(emptyList);
        int maxActions = 2;
        if (current) { maxActions = selectedActor.GetActions(); }
        for (int i = Mathf.Min(maxActions, colorDictionary.keys.Count - 2); i >= 0; i--)
        {
            UpdateHighlightsWithoutReseting(moveManager.GetReachableTilesBasedOnActions(selectedActor, battlingActors, i), colorDictionary.keys[i + 1]);
        }
        //UpdateHighlights(moveManager.GetAllReachableTiles(selectedActor, battlingActors, current));
    }

    protected void UpdateHighlightsWithoutReseting(List<int> newTiles, string colorKey = "MoveClose", int layer = 3)
    {
        string colorName = colorDictionary.GetColorNameByKey(colorKey);
        for (int i = 0; i < newTiles.Count; i++)
        {
            highlightedTiles[newTiles[i]] = colorName;
        }
        mapDisplayers[layer].HighlightCurrentTiles(mapTiles, highlightedTiles, currentTiles);
    }

    public void UpdateStartingPositionTiles()
    {
        List<int> tiles = new List<int>();
        for (int i = 0; i < mapSize * mapSize; i++)
        {
            if (mapUtility.GetColumn(i, mapSize) < (AllTeamMembers(0).Count / mapSize) + 2)
            {
                tiles.Add(i);
            }
        }
        UpdateHighlights(tiles, "Green", 4);
    }

    public void UpdateHighlights(List<int> newTiles, string colorKey = "MoveClose", int layer = 3)
    {
        string colorName = colorDictionary.GetColorNameByKey(colorKey);
        if (emptyList.Count < mapSize * mapSize) { InitializeEmptyList(); }
        highlightedTiles = new List<string>(emptyList);
        for (int i = 0; i < newTiles.Count; i++)
        {
            highlightedTiles[newTiles[i]] = colorName;
        }
        mapDisplayers[layer].HighlightCurrentTiles(mapTiles, highlightedTiles, currentTiles);
    }

    [ContextMenu("Reset Highlights")]
    public void ResetHighlights()
    {
        if (emptyList.Count < mapSize * mapSize) { InitializeEmptyList(); }
        highlightedTiles = new List<string>(emptyList);
        mapDisplayers[3].HighlightCurrentTiles(mapTiles, highlightedTiles, currentTiles);
        mapDisplayers[4].HighlightCurrentTiles(mapTiles, highlightedTiles, currentTiles);
    }

    public override void ClickOnTile(int tileNumber)
    {
        battleManager.ClickOnTile(tileNumber);
    }

    public TacticActor GetActorOnTile(int tileNumber)
    {
        if (tileNumber < 0){ return null; }
        string actorName = actorTiles[tileNumber];
        for (int i = 0; i < battlingActors.Count; i++)
        {
            // Some actors are not interactable and should be returned as null. IE buildings. Also if their health is less than zero then they can't be interacted with anymore.
            if (battlingActors[i].GetSpriteName() == actorName && battlingActors[i].GetLocation() == tileNumber && battlingActors[i].GetHealth() > 0)
            {
                return battlingActors[i];
            }
        }
        return null;
    }

    public TacticActor GetActorByIndex(int index)
    {
        if (index < 0 || index >= battlingActors.Count) { return null; }
        return battlingActors[index];
    }

    public string GetTileInfoOfActor(TacticActor actor)
    {
        return mapInfo[actor.GetLocation()];
    }

    public List<TacticActor> GetActorsOnTiles(List<int> tiles)
    {
        List<TacticActor> actors = new List<TacticActor>();
        for (int i = 0; i < tiles.Count; i++)
        {
            TacticActor testActor = GetActorOnTile(tiles[i]);
            if (testActor != null)
            {
                actors.Add(testActor);
            }
        }
        return actors;
    }

    public List<TacticActor> GetAdjacentActors(int tileNumber)
    {
        return GetActorsOnTiles(mapUtility.AdjacentTiles(tileNumber, mapSize));
    }

    public List<int> GetAttackableTiles(TacticActor actor)
    {
        return mapUtility.GetTilesInCircleShape(actor.GetLocation(), actor.GetAttackRange(), mapSize);
    }

    public List<TacticActor> GetAttackableEnemies(TacticActor actor)
    {
        List<TacticActor> attackableEnemies = GetActorsOnTiles(GetAttackableTiles(actor));
        for (int i = attackableEnemies.Count - 1; i >= 0; i--)
        {
            if (attackableEnemies[i].GetTeam() == actor.GetTeam())
            {
                attackableEnemies.RemoveAt(i);
            }
        }
        return attackableEnemies;
    }

    public int GetRandomEnemyLocation(TacticActor actor, List<int> targetedTiles)
    {
        List<TacticActor> enemies = GetActorsOnTiles(targetedTiles);
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i].GetTeam() == actor.GetTeam())
            {
                enemies.RemoveAt(i);
            }
        }
        if (enemies.Count == 0)
        {
            return -1;
        }
        return enemies[Random.Range(0, enemies.Count)].GetLocation();
    }

    public TacticActor GetClosestEnemy(TacticActor actor)
    {
        List<TacticActor> enemies = AllEnemies(actor);
        int index = -1;
        int distance = mapSize * 2;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (mapUtility.DistanceBetweenTiles(actor.GetLocation(), enemies[i].GetLocation(), mapSize) < distance)
            {
                distance = mapUtility.DistanceBetweenTiles(actor.GetLocation(), enemies[i].GetLocation(), mapSize);
                index = i;
            }
        }
        return enemies[index];
    }

    public List<int> GetAdjacentEmptyTiles(int tileNumber)
    {
        List<int> allAdjacent = mapUtility.AdjacentTiles(tileNumber, mapSize);
        for (int i = 0; i < allAdjacent.Count; i++)
        {
            if (TileNotEmpty(allAdjacent[i]))
            {
                allAdjacent.RemoveAt(i);
            }
        }
        return allAdjacent;
    }

    public int ReturnRandomAdjacentEmptyTile(int tileNumber)
    {
        List<int> emptyAdjacent = GetAdjacentEmptyTiles(tileNumber);
        if (emptyAdjacent.Count == 0)
        {
            return tileNumber;
        }
        int tile = emptyAdjacent[Random.Range(0, emptyAdjacent.Count)];
        if (TileNotEmpty(tile))
        {
            return tileNumber;
        }
        return tile;
    }

    public int ReturnTileInRelativeDirection(TacticActor actor, int relativeDirection)
    {
        int direction = (actor.GetDirection() + relativeDirection) % 6;
        return mapUtility.PointInDirection(actor.GetLocation(), direction, mapSize);
    }

    public bool AlliesInTiles(TacticActor actor, List<int> tiles)
    {
        int team = actor.GetTeam();
        for (int i = 0; i < tiles.Count; i++)
        {
            TacticActor tileActor = GetActorOnTile(tiles[i]);
            if (tileActor == null)
            {
                continue;
            }
            if (tileActor.GetTeam() == team)
            {
                return true;
            }
        }
        return false;
    }

    public List<TacticActor> ReturnAlliesInTiles(TacticActor actor, List<int> tiles)
    {
        int team = actor.GetTeam();
        List<TacticActor> actors = new List<TacticActor>();
        for (int i = 0; i < tiles.Count; i++)
        {
            TacticActor tileActor = GetActorOnTile(tiles[i]);
            if (tileActor == null)
            {
                continue;
            }
            if (tileActor.GetTeam() == team)
            {
                actors.Add(tileActor);
            }
        }
        return actors;
    }

    public bool EnemiesInTiles(TacticActor actor, List<int> tiles)
    {
        int team = actor.GetTeam();
        for (int i = 0; i < tiles.Count; i++)
        {
            TacticActor tileActor = GetActorOnTile(tiles[i]);
            if (tileActor == null)
            {
                continue;
            }
            if (tileActor.GetTeam() != team)
            {
                return true;
            }
        }
        return false;
    }

    public List<TacticActor> ReturnEnemiesInTiles(TacticActor actor, List<int> tiles)
    {
        int team = actor.GetTeam();
        List<TacticActor> actors = new List<TacticActor>();
        for (int i = 0; i < tiles.Count; i++)
        {
            TacticActor tileActor = GetActorOnTile(tiles[i]);
            if (tileActor == null)
            {
                continue;
            }
            if (tileActor.GetTeam() != team)
            {
                actors.Add(tileActor);
            }
        }
        return actors;
    }

    public List<TacticActor> GetAdjacentAllies(TacticActor actor)
    {
        List<TacticActor> all = new List<TacticActor>();
        all = GetAdjacentActors(actor.GetLocation());
        for (int i = all.Count - 1; i >= 0; i--)
        {
            if (all[i].GetTeam() != actor.GetTeam()) { all.RemoveAt(i); }
        }
        return all;
    }

    public List<TacticActor> GetAdjacentEnemies(TacticActor actor)
    {
        List<TacticActor> all = new List<TacticActor>();
        all = GetAdjacentActors(actor.GetLocation());
        for (int i = all.Count - 1; i >= 0; i--)
        {
            if (all[i].GetTeam() == actor.GetTeam()){all.RemoveAt(i);}
        }
        return all;
    }

    public void ApplyMovingTileEffect(TacticActor actor, int tileNumber)
    {
        ApplyTileMovingEffect(actor, tileNumber);
        ApplyTerrainEffect(actor, tileNumber);
        ApplyTrapEffect(actor, tileNumber);
    }

    public void ApplyTileMovingEffect(TacticActor actor, int tileNumber)
    {
        string tileEffect = ReturnTileMovingPassive(actor);
        if (tileEffect.Length < 1) { return; }
        List<string> data = passiveData.ReturnStats(tileEffect);
        if (passiveEffect.CheckStartEndConditions(actor, data[1], data[2], this))
        {
            passiveEffect.AffectActor(actor, data[4], data[5]);
        }
    }

    public void ApplyTerrainEffect(TacticActor actor, int tileNumber)
    {
        // Get the terrain info.
        string terrainEffect = terrainEffectTiles[tileNumber];
        if (terrainEffect.Length < 1) { return; }
        // Apply the terrain effect.
        List<string> data = terrainEffectData.ReturnStats(terrainEffect);
        passiveEffect.AffectActor(actor, data[0], data[1]);
    }

    public bool ApplyTrapEffect(TacticActor actor, int tileNumber)
    {
        string trapEffect = trappedTiles[tileNumber];
        if (trapEffect.Length < 1) { return false; }
        List<string> data = terrainEffectData.ReturnStats(trapEffect);
        passiveEffect.AffectActor(actor, data[0], data[1]);
        TriggerTrap(tileNumber);
        combatLog.UpdateNewestLog(actor.GetPersonalName() + " triggers a " + trapEffect + " trap.");
        // If a trap forces actors to stop then return true.
        if (stoppingTraps.Contains(trapEffect)) { return true; }
        return false;
    }

    public void ApplyEndTerrainEffect(TacticActor actor)
    {
        string terrainEffect = terrainEffectTiles[actor.GetLocation()];
        if (terrainEffect.Length < 1) { return; }
        List<string> data = terrainEffectData.ReturnStats(terrainEffect);
        if (data.Count < 4) { return; }
        passiveEffect.AffectActor(actor, data[2], data[3]);
    }

    public void NextRound()
    {
        // Apply weather/tile to terrain effects.
        string t_w = "";
        string t_t = "";
        List<int> spreadingEffects = new List<int>();
        List<int> expandingEffects = new List<int>();
        for (int i = 0; i < terrainEffectTiles.Count; i++)
        {
            if (terrainEffectTiles[i] == ""){ continue; }
            t_w = terrainEffectTiles[i] + "-" + GetWeather();
            t_t = terrainEffectTiles[i] + "-" + mapInfo[i];
            if (terrainWeatherInteractions.ReturnValue(t_w) == "Remove" || terrainTileInteractions.ReturnValue(t_t) == "Remove")
            {
                ChangeTEffect(i, "");
                continue;
            }
            else if (terrainWeatherInteractions.ReturnValue(t_w) == "Expand" || terrainTileInteractions.ReturnValue(t_t) == "Expand")
            {
                expandingEffects.Add(i);
                continue;
            }
            else if (terrainWeatherInteractions.ReturnValue(t_w) == "Spread" || terrainTileInteractions.ReturnValue(t_t) == "Spread")
            {
                spreadingEffects.Add(i);
                continue;
            }
        }
        for (int i = 0; i < spreadingEffects.Count; i++)
        {
            RandomlySpreadTerrainEffect(spreadingEffects[i]);
        }
        for (int i = 0; i < expandingEffects.Count; i++)
        {
            SpreadTerrainEffect(expandingEffects[i]);
        }
        // Apply weather effects to tiles.
        for (int i = 0; i < mapInfo.Count; i++)
        {
            t_w = mapInfo[i] + "-" + GetWeather();
            string effectAndSpecifics = tileWeatherInteractions.ReturnValue(t_w);
            string[] blocks = effectAndSpecifics.Split("-");
            if (blocks.Length < 2) { continue; }
            switch (blocks[0])
            {
                case "Tile":
                    ChangeTerrain(i, blocks[1]);
                    break;
                case "Feature":
                    ChangeTEffect(i, blocks[1]);
                    break;
            }
        }
        IncrementDelayedEffects();
        UpdateMap();
    }
}
