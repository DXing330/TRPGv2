using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add a battle manager so the map doesn't get too bloated, the map is just the visualization of the battle map and anything that will result in a change of that visualization, a lot of logic will be handled somewhere else.
public class BattleMap : MapManager
{
    public MapPatternLocations mapPatterns;
    public string weather;
    public TerrainPassivesList weatherPassives;
    public void ApplyWeatherStartEffect(TacticActor actor)
    {
        string[] data = weatherPassives.ReturnStartPassive(weather).Split("|");
        if (data.Length < 6){return;}
        if (passiveEffect.CheckStartEndConditions(actor, data[1], data[2], this))
        {
            passiveEffect.AffectActor(actor, data[4], data[5]);
        }
    }
    public void ApplyWeatherEndEffect(TacticActor actor)
    {
        string[] data = weatherPassives.ReturnEndPassive(weather).Split("|");
        if (data.Length < 6){return;}
        if (passiveEffect.CheckStartEndConditions(actor, data[1], data[2], this))
        {
            passiveEffect.AffectActor(actor, data[4], data[5]);
        }
    }
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
    public WeatherFilter timeDisplay;
    public void SetTime(string newInfo)
    { 
        time = newInfo;
        timeFilter.SetTime(time);
        timeDisplay.UpdateFilter(time);
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
    public void ApplyTileStartEffect(TacticActor actor)
    {
        string terrainType = mapInfo[actor.GetLocation()];
        string[] data = terrainPassives.ReturnStartPassive(terrainType).Split("|");
        if (data.Length < 6){return;}
        if (passiveEffect.CheckStartEndConditions(actor, data[1], data[2], this))
        {
            passiveEffect.AffectActor(actor, data[4], data[5]);
        }
    }
    public void ApplyTileEndEffect(TacticActor actor)
    {
        string terrainType = mapInfo[actor.GetLocation()];
        string[] data = terrainPassives.ReturnEndPassive(terrainType).Split("|");
        if (data.Length < 6){return;}
        if (passiveEffect.CheckStartEndConditions(actor, data[1], data[2], this))
        {
            passiveEffect.AffectActor(actor, data[4], data[5]);
        }
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
    public TerrainPassivesList terrainEffectData;
    public StatDatabase terrainWeatherInteractions;
    public StatDatabase terrainTileInteractions;
    public StatDatabase tileWeatherInteractions;
    public PassiveSkill passiveEffect;
    [ContextMenu("ForceStart")]
    public void ForceStart()
    {
        InitializeEmptyList();
        terrainEffectTiles = new List<string>(emptyList);
        interactables.Clear();
    }
    protected override void Start()
    {
        InitializeEmptyList();
        // Don't start again if you already force started.
        if (terrainEffectTiles.Count < emptyList.Count)
        {
            terrainEffectTiles = new List<string>(emptyList);
            interactables.Clear();
            //trappedTiles = new List<string>(emptyList);
        }
        //base.Start();
    }
    public BattleManager battleManager;
    public List<TacticActor> ReturnEndOfBattleActors()
    {
        List<TacticActor> endOfB = new List<TacticActor>();
        endOfB.AddRange(escapedActors);
        endOfB.AddRange(capturedActors);
        endOfB.AddRange(battlingActors);
        return endOfB;
    }
    public List<TacticActor> battlingActors;
    public bool EnemyExists(string spriteName)
    {
        for (int i = 0; i < battlingActors.Count; i++)
        {
            if (battlingActors[i].GetTeam() == 0){continue;}
            if (battlingActors[i].GetSpriteName() == spriteName){return true;}
        }
        return false;
    }
    public int AverageActorHealth()
    {
        int health = 0;
        int count = battlingActors.Count;
        for (int i = 0; i < count; i++)
        {
            health += battlingActors[i].GetHealth();
        }
        return health / count;
    }
    public List<TacticActor> defeatedActors;
    public List<TacticActor> capturedActors;
    public void CaptureActor(TacticActor actor)
    {
        int indexOf = battlingActors.IndexOf(actor);
        if (indexOf >= 0)
        {
            capturedActors.Add(actor);
            battlingActors.RemoveAt(indexOf);
        }
    }
    public bool CapturedActor(string spriteName, string actorName = "")
    {
        for (int i = 0; i < capturedActors.Count; i++)
        {
            if (capturedActors[i].GetSpriteName() == spriteName)
            {
                if (actorName == ""){return true;}
                else if (capturedActors[i].GetPersonalName() == actorName)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public List<TacticActor> escapedActors;
    public bool ActorCanEscape(TacticActor actor)
    {
        if (!mapUtility.BorderTile(actor.GetLocation(), mapSize)){return false;}
        if (GetAdjacentEnemies(actor).Count > 0){return false;}
        return true;
    }
    public void ActorEscapesBattle(TacticActor actor)
    {
        int indexOf = battlingActors.IndexOf(actor);
        if (indexOf >= 0)
        {
            // Deal with escaped actors differently at the end of battle.
            escapedActors.Add(actor);
            // Just play dead and leave the battle?
        }
    }
    public bool ActorEscaped(string spriteName, string actorName = "")
    {
        for (int i = 0; i < escapedActors.Count; i++)
        {
            if (escapedActors[i].GetSpriteName() == spriteName)
            {
                if (actorName == ""){return true;}
                else if (escapedActors[i].GetPersonalName() == actorName)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public List<int> GetAttackableTiles()
    {
        List<int> tiles = new List<int>();
        for (int i = 0; i < battlingActors.Count; i++)
        {
            tiles.Add(battlingActors[i].GetLocation());
        }
        for (int i = 0; i < interactables.Count; i++)
        {
            tiles.Add(interactables[i].GetLocation());
        }
        return tiles;
    }
    public TacticActor ReturnLatestActor()
    {
        return battlingActors[battlingActors.Count - 1];
    }
    public void AddActorToBattle(TacticActor actor)
    {
        battlingActors.Add(actor);
        UpdateMap();
    }
    public void MoveActorPassive(TacticActor actor, string effect, string specifics)
    {
        int direction = actor.GetDirection();
        switch (effect)
        {
            default:
            break;
            case "MoveForwardRandom":
            // Get the direction, either + (5, 0, 1) from your current direction;
            direction = (direction + UnityEngine.Random.Range(-1, 2) + 6) % 6;
            break;
            case "MoveBackwardRandom":
            // Get the direction, either + (2, 3, 4) from your current direction;
            direction = (direction + UnityEngine.Random.Range(2, 5)) % 6;
            break;
        }
        int rTile = mapUtility.PointInDirection(actor.GetLocation(), direction, mapSize);
        if (GetActorOnTile(rTile) == null)
        {
            actor.SetLocation(rTile);
            ApplyMovingTileEffect(actor, rTile);
            UpdateMap();
        }
    }
    public void SwitchActorLocations(TacticActor actor1, TacticActor actor2)
    {
        int temp = actor1.GetLocation();
        actor1.SetLocation(actor2.GetLocation());
        actor2.SetLocation(temp);
        UpdateMap();
    }
    public int GetClosestTeamMemberInRange(int start, int range, int team, List<int> except)
    {
        // Get all adjacent tiles in range.
        List<int> tilesInRange = mapUtility.GetTilesInCircleShape(start, range, mapSize);
        for (int i = 0; i < tilesInRange.Count; i++)
        {
            TacticActor tempActor = GetActorOnTile(tilesInRange[i]);
            if (tempActor != null && tempActor.GetTeam() == team && !except.Contains(tilesInRange[i]))
            {
                return tilesInRange[i];
            }
        }
        return -1;
    }
    public List<TacticActor> ChainLightningTargets(int start, int bounceRange = 2, int bounceCount = 3)
    {
        List<TacticActor> actors = new List<TacticActor>();
        List<int> chainTiles = new List<int>();
        TacticActor latestActor = GetActorOnTile(start);
        if (latestActor == null){return actors;}
        int team = latestActor.GetTeam();
        int lightningTile = latestActor.GetLocation();
        int nextLightningTile = -1;
        chainTiles.Add(lightningTile);
        for (int i = 0; i < bounceCount; i++)
        {
            nextLightningTile = GetClosestTeamMemberInRange(lightningTile, bounceRange, team, chainTiles);
            if (nextLightningTile < 0){break;}
            chainTiles.Add(nextLightningTile);
            lightningTile = nextLightningTile;
        }
        for (int i = 0; i < chainTiles.Count; i++)
        {
            actors.Add(GetActorOnTile(chainTiles[i]));
        }
        return actors;
    }
    // Should highlight all valid starting tiles to make this clear.
    public bool ValidStartingTile(string pattern, int tileNumber)
    {
        int startTileCount = mapSize * mapSize / 2 - (mapSize * mapSize / 2) % mapSize;
        List<int> startingTiles = mapPatterns.ReturnTilesOfPattern(pattern, startTileCount, mapSize);
        return startingTiles.Contains(tileNumber);
    }
    public List<string> excludedStartingTiles;
    public void RandomAllyStartingPositions(string pattern)
    {
        List<TacticActor> team = AllTeamMembers(0);
        for (int i = 0; i < team.Count; i++)
        {
            team[i].SetLocation(RandomStartingTile(pattern));
        }
        UpdateMap();
    }
    public void RandomEnemyStartingPositions(string pattern)
    {
        List<TacticActor> enemyTeam = AllTeamMembers(1);
        for (int i = 0; i < enemyTeam.Count; i++)
        {
            enemyTeam[i].SetLocation(RandomStartingTile(pattern));
        }
        UpdateMap();
    }
    protected int RandomStartingTile(string pattern)
    {
        int tile = UnityEngine.Random.Range(0, mapSize * mapSize);
        if (ValidRandomStartingTile(pattern, tile))
        {
            return tile;
        }
        return RandomStartingTile(pattern);
    }
    // Needs to be updated based on the spawning pattern.
    protected bool ValidRandomStartingTile(string pattern, int tileNumber)
    {
        int startTileCount = mapSize * mapSize / 2 - (mapSize * mapSize / 2) % mapSize;
        List<int> startingTiles = mapPatterns.ReturnTilesOfPattern(pattern, startTileCount, mapSize);
        if (startingTiles.Contains(tileNumber) && !excludedStartingTiles.Contains(mapInfo[tileNumber]) && !TileNotEmpty(tileNumber))
        {
            return true;
        }
        return false;
    }
    public void ChangeActorsLocation(int startingTile, int newTile)
    {
        TacticActor actor = GetActorOnTile(startingTile);
        TacticActor actor2 = GetActorOnTile(newTile);
        if (actor == null){return;}
        actor.SetLocation(newTile);
        if (actor2 == null){}
        else
        {
            actor2.SetLocation(startingTile);
        }
        UpdateMap();
    }
    public int RemoveActorsFromBattle(int turnNumber = -1)
    {
        int originalTurnNumber = turnNumber;
        for (int i = battlingActors.Count - 1; i >= 0; i--)
        {
            if (ActorEscaped(battlingActors[i].GetSpriteName()))
            {
                combatLog.UpdateNewestLog(battlingActors[i].GetPersonalName() + " escaped from the battle.");
                battlingActors.RemoveAt(i);
                // If someone whose turn already passed escapes, then the turn count needs to be decremented to avoid skipping someones turn.
                if (i <= originalTurnNumber) { turnNumber--; }
            }
            if (battlingActors[i].GetHealth() <= 0)
            {
                // Apply the death passives here.
                combatLog.UpdateNewestLog(battlingActors[i].GetPersonalName() + " is defeated.");
                battleManager.ActiveDeathPassives(battlingActors[i]);
                defeatedActors.Add(battlingActors[i]);
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
            if (adjacentActors[i].GetTeam() == team && adjacentActors[i].GetSpriteName().Contains(specificSprite))
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
    public bool FacingEmptyTile(TacticActor actor, bool forward = true)
    {
        int dir = actor.GetDirection();
        if (!forward)
        {
            dir = (dir + 3) % 6;
        }
        return !TileNotEmpty(mapUtility.PointInDirection(actor.GetLocation(), dir, mapSize));
    }
    public bool TargetFacingActor(TacticActor actor)
    {
        TacticActor target = actor.GetTarget();
        if (target == null){return false;}
        int direction = target.GetDirection();
        int directionBetween = mapUtility.DirectionBetweenLocations(target.GetLocation(), actor.GetLocation(), mapSize);
        if (direction == directionBetween || (direction + 1) % 6 == directionBetween || (direction + 5) % 6 == directionBetween)
        {
            return true;
        }
        return false;
    }
    public bool FacingActor(TacticActor actor)
    {
        int startingPoint = actor.GetLocation();
        int direction = actor.GetDirection();
        for (int i = 0; i < actor.GetAttackRange(); i++)
        {
            if (TileNotEmpty(mapUtility.PointInDirection(startingPoint, direction, mapSize)))
            {
                return true;
            }
            startingPoint = mapUtility.PointInDirection(startingPoint, direction, mapSize);
        }
        return false;
    }
    public TacticActor ReturnClosestFacingActor(TacticActor actor)
    {
        int startingPoint = actor.GetLocation();
        int direction = actor.GetDirection();
        for (int i = 0; i < actor.GetAttackRange(); i++)
        {
            if (TileNotEmpty(mapUtility.PointInDirection(startingPoint, direction, mapSize)))
            {
                return GetActorOnTile(mapUtility.PointInDirection(startingPoint, direction, mapSize));
            }
            startingPoint = mapUtility.PointInDirection(startingPoint, direction, mapSize);
        }
        return null;
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
    public void ChangeTile(int tileNumber, string effect, string specifics, bool force = false)
    {
        switch (effect)
        {
            case "Tile":
                ChangeTerrain(tileNumber, specifics, force);
                break;
            case "TerrainEffect":
                ChangeTEffect(tileNumber, specifics, force);
                break;
            case "Spread":
                SpreadTerrainEffect(tileNumber, specifics);
                break;
            case "RSpread":
                RandomlySpreadTerrainEffect(tileNumber, specifics);
                break;
            case "ChainSpread":
                ChainSpreadTerrainEffect(tileNumber, specifics);
                break;
            case "ChainReplaceTEffect":
                ChainReplaceTEffects(tileNumber, specifics);
                break;
            case "SwitchBetween":
                string[] between = specifics.Split("=");
                if (between.Length < 2){return;}
                if (mapInfo[tileNumber] == between[0])
                {
                    ChangeTerrain(tileNumber, between[1], true);
                }
                else if (mapInfo[tileNumber] == between[1])
                {
                    ChangeTerrain(tileNumber, between[0], true);
                }
                break;
            case "RandomTileSwap":
                // Switch tiles with a random adjacent tile.
                List<int> adjacent = mapUtility.AdjacentTiles(tileNumber, mapSize);
                if (adjacent.Count <= 1){return;}
                int random = adjacent[UnityEngine.Random.Range(0, adjacent.Count)];
                SwitchTile(tileNumber, random);
                break;
        }
        UpdateMap();
    }
    protected void NewTileElevation(int tileNumber)
    {
        if (tileNumber < 0 || tileNumber >= mapElevations.Count){return;}
        mapElevations[tileNumber] = RandomElevation(mapInfo[tileNumber]);
        mapTiles[tileNumber].SetElevation(mapElevations[tileNumber]);
        mapTiles[tileNumber].UpdateElevationSprite(elevationSprites.SpriteDictionary("E"+mapTiles[tileNumber].GetElevation().ToString()));
        battleManager.moveManager.SetMapElevations(mapElevations);
        UpdateMap();
    }
    // TESTED IN BATTLEMAPTESTER
    public void ChainReplaceTEffects(int tileNumber, string change)
    {
        List<int> connected = AllConnectedTerrain(tileNumber);
        for (int i = 0; i < connected.Count; i++)
        {
            ChangeTEffect(connected[i], change, true);
        }
    }
    public void ChangeTerrain(int tileNumber, string change, bool force = false)
    {
        if (mapInfo[tileNumber] == change){return;}
        if (force)
        {
            mapInfo[tileNumber] = change;
            NewTileElevation(tileNumber);
            return;
        }
        string t_t = mapInfo[tileNumber] + "-" + change;
        // TESTED IN BATTLEMAPTESTER.
        string newInfo = tileTileInteractions.ReturnValue(t_t);
        if (newInfo == "")
        {
            mapInfo[tileNumber] = change;
        }
        else
        {
            mapInfo[tileNumber] = newInfo;
        }
        // Update the elevation.
        NewTileElevation(tileNumber);
    }
    public StatDatabase terrainTerrainInteractions;
    public List<string> terrainEffectTiles;
    public virtual void GetNewTerrainEffects(List<string> featuresAndPatterns)
    {
        terrainEffectTiles = new List<string>(emptyList);
        for (int i = 0; i < featuresAndPatterns.Count; i++)
        {
            string[] fPSplit = featuresAndPatterns[i].Split("=");
            if (fPSplit.Length < 2){continue;}
            terrainEffectTiles = mapMaker.AddFeature(terrainEffectTiles, fPSplit[0], fPSplit[1]);
        }
        UpdateMap();
    }
    public void ChangeTEffect(int tileNumber, string newEffect, bool force = false)
    {
        if (terrainEffectTiles[tileNumber] == newEffect){return;}
        if (force)
        {
            terrainEffectTiles[tileNumber] = newEffect;
            return;
        }
        if (terrainEffectTiles[tileNumber] == "" || newEffect == "")
        {
            terrainEffectTiles[tileNumber] = newEffect;
        }
        else
        {
            string t_t = terrainEffectTiles[tileNumber] + "-" + newEffect;
            string newValue = terrainTerrainInteractions.ReturnValue(t_t);
            if (newValue == "")
            {
                terrainEffectTiles[tileNumber] = newEffect;
            }
            else if (newValue.Contains("ChainReplace"))
            {
                string[] cRS = newValue.Split("=");
                if (cRS.Length < 2)
                {
                    ChainReplaceTEffects(tileNumber, newEffect);
                }
                else
                {
                    Debug.Log(cRS[1]);
                    ChainReplaceTEffects(tileNumber, cRS[1]);
                }
                return;
            }
            else
            {
                terrainEffectTiles[tileNumber] = newValue;
            }
        }
        UpdateMap();
    }
    public void SwitchTerrainEffect(int tile1, int tile2)
    {
        string temp = terrainEffectTiles[tile1];
        terrainEffectTiles[tile1] = terrainEffectTiles[tile2];
        terrainEffectTiles[tile2] = temp;
        UpdateMap();
    }
    public void SpreadTerrainEffect(int tileNumber, string sTEffect = "")
    {
        string tEffect = terrainEffectTiles[tileNumber];
        if (tEffect == ""){return;}
        if (sTEffect != "" && tEffect != sTEffect){return;}
        List<int> adjacent = mapUtility.AdjacentTiles(tileNumber, mapSize);
        for (int i = 0; i < adjacent.Count; i++)
        {
            ChangeTEffect(adjacent[i], tEffect);
        }
    }
    public void RandomlySpreadTerrainEffect(int tileNumber, string sTEffect = "")
    {
        string tEffect = terrainEffectTiles[tileNumber];
        if (tEffect == ""){return;}
        if (sTEffect != "" && tEffect != sTEffect){return;}
        List<int> adjacent = mapUtility.AdjacentTiles(tileNumber, mapSize);
        ChangeTEffect(adjacent[UnityEngine.Random.Range(0, adjacent.Count)], tEffect);
    }
    public List<int> AllConnectedTiles(int tileNumber)
    {
        List<int> connected = new List<int>();
        string tile = mapInfo[tileNumber];
        if (tile == "")
        {
            return connected;
        }
        List<int> viewedTiles = new List<int>();
        List<int> queuedTiles = new List<int>();
        viewedTiles.Add(tileNumber);
        connected.Add(tileNumber);
        queuedTiles.Add(tileNumber);
        while (queuedTiles.Count > 0)
        {
            int currentTile = queuedTiles[0];
            List<int> adjacentTiles = mapUtility.AdjacentTiles(currentTile, mapSize);
            for (int i = 0; i < adjacentTiles.Count; i++)
            {
                // Only look at new tiles.
                if (viewedTiles.Contains(adjacentTiles[i])){continue;}
                viewedTiles.Add(adjacentTiles[i]);
                if (mapInfo[adjacentTiles[i]] == tile)
                {
                    connected.Add(adjacentTiles[i]);
                    queuedTiles.Add(adjacentTiles[i]);
                }
            }
            queuedTiles.RemoveAt(0);
        }
        return connected;
    }
    public List<int> AllConnectedTerrain(int tileNumber)
    {
        List<int> connected = new List<int>();
        string tEffect = terrainEffectTiles[tileNumber];
        if (tEffect == "")
        {
            return connected;
        }
        List<int> viewedTiles = new List<int>();
        List<int> queuedTiles = new List<int>();
        viewedTiles.Add(tileNumber);
        connected.Add(tileNumber);
        queuedTiles.Add(tileNumber);
        while (queuedTiles.Count > 0)
        {
            int currentTile = queuedTiles[0];
            List<int> adjacentTiles = mapUtility.AdjacentTiles(currentTile, mapSize);
            for (int i = 0; i < adjacentTiles.Count; i++)
            {
                // Only look at new tiles.
                if (viewedTiles.Contains(adjacentTiles[i])){continue;}
                viewedTiles.Add(adjacentTiles[i]);
                if (terrainEffectTiles[adjacentTiles[i]] == tEffect)
                {
                    connected.Add(adjacentTiles[i]);
                    queuedTiles.Add(adjacentTiles[i]);
                }
            }
            queuedTiles.RemoveAt(0);
        }
        return connected;
    }
    public void ChainSpreadTerrainEffect(int tileNumber, string sTEffect = "")
    {
        string tEffect = terrainEffectTiles[tileNumber];
        if (tEffect != sTEffect){return;}
        // Get all connected tiles of the same teffect.
        List<int> connected = AllConnectedTerrain(tileNumber);
        if (connected.Count <= 0){return;}
        // Get all borders to the connected tiles.
        List<int> borders = mapUtility.AdjacentBorders(connected, mapSize);
        // Try to spread to all the border tiles.
        for (int i = 0; i < borders.Count; i++)
        {
            ChangeTEffect(borders[i], sTEffect);
        }
    }
    protected void UpdateTerrain()
    {
        mapDisplayers[2].DisplayCurrentTiles(mapTiles, terrainEffectTiles, currentTiles);
    }
    public List<Interactable> interactables;
    public void RemoveInteractable(Interactable iAct)
    {
        interactables.Remove(iAct);
    }
    public void AddInteractable(Interactable iAct)
    {
        interactables.Add(iAct);
    }
    public Interactable GetInteractableOnTile(int tileNumber)
    {
        if (GetActorOnTile(tileNumber) != null){return null;}
        for (int i = 0; i < interactables.Count; i++)
        {
            if (interactables[i].GetLocation() == tileNumber)
            {
                return interactables[i];
            }
        }
        return null;
    }
    public List<Interactable> GetInteractablesOnTile(int tileNumber)
    {
        List<Interactable> inters = new List<Interactable>();
        for (int i = 0; i < interactables.Count; i++)
        {
            if (interactables[i].GetLocation() == tileNumber)
            {
                inters.Add(interactables[i]);
            }
        }
        return inters;
    }
    public void AttackInteractable(int tileNumber, TacticActor attacker)
    {
        List<Interactable> interactables = GetInteractablesOnTile(tileNumber);
        if (interactables.Count <= 0){return;}
        for (int i = 0; i < interactables.Count; i++)
        {
            interactables[i].AttackTrigger(this, attacker);
        }
    }
    public void EndTurnOnInteractable(TacticActor actor)
    {
        List<Interactable> interactables = GetInteractablesOnTile(actor.GetLocation());
        if (interactables.Count <= 0){return;}
        for (int i = 0; i < interactables.Count; i++)
        {
            interactables[i].EndTrigger(this, actor);
        }
    }
    public bool InteractableOnTile(int tileNumber)
    {
        for (int i = 0; i < interactables.Count; i++)
        {
            if (interactables[i].GetLocation() == tileNumber)
            {
                return true;
            }
        }
        return false;
    }
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
        for (int i = 0; i < interactables.Count; i++)
        {
            actorTiles[interactables[i].GetLocation()] = interactables[i].GetSpriteName();
            actorDirections[interactables[i].GetLocation()] = "";
        }
        for (int i = 0; i < battlingActors.Count; i++)
        {
            // Just for fun make the actor not appear on the map.
            // They can still use movement to determine the actors location so it doesn't really do anything if you're paying the slightest amount of attention.
            if (battlingActors[i].invisible){continue;}
            actorTiles[battlingActors[i].GetLocation()] = battlingActors[i].GetSpriteName();
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
    }

    public void UpdateMovingPath(TacticActor actor, MoveCostManager moveManager, int selectedTile)
    {
        List<string> originalHighlightedTiles = new List<string>(highlightedTiles);
        List<int> path = moveManager.GetPrecomputedPath(actor.GetLocation(), selectedTile);
        UpdateHighlightsWithoutReseting(path, "Red");
        highlightedTiles = new List<string>(originalHighlightedTiles);
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

    protected void HighlightTileWithoutReseting(int newTile, string colorKey, int layer = 3)
    {
        string colorName = colorDictionary.GetColorNameByKey(colorKey);
        highlightedTiles[newTile] = colorName;
        mapDisplayers[layer].HighlightCurrentTiles(mapTiles,highlightedTiles, currentTiles);
    }

    public void UpdateStartingPositionTiles(string pattern, int selectedTile = -1)
    {
        int startTileCount = mapSize * mapSize / 2 - (mapSize * mapSize / 2) % mapSize;
        List<int> startingTiles = mapPatterns.ReturnTilesOfPattern(pattern, startTileCount, mapSize);
        UpdateHighlights(startingTiles, "Green", 4);
        if (selectedTile >= 0)
        {
            HighlightTileWithoutReseting(selectedTile, "Blue", 4);
        }
    }

    public void UpdateHighlights(List<int> newTiles, string colorKey = "MoveClose", int layer = 3)
    {
        string colorName = colorDictionary.GetColorNameByKey(colorKey);
        if (emptyList.Count < mapSize * mapSize) { InitializeEmptyList(); }
        highlightedTiles = new List<string>(emptyList);
        for (int i = 0; i < newTiles.Count; i++)
        {
            if (newTiles[i] < 0){continue;}
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

    public bool TileTypeExists(string tileType)
    {
        return mapInfo.Contains(tileType);
    }

    public int ReturnClosestTileOfType(TacticActor actor, string tileType)
    {
        int tile = -1;
        int distance = mapSize * mapSize;
        for (int i = 0; i < mapInfo.Count; i++)
        {
            if (actor.GetMoveType() != "Flying" && excludedTileTypesForNonFlying.Contains(mapInfo[i]))
            {
                continue;
            }
            if (mapInfo[i].Contains(tileType) && GetActorOnTile(i) == null)
            {
                int newDistance = mapUtility.DistanceBetweenTiles(i, actor.GetLocation(), mapSize);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    tile = i;
                }
            }
        }
        return tile;
    }

    // Override this to ignore tiles that have actors on them and then give the next closest tile.
    public override int ReturnClosestTileWithinElevationDifference(int start, int end, int maxElvDiff)
    {
        List<int> adjacentTiles = mapUtility.AdjacentTiles(end, mapSize);
        int target = end;
        int dist = mapSize * mapSize;
        for (int i = 0; i < adjacentTiles.Count; i++)
        {
            if (ReturnElevationDifference(adjacentTiles[i], end) > maxElvDiff || GetActorOnTile(adjacentTiles[i]) != null)
            {
                continue;
            }
            if (mapUtility.DistanceBetweenTiles(adjacentTiles[i], start, mapSize) < dist)
            {
                dist = mapUtility.DistanceBetweenTiles(adjacentTiles[i], start, mapSize);
                target = adjacentTiles[i];
            }
        }
        return target;
    }

    public List<string> excludedTileTypesForNonFlying;
    public bool TileExcluded(TacticActor actor, string tile)
    {
        if (actor.GetMoveType() == "Flying"){return false;}
        return excludedTileTypesForNonFlying.Contains(tile);
    }

    public bool TargetSandwiched(TacticActor actor, string tileType)
    {
        if (actor.GetTarget() == null){return false;}
        int location = actor.GetLocation();
        int targetLoc = actor.GetTarget().GetLocation();
        if (!mapUtility.TilesAdjacent(location, targetLoc, mapSize)){return false;}
        int direction = mapUtility.DirectionBetweenLocations(location, targetLoc, mapSize);
        int sandwichingPoint = mapUtility.PointInDirection(targetLoc, direction, mapSize);
        if (sandwichingPoint < 0){return false;}
        return mapInfo[sandwichingPoint].Contains(tileType);
    }

    public bool TargetSandwichable(TacticActor actor, string tileType)
    {
        return ReturnClosestSandwichTargetBetweenTileOfType(actor, tileType) >= 0;
    }

    public int ReturnClosestSandwichTargetBetweenTileOfType(TacticActor actor, string tileType)
    {
        int tile = -1;
        int distance = mapSize * mapSize;
        if (actor.GetTarget() == null){return tile;}
        int targetLocation = actor.GetTarget().GetLocation();
        List<int> adjacentTiles = mapUtility.AdjacentTiles(targetLocation, mapSize);
        for (int i = 0; i < adjacentTiles.Count; i++)
        {
            // Check if the target is adjacent to any of the requested tile types.
            if (mapInfo[adjacentTiles[i]].Contains(tileType))
            {
                // Check the opposite tile and make sure it's empty and valid.
                int sandwichingPoint = mapUtility.PointInOppositeDirection(targetLocation, adjacentTiles[i], mapSize);
                // Can't be out of bounds, excluded or have an actor on it.
                if (sandwichingPoint < 0 || TileExcluded(actor, mapInfo[sandwichingPoint]) || GetActorOnTile(sandwichingPoint) != null)
                {
                    continue;
                }
                int newDistance = mapUtility.DistanceBetweenTiles(sandwichingPoint, actor.GetLocation(), mapSize);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    tile = sandwichingPoint;
                }
            }
        }
        return tile;
    }

    public string GetTileEffectOfActor(TacticActor actor)
    {
        return terrainEffectTiles[actor.GetLocation()];
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

    public bool TileInAttackRange(TacticActor actor, int tileIndex)
    {
        List<int> attackable = GetAttackableTiles(actor);
        return attackable.Contains(tileIndex);
    }

    public List<int> GetAttackableTiles(TacticActor actor)
    {
        int range = actor.GetAttackRange();
        int startTile = actor.GetLocation();
        List<int> attackable = mapUtility.GetTilesInCircleShape(startTile, range, mapSize);
        attackable.Remove(startTile);
        // Melee attacks can't attack if the height difference is too large.
        // Skills still work?
        if (range <= 1)
        {
            for (int i = attackable.Count - 1; i >= 0; i--)
            {
                // Elevation difference of 3 or more means melee basic attacks are ineffective.
                if (ReturnElevationDifference(attackable[i], startTile) > actor.GetWeaponReach())
                {
                    attackable.RemoveAt(i);
                }
            }
        }
        return attackable;
    }

    public void UpdateSelectedAttackTile(TacticActor actor, int selectedTile)
    {
        List<int> attackable = GetAttackableTiles(actor);
        UpdateHighlights(attackable, "Attack");
        if (!attackable.Contains(selectedTile)){return;}
        List<int> selected = new List<int>();
        selected.Add(selectedTile);
        UpdateHighlightsWithoutReseting(selected, "Green");
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

    public List<int> GetShootableTiles(TacticActor actor, int shootingRange = 2)
    {
        List<int> attackable = mapUtility.GetTilesInCircleShape(actor.GetLocation(), shootingRange, mapSize);
        attackable.Remove(actor.GetLocation());
        return attackable;
    }

    public bool ShootableEnemies(TacticActor actor, int shootingRange = 2)
    {
        List<TacticActor> attackableEnemies = GetActorsOnTiles(GetShootableTiles(actor, shootingRange));
        for (int i = attackableEnemies.Count - 1; i >= 0; i--)
        {
            if (attackableEnemies[i].GetTeam() == actor.GetTeam())
            {
                attackableEnemies.RemoveAt(i);
            }
        }
        return attackableEnemies.Count > 0;
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
        return enemies[UnityEngine.Random.Range(0, enemies.Count)].GetLocation();
    }

    public int DistanceBetweenActors(TacticActor actor1, TacticActor actor2)
    {
        if (actor1 == null || actor2 == null)
        {
            return mapSize * mapSize + 1;
        }
        return mapUtility.DistanceBetweenTiles(actor1.GetLocation(), actor2.GetLocation(), mapSize);
    }

    public bool StraightLineBetweenActors(TacticActor actor1, TacticActor actor2)
    {
        if (actor1 == null || actor2 == null)
        {
            return false;
        }
        return mapUtility.StraightLineBetweenPoints(actor1.GetLocation(), actor2.GetLocation(), mapSize);
    }

    public TacticActor GetClosestEnemy(TacticActor actor)
    {
        List<TacticActor> enemies = AllEnemies(actor);
        int index = -1;
        int distance = mapSize * 2;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (DistanceBetweenActors(actor, enemies[i]) < distance)
            {
                distance = DistanceBetweenActors(actor, enemies[i]);
                index = i;
            }
        }
        return enemies[index];
    }

    public int GetClosestEmptyTile(TacticActor actor)
    {
        int location = actor.GetLocation();
        if (actor.GetHealth() < 0){return location;}
        List<int> adjacentEmpty = GetAdjacentEmptyTiles(location);
        if (adjacentEmpty.Count > 0)
        {
            return adjacentEmpty[UnityEngine.Random.Range(0, adjacentEmpty.Count)];
        }
        int distance = mapSize;
        int tile = -1;
        for (int i = 0; i < mapSize * mapSize; i++)
        {
            if (!TileNotEmpty(i))
            {
                int newDist = mapUtility.DistanceBetweenTiles(i, location, mapSize);
                if (newDist < distance)
                {
                    distance = newDist;
                    tile = i;
                }
            }
        }
        return tile;
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
        int tile = emptyAdjacent[UnityEngine.Random.Range(0, emptyAdjacent.Count)];
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

    public void MoveActor(TacticActor actor, string effect, string specifics)
    {
        int tile = -1;
        int start = actor.GetLocation();
        switch (effect)
        {
            default:
            break;
            case "DirMove":
            tile = mapUtility.PointInDirection(start, int.Parse(specifics), mapSize);
            if (tile == start || tile < 0){return;}
            if (GetActorOnTile(tile) != null){return;}
            actor.SetLocation(tile);
            ApplyMovingTileEffect(actor, tile);
            break;
        }
        UpdateMap();
    }

    // All moving must pass through here, thus update the combat log here.
    public bool ApplyMovingTileEffect(TacticActor actor, int tileNumber)
    {
        combatLog.UpdateNewestLog(actor.GetPersonalName() + " moves to " + mapUtility.GetRowColumnCoordinateString(tileNumber, mapSize));
        ApplyTileMovingEffect(actor, tileNumber);
        ApplyTerrainMovingEffect(actor, tileNumber);
        ApplyInteractableEffect(actor, tileNumber);
        return false;
    }

    protected void ApplyTileMovingEffect(TacticActor actor, int tileNumber)
    {
        string tileEffect = ReturnTileMovingPassive(actor);
        if (tileEffect.Length < 1) { return; }
        List<string> data = tileEffect.Split("|").ToList();
        if (passiveEffect.CheckStartEndConditions(actor, data[1], data[2], this))
        {
            passiveEffect.AffectActor(actor, data[4], data[5]);
        }
    }

    protected void ApplyTerrainMovingEffect(TacticActor actor, int tileNumber)
    {
        // Get the terrain info.
        string terrainEffect = terrainEffectTiles[tileNumber];
        if (terrainEffect.Length < 1) { return; }
        // Apply the terrain effect.
        string[] tMovingEffect = terrainEffectData.ReturnMovingPassive(terrainEffect).Split("|");
        if (tMovingEffect.Length < 6){return;}
        if (passiveEffect.CheckStartEndConditions(actor, tMovingEffect[1], tMovingEffect[2], this))
        {
            passiveEffect.AffectActor(actor, tMovingEffect[4], tMovingEffect[5]);
        }
    }

    protected void ApplyInteractableEffect(TacticActor actor, int tileNumber)
    {
        List<Interactable> triggered = GetInteractablesOnTile(tileNumber);
        if (triggered.Count <= 0){return;}
        for (int i = 0; i < triggered.Count; i++)
        {
            triggered[i].MoveTrigger(this, actor);
        }
    }

    public void ApplyTerrainStartEffect(TacticActor actor)
    {
        string terrainEffect = terrainEffectTiles[actor.GetLocation()];
        if (terrainEffect.Length < 1) { return; }
        string[] tMovingEffect = terrainEffectData.ReturnStartPassive(terrainEffect).Split("|");
        if (tMovingEffect.Length < 6){return;}
        if (passiveEffect.CheckStartEndConditions(actor, tMovingEffect[1], tMovingEffect[2], this))
        {
            passiveEffect.AffectActor(actor, tMovingEffect[4], tMovingEffect[5]);
        }
    }

    public void ApplyEndTerrainEffect(TacticActor actor)
    {
        string terrainEffect = terrainEffectTiles[actor.GetLocation()];
        if (terrainEffect.Length < 1) { return; }
        string[] tMovingEffect = terrainEffectData.ReturnEndPassive(terrainEffect).Split("|");
        if (tMovingEffect.Length < 6){return;}
        if (passiveEffect.CheckStartEndConditions(actor, tMovingEffect[1], tMovingEffect[2], this))
        {
            passiveEffect.AffectActor(actor, tMovingEffect[4], tMovingEffect[5]);
        }
    }

    public void NextRound()
    {
        // Apply weather/tile to terrain effects.
        string t_w = "";
        string t_t = "";
        List<int> spreadingEffects = new List<int>();
        List<int> expandingEffects = new List<int>();
        List<int> removedEffects = new List<int>();
        for (int i = 0; i < terrainEffectTiles.Count; i++)
        {
            if (terrainEffectTiles[i] == ""){ continue; }
            t_w = terrainEffectTiles[i] + "-" + GetWeather();
            t_t = mapInfo[i] + "-" + terrainEffectTiles[i];
            if (terrainWeatherInteractions.ReturnValue(t_w) == "Remove" || terrainTileInteractions.ReturnValue(t_t) == "Remove")
            {
                removedEffects.Add(i);
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
        for (int i = 0; i < removedEffects.Count; i++)
        {
            ChangeTEffect(removedEffects[i], "");
        }
        IncrementDelayedEffects();
        UpdateMap();
    }
}
