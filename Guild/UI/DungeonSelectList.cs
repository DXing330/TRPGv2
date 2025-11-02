using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DungeonSelectList : MonoBehaviour
{
    public GeneralUtility utility;
    public PartyDataManager partyData;
    public List<GameObject> dungeonSelectObjects;
    public int page = 0;
    public StatDatabase dungeonData;
    public List<TMP_Text> difficultyTexts;
    public List<TMP_Text> questCountTexts;
    public List<TMP_Text> dungeonNames;
    public Dungeon dungeon;
    public SceneMover sceneMover;
}
