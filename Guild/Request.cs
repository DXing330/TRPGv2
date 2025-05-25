using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Request : MonoBehaviour
{
    // Might create some features depending on the request.
    public SavedOverworld overworld;
    public SavedCaravan caravan;
    public StatDatabase goals;
    public string delimiter = "|";
    public int difficulty;
    public int GetDifficulty(){ return difficulty; }
    public int reward;
    public int GetReward(){return reward;}
    public string goal;
    public string GetGoal(){return goal;}
    public string goalSpecifics;
    public string GetGoalSpecifics(){ return goalSpecifics; }
    // # days left before failure
    public int deadline;
    public int GetDeadline(){return deadline;}
    // 0 = false, 1 = true
    public int completed;
    public bool GetCompletion(){ return completed == 1; }
    public int location;
    public int GetLocation(){ return location; }
    // city/bandits/village/person/etc.
    public string locationSpecifics;

    public void Load(string requestDetails)
    {
        string[] data = requestDetails.Split(delimiter);
        difficulty = int.Parse(data[0]);
        reward = int.Parse(data[1]);
        goal = data[2];
        goalSpecifics = data[3];
        deadline = int.Parse(data[4]);
        completed = int.Parse(data[5]);
        location = int.Parse(data[6]);
        locationSpecifics = data[7];
    }

    public string ReturnDetails()
    {
        string details = difficulty+delimiter+reward+delimiter+goal+delimiter+goalSpecifics+delimiter+deadline+delimiter+completed+delimiter+location+delimiter+locationSpecifics+delimiter;
        return details;
    }
}
