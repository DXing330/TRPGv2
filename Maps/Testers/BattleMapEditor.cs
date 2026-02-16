using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleMapEditor : MonoBehaviour
{
    void Start()
    {
        actorSelect.SetData(actorStats.GetAllKeys(), actorStats.GetAllKeys(), actorStats.GetAllValues());
        LoadBattle("TestBattle0");
    }
    public string battleID;
    public void SetBattleID(string newID)
    {
        battleID = newID;
        battleIDText.text = battleID;
    }
    public TMP_Text battleIDText;
    public BattleMapEditorSaver savedBattles;
    public void SaveBattle()
    {
        savedBattles.SaveBattle(this, battleID);
    }
    public void LoadBattle(string newID)
    {
        savedBattles.LoadBattle(this, newID);
        SetBattleID(newID);
    }
    public void DeleteBattle()
    {
        savedBattles.DeleteKey(battleID);
        LoadBattle("TestBattle0");
    }
    public GameObject nRObject;
    public NameRater nameRater;
    enum NameRaterState
    {
        copying,
        newing
    }
    // Battle Name Rater State.
    NameRaterState bNRS;
    public void NameRaterConfirm()
    {
        string newName = nameRater.ConfirmName();
        if (newName.Length <= 0){return;}
        switch (bNRS)
        {
            case NameRaterState.newing:
            // Make New Map.
            LoadBattle(newName);
            break;
            case NameRaterState.copying:
            // Copy To New Map.
            SetBattleID(newName);
            SaveBattle();
            break;
        }
    }
    public void NewBattle()
    {
        bNRS = NameRaterState.newing;
        nRObject.SetActive(true);
    }
    public void CopyBattle()
    {
        bNRS = NameRaterState.copying;
        nRObject.SetActive(true);
    }
    public SelectList battleSelectList;
    public void TryToLoadBattle()
    {
        battleSelectList.SetSelectables(savedBattles.savedKeys);
    }
    public void SelectLoadBattle()
    {
        if (battleSelectList.GetSelected() < 0){return;}
        LoadBattle(battleSelectList.GetSelectedString());
    }
    public MapEditorSaver savedMaps;
    public MapEditor mapEditor;
    public StatDatabase actorStats;
    public ActorSpriteHPList actorSelect;
    public void ResetSelectedEnemy()
    {
        selectedEnemy = "";
    }
    public void SelectEnemy()
    {
        if (actorSelect.GetSelected() < 0)
        {
            ResetSelectedEnemy();
            return;
        }
        selectedEnemy = actorSelect.GetSelectedName();
        //View stats/skills/passives when selecting actors.
    }
    public string selectedEnemy;
    public NameRater filter;
    public void ResetFilter()
    {
        filter.ResetNewName();
        actorSelect.SetData(actorStats.GetAllKeys(), actorStats.GetAllKeys(), actorStats.GetAllValues());
    }
    public void FilterActorSelect()
    {
        actorSelect.ResetSelected();
        List<string> filters = new List<string>();
        filters.Add(filter.ReturnNameWithFirstCharUpperCase());
        filters.Add(filter.ConfirmName().ToLower());
        actorSelect.SetData(actorStats.GetFilteredKeys(filters), actorStats.GetFilteredKeys(filters), actorStats.GetFilteredValues(filters));
    }
    public void InitializeNewMap()
    {
        mapEditor.InitializeNewMap();
        enemies.Clear();
        enemyLocations.Clear();
    }
    public List<string> enemies;
    public List<string> enemyLocations;
    public void ResetEnemies()
    {
        enemies.Clear();
        enemyLocations.Clear();
        UpdateMap();
    }
    public void AddEnemy()
    {
        if (selectedTile < 0 || selectedEnemy == ""){return;}
        if (enemyLocations.Contains(selectedTile.ToString())){return;}
        enemies.Add(selectedEnemy);
        enemyLocations.Add(selectedTile.ToString());
        UpdateMap();
    }
    public void RemoveEnemy()
    {
        if (selectedTile < 0){return;}
        for (int i = 0; i < enemyLocations.Count; i++)
        {
            if (int.Parse(enemyLocations[i]) == selectedTile)
            {
                enemyLocations.RemoveAt(i);
                enemies.RemoveAt(i);
            }
        }
        UpdateMap();
    }
    public int selectedTile = -1;
    public void ClickOnTile(int tileNumber)
    {
        selectedTile = tileNumber;
        mapEditor.HighlightTile(selectedTile);
    }
    public void UpdateMap()
    {
        mapEditor.UpdateMapWithActors(enemies, enemyLocations);   
    }
    // Enemy Equipment?
    // Enemy Buffs?
}
