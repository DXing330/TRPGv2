using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OverworldUIManager : MonoBehaviour
{
    public PartyDataManager partyData;
    public OverworldState overworldState;
    public OverworldMap overworldMap;
    public int state = 0;
    public List<string> stateNames;
    public List<int> statePanelIndexes;
    public void ChangeState(int newState)
    {
        if (state == newState){return;}
        panels[state].SetActive(false);
        panels[newState].SetActive(true);
        state = newState;
        // Update the panel that was just activated.
    }
    public List<GameObject> panels;
    // Caravan stats.

    public TMP_Text remainingMovement;
    public GameObject moveSpeedBar;
    public GameObject cargoWeightBar;
    public float barYScale = 0.7f;
    public float barMinWidth = 0.1f;
    // Bar for movement.
    // Bar for current/max carry weight.
    public void UpdateMoveRange()
    {
        remainingMovement.text = overworldState.GetMoves().ToString();
    }
    public void UpdateMoveSpeed()
    {
        float moveSpeedRatio = partyData.caravan.ReturnPullCargoRatio();
        float adjRatio = moveSpeedRatio / partyData.caravan.GetMaxDistance();
        moveSpeedBar.transform.localScale = new Vector3(Mathf.Min(1,adjRatio),barYScale,0);
    }
    public void UpdateCargoWeight()
    {
        float cargoWeightRatio = partyData.caravan.ReturnCarryCargoRatio();
        if (cargoWeightRatio <= barMinWidth){cargoWeightRatio = barMinWidth;}
        cargoWeightBar.transform.localScale = new Vector3(Mathf.Min(1,cargoWeightRatio),barYScale,0);
    }
    public void UpdateCaravanPanel()
    {
        UpdateMoveRange();
        UpdateMoveSpeed();
        UpdateCargoWeight();
    }
    public void UpdateCurrentPanel()
    {
        switch (state)
        {
            case 0:
            return;
            case 1:
            UpdateCaravanPanel();
            break;
        }
    }
    // Party stats.
    // Quest stats.
}
