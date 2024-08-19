using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticActor : MonoBehaviour
{
    public ActorStats allStats;
    public int GetMoveRange(int actionsLeft = -1)
    {
        // Default is two actions.
        if (actionsLeft < 0){actionsLeft = 2;}
        return allStats.GetMoveSpeed()*actionsLeft;
    }
    public string personalName;
    public void SetPersonalName(string newName){personalName = newName;}
    public string GetPersonalName(){return personalName;}
    public string spriteName;
    public void SetSpriteName(string newName){spriteName = newName;}
    public string GetSpriteName(){return spriteName;}
    public int location;
    public void SetLocation(int newLocation){location = newLocation;}
    public int GetLocation(){return location;}
}
