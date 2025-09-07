using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageStatDisplayManager : MonoBehaviour
{
    public GeneralUtility utility;
    public int currentPage = 0;
    public void ChangePage(bool right = true)
    {
        currentPage = utility.ChangePage(currentPage, right, actorDisplayObjects, actorNames);
        UpdateCurrentPageDisplay();
    }
    public GameObject displayObject;
    // Bar graph sort of thing.
    // Y-axis is scaling measure of damage dealt and taken.
    // X-axis is actor sprites and names.
    public SpriteContainer actorSprites;
    public List<GameObject> actorDisplayObjects;
    public List<DamageStatDisplay> actorDisplays;
    public TMP_Text winningTeam;
    public void SetWinningTeam(int winners)
    {
        winningTeam.text = "Winning Team: "+winners.ToString();
    }
    public List<string> actorNames;
    public List<string> actorSpriteNames;
    public List<int> damageDealt;
    public int maxDamageDealt;
    public float ReturnDamageDealtProportion(int damage)
    {
        return (float)damage / (float)maxDamageDealt;
    }
    public List<int> damageTaken;
    public int maxDamageTaken;
    public float ReturnDamageTakenProportion(int damage)
    {
        return (float)damage / (float)maxDamageTaken;
    }
    public void ResetStatDisplay()
    {
        currentPage = 0;
        actorNames = new List<string>();
        actorSpriteNames = new List<string>();
        damageDealt = new List<int>();
        damageTaken = new List<int>();
        maxDamageDealt = 0;
        maxDamageTaken = 0;
    }

    public void InitializeDisplay(BattleStatsTracker damageTracker)
    {
        displayObject.SetActive(true);
        ResetStatDisplay();
        SetWinningTeam(damageTracker.winningTeam);
        actorNames = damageTracker.GetActorNames();
        actorSpriteNames = damageTracker.GetActorSprites();
        damageDealt = damageTracker.GetDamageDealt();
        damageTaken = damageTracker.GetDamageTaken();
        UpdateCurrentPageDisplay();
    }

    public void UpdateCurrentPageDisplay()
    {
        utility.DisableGameObjects(actorDisplayObjects);
        maxDamageDealt = damageDealt.Max();
        maxDamageTaken = damageTaken.Max();
        List<int> currentIndices = utility.GetCurrentPageIndices(currentPage, actorDisplayObjects, actorNames);
        for (int i = 0; i < currentIndices.Count; i++)
        {
            int index = currentIndices[i];
            actorDisplayObjects[i].SetActive(true);
            actorDisplays[i].UpdateDisplay(actorSprites.SpriteDictionary(actorSpriteNames[index]), actorNames[index], damageDealt[index], damageTaken[index], ReturnDamageDealtProportion(damageDealt[index]), ReturnDamageTakenProportion(damageTaken[index]));
        }
    }

    public int testStatCount = 6;
    [ContextMenu("Test Display Damage")]
    public void TestDisplay()
    {
        ResetStatDisplay();
        // Generate random names and sprite names.
        for (int i = 0; i < testStatCount; i++)
        {
            string randomName = actorSprites.RandomSpriteName();
            actorNames.Add(randomName);
            actorSpriteNames.Add(randomName);
            damageDealt.Add(Random.Range(1, 100));
            damageTaken.Add(Random.Range(1, 100));
        }
        UpdateCurrentPageDisplay();
    }
}
