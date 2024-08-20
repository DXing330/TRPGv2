using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticActor : MonoBehaviour
{
    public GameObject actorObject;
    public void DestroyActor(){DestroyImmediate(actorObject);}
    public ActorStats allStats;
    public int team;
    public int GetTeam(){return team;}
    public void SetTeam(int newTeam){team = newTeam;}
    public int actions;
    public int GetActions(){return actions;}
    public void PayAttackCost()
    {
        actions--;
    }
    public bool ActionsLeft(){return actions > 0;}
    public int movement;
    protected void MoveAction()
    {
        actions--;
        movement += allStats.GetMoveSpeed();
    }
    public void PayMoveCost(int cost)
    {
        movement -= cost;
        if (movement < 0)
        {
            int maxActions = actions;
            for (int i = 0; i < maxActions; i++)
            {
                MoveAction();
            }
        }
    }
    public void NewTurn()
    {
        // Default is two actions.
        actions = Mathf.Max(actions, 2);
        movement = 0;
        // Apply status effects and passives.
    }
    public int GetMoveRange(bool current = true)
    {
        if (current)
        {
            return (movement + (allStats.GetMoveSpeed()*actions));
        }
        // Default is two actions.
        return allStats.GetMoveSpeed()*2;
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
    public int initiative;
    public int GetInitiative(){return initiative;}
    public void ChangeInitiative(int change){initiative += change;}
    public int direction;
    public int GetDirection(){return direction;}
    public void SetDirection(int newDirection){direction = newDirection;}
}
