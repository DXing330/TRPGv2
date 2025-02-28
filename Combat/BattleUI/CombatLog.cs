using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatLog : MonoBehaviour
{
    void Start()
    {
        combatRoundTracker.Clear();
        combatTurnTracker.Clear();
        allLogs.Clear();
    }
    public BattleManager battleManager;
    public GeneralUtility utility;
    // Change rounds.
    public int round;
    public void ChangeRound(bool increase = true)
    {
        turn = 0;
        int maxRound = battleManager.GetRoundNumber();
        if (increase)
        {
            if (maxRound > round){round++;}
            else {round = 0;}
        }
        else
        {
            if (round > 0){round--;}
            else {round = maxRound;}
        }
        UpdateLog();
    }
    public int turn;
    public int DetermineTurnsInRound()
    {
        int maxTurn = 1;
        for (int i = 0; i < combatRoundTracker.Count; i++)
        {
            if (combatRoundTracker[i] == round)
            {
                if (combatTurnTracker[i] > maxTurn){maxTurn = combatTurnTracker[i];}
            }
        }
        return maxTurn;
    }
    public void ChangeTurn(bool increase = true)
    {
        // Need to determine the max number of turns in the current round.
        int maxTurn = DetermineTurnsInRound();
        if (increase)
        {
            if (maxTurn > turn){turn++;}
            // If you go too far then just go to the next turn.
            else {ChangeRound(increase);}
        }
        else
        {
            if (turn > 0){turn--;}
            else
            {
                ChangeRound(increase);
                // If you go down in rounds then you need to start at the max turn.
                maxTurn = DetermineTurnsInRound();
                turn = maxTurn;
            }
        }
        UpdateLog();
    }
    public List<int> combatRoundTracker;
    public List<int> combatTurnTracker;
    public List<string> allLogs;
    public void AddNewLog()
    {
        round = battleManager.GetRoundNumber();
        turn = battleManager.GetTurnIndex();
        combatRoundTracker.Add(round);
        combatTurnTracker.Add(turn);
        allLogs.Add("");
    }
    public void UpdateNewestLog(string newText)
    {
        if (allLogs[allLogs.Count - 1] == "")
        {
            allLogs[allLogs.Count - 1] = newText;
        }
        else
        {
            allLogs[allLogs.Count - 1] = allLogs[allLogs.Count - 1]+"\n"+newText;
        }
        if (round == battleManager.GetRoundNumber() && turn == battleManager.GetTurnIndex()){UpdateLog();}
    }
    public TMP_Text roundTrackerText;
    public TMP_Text turnTrackerText;
    public TMP_Text eventLog;
    public void UpdateLog()
    {
        for (int i = 0; i < combatRoundTracker.Count; i++)
        {
            if (combatRoundTracker[i] == round && combatTurnTracker[i] == turn)
            {
                roundTrackerText.text = "Round "+round;
                turnTrackerText.text = "Turn "+(turn+1);
                eventLog.text = allLogs[i];
            }
        }
    }
    protected void UpdateNewestLog()
    {
        roundTrackerText.text = "Round "+round;
        turnTrackerText.text = "Turn "+(turn+1);
        eventLog.text = allLogs[allLogs.Count - 1];
    }
}
