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
    public SkillEffect effect;
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
    // List of actor names on tiles.
    public List<string> actorTiles;
    public bool TileNotEmpty(int tileNumber)
    {
        // If someone's name is on the tile then it's not empty.
        return (actorTiles[tileNumber].Length > 1);
    }
    // List of actor directions on tiles.
    public List<string> actorDirections;
    public List<string> terrainEffectTiles;
    public void ChangeTerrain(int tileNumber, string change)
    {
        // Straight forward change ignoring everything else.
        mapInfo[tileNumber] = change;
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
        if (emptyList.Count < mapSize * mapSize) { InitializeEmptyList(); }
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

    protected override void UpdateMap()
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

    public List<TacticActor> GetAdjacentAllies(TacticActor actor)
    {
        List<TacticActor> all = new List<TacticActor>();
        all = GetAdjacentActors(actor.GetLocation());
        for (int i = all.Count - 1; i >= 0; i--)
        {
            if (all[i].GetTeam() != actor.GetTeam()){all.RemoveAt(i);}
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
        effect.AffectActor(actor, data[2], data[3]);
    }

    public void NextRound()
    {
        // Apply weather/tile to terrain effects.
        string t_w = "";
        string t_t = "";
        for (int i = 0; i < terrainEffectTiles.Count; i++)
        {
            if (terrainEffectTiles[i] == ""){ continue; }
            t_w = terrainEffectTiles[i] + "-" + GetWeather();
            t_t = terrainEffectTiles[i] + "-" + mapInfo[i];
            if (terrainWeatherInteractions.ReturnValue(t_w) == "Remove" || terrainTileInteractions.ReturnValue(t_t) == "Remove")
            {
                ChangeTerrainEffect(i, "");
            }
        }
        // Apply weather effects to tile?
        UpdateMap();
    }
}
