using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add a battle manager so the map doesn't get too bloated, the map is just the visualization of the battle map, a lot of logic will be handled somewhere else.
public class BattleMap : MapManager
{
    public string weather;
    public WeatherFilter weatherFilter;
    public void SetWeather(string newInfo)
    {
        weather = newInfo;
        weatherFilter.UpdateFilter(weather);
    }
    public string GetWeather() { return weather; }
    public string time;
    public void SetTime(string newInfo){ time = newInfo; }
    public string GetTime(){ return time; }
    public CombatLog combatLog;
    public BattleStatsTracker damageTracker;
    public TerrainPassivesList terrainPassives;
    public string ReturnTerrainStartPassive(TacticActor actor)
    {
        string terrainType = mapInfo[actor.GetLocation()];
        if (terrainPassives.TerrainPassivesExist(terrainType))
        {
            return terrainPassives.ReturnTerrainPassive(terrainType).GetStartPassive();
        }
        return "";
    }
    public string ReturnTerrainEndPassive(TacticActor actor)
    {
        string terrainType = mapInfo[actor.GetLocation()];
        if (terrainPassives.TerrainPassivesExist(terrainType))
        {
            return terrainPassives.ReturnTerrainPassive(terrainType).GetEndPassive();
        }
        return "";
    }
    public StatDatabase terrainEffectData;
    public StatDatabase terrainWeatherInteractions;
    public StatDatabase terrainTileInteractions;
    public StatDatabase tileWeatherInteractions;
    public SkillEffect effect;
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
        terrainEffectTiles = new List<string>(emptyList);
        trappedTiles = new List<string>(emptyList);
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
    public int RemoveActorsFromBattle(int turnNumber = -1)
    {
        for (int i = battlingActors.Count - 1; i >= 0; i--)
        {
            if (battlingActors[i].GetHealth() <= 0)
            {
                // Apply the death passives here.
                combatLog.UpdateNewestLog(battlingActors[i].GetPersonalName() + " is defeated.");
                battleManager.ActiveDeathPassives(battlingActors[i]);
                battlingActors.RemoveAt(i);
                // If someone whose turn already passed dies, then the turn count needs to be decremented to avoid skipping someones turn.
                if (i <= turnNumber) { turnNumber--; }
            }
        }
        return turnNumber;
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
                ChangeTerrainEffect(tileNumber, specifics);
                break;
        }
    }
    public void ChangeTerrain(int tileNumber, string change)
    {
        string changeKey = mapInfo[tileNumber]+">"+change;
        string interaction = tileTileInteractions.ReturnValue(changeKey);
        if (interaction == "")
        {
            // Straight forward change ignoring everything else.
            mapInfo[tileNumber] = change;
        }
        else
        {
            mapInfo[tileNumber] = interaction;
        }
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
    public void ChangeTerrainEffect(int tileNumber, string newEffect)
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
        for (int i = maxActions; i >= 0; i--)
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
        ApplyTerrainEffect(actor, tileNumber);
        ApplyTrapEffect(actor, tileNumber);
    }

    public void ApplyTerrainEffect(TacticActor actor, int tileNumber)
    {
        // Get the terrain info.
        string terrainEffect = terrainEffectTiles[tileNumber];
        if (terrainEffect.Length < 1) { return; }
        // Apply the terrain effect.
        List<string> data = terrainEffectData.ReturnStats(terrainEffect);
        effect.AffectActor(actor, data[0], data[1]);
    }

    public bool ApplyTrapEffect(TacticActor actor, int tileNumber)
    {
        string trapEffect = trappedTiles[tileNumber];
        if (trapEffect.Length < 1) { return false; }
        List<string> data = terrainEffectData.ReturnStats(trapEffect);
        effect.AffectActor(actor, data[0], data[1]);
        TriggerTrap(tileNumber);
        combatLog.UpdateNewestLog(actor.GetPersonalName() + " triggers a " + trapEffect + " trap.");
        // If a trap forces actors to stop then return true.
        if (stoppingTraps.Contains(trapEffect)) { return true; }
        return false;
    }

    public void ApplyEndTileEffect(TacticActor actor)
    {
        string terrainEffect = terrainEffectTiles[actor.GetLocation()];
        if (terrainEffect.Length < 1) { return; }
        List<string> data = terrainEffectData.ReturnStats(terrainEffect);
        if (data.Count < 4) { return; }
        effect.AffectActor(actor, data[2], data[3]);
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
                ChangeTerrainEffect(i, "");
            }
            else if (terrainWeatherInteractions.ReturnValue(t_w) == "Spread" || terrainTileInteractions.ReturnValue(t_t) == "Spread")
            {
                spreadingEffects.Add(i);
            }
            else if (terrainWeatherInteractions.ReturnValue(t_w) == "Expand" || terrainTileInteractions.ReturnValue(t_t) == "Expand")
            {
                expandingEffects.Add(i);
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
                    mapInfo[i] = blocks[1];
                    break;
                case "Feature":
                    terrainEffectTiles[i] = blocks[1];
                    break;
            }
        }
        UpdateMap();
    }
}
