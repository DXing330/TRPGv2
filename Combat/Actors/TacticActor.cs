using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticActor : ActorStats
{
    public string weaponType;
    public void ResetWeaponType()
    {
        weaponType = "";
        weaponName = "";
    }
    public void SetWeaponType(string newWeapon){weaponType = newWeapon;}
    public string GetWeaponType(){return weaponType;}
    public string weaponName;
    public void ResetWeaponName()
    {
        weaponType = "";
        weaponName = "";
    }
    public void SetWeaponName(string newInfo){weaponName = newInfo;}
    public string GetWeaponName(){return weaponName;}
    public GameObject actorObject;
    public void DestroyActor(){DestroyImmediate(actorObject);}
    public int team;
    public int GetTeam(){return team;}
    public void SetTeam(int newTeam){team = newTeam;}
    public int baseActions = 2;
    public void UpdateBaseActions(int amount){baseActions += amount;}
    public int GetBaseActions() { return baseActions; }
    public void SetBaseActions(int newActions){baseActions = newActions;}
    public int actions;
    public void SetActions(int newActions){actions = newActions;}
    public void ResetActions(){actions = 0;}
    // AAA
    public void AdjustActionAmount(int change){actions += change;}
    public void SpendAction()
    {
        if (bonusActions > 0)
        {
            bonusActions--;
            return;
        }
        actions--;
    }
    public int bonusActions;
    public void ResetBonusActions(){bonusActions = 0;}
    public void GainBonusActions(int amount){bonusActions += amount;}
    public int GetActions(){return actions + bonusActions;}
    public void PayAttackCost()
    {
        SpendAction();
    }
    public bool ActionsLeft(){return actions > 0;}
    public void PayActionCost(int cost){actions -= cost;}
    public int movement;
    public int GetMovement(){return movement;}
    protected void MoveAction()
    {
        SpendAction();
        movement += GetSpeed();
    }
    public void GainMovement(int amount){movement += amount;}
    public void PayMoveCost(int cost)
    {
        movement -= cost;
        if (movement < 0)
        {
            int maxActions = actions;
            for (int i = 0; i < maxActions; i++)
            {
                MoveAction();
                if (movement >= 0){break;}
            }
        }
    }
    public int counterAttacks;
    public void GainCounterAttacks(int amount = 1)
    {
        counterAttacks += amount;
    }
    public bool CounterAttackAvailable()
    {
        return counterAttacks > 0;
    }
    public void UseCounterAttack()
    {
        counterAttacks--;
    }
    public void NewTurn()
    {
        // Default is two actions.
        actions = Mathf.Max(actions, baseActions);
        movement = 0;
        counterAttacks = 0;
        ResetStats();
        
    }
    public int GetMoveRangeBasedOnActions(int actionCount)
    {
        return (movement + (GetSpeed()*actionCount));
    }
    public int GetMoveRange(bool current = true)
    {
        if (current)
        {
            return (movement + (GetSpeed()*actions));
        }
        // Default is two actions.
        return GetMoveSpeed()*2;
    }
    public int GetMoveRangeWhileAttacking(bool current = true)
    {
        if (current)
        {
            return (movement + (GetSpeed()*(actions-1)));
        }
        return GetMoveSpeed();
    }
    public string personalName;
    public void SetPersonalName(string newName){personalName = newName;}
    public string GetPersonalName()
    {
        if (personalName.Length <= 0){return GetSpriteName();}
        return personalName;
    }
    public string element;
    public void SetElement(string newInfo){element = newInfo;}
    public string GetElement(){return element;}
    // Used mainly for enemy AI
    public int counter = 0;
    public void ResetCounter(){counter = 0;}
    public void UpdateCounter(int changeAmount)
    {
        counter += changeAmount;
    }
    public void IncrementCounter(){counter++;}
    public void SetCounter(int newInfo){counter = newInfo;}
    public int GetCounter(){return counter;}
    public string species;
    public void SetSpecies(string newSpecies){species = newSpecies;}
    public string GetSpecies(){return species;}
    public string spriteName;
    public void SetSpriteName(string newName){spriteName = newName;}
    public string GetSpriteName(){return spriteName;}
    public int location;
    public void SetLocation(int newLocation){location = newLocation;}
    public int GetLocation(){return location;}
    public int direction;
    public int GetDirection(){return direction;}
    public void SetDirection(int newDirection){direction = newDirection;}
    protected string immuneMentalState = "Calm";
    public string mentalState;
    public void ResetMentalState()
    {
        if (mentalState == immuneMentalState) { return; }
        mentalState = "";
    }
    public void SetMentalState(string newInfo)
    {
        // Can change to immune no matter what.
        if (newInfo == immuneMentalState)
        {
            mentalState = immuneMentalState;
            return;
        }
        // Immunity blocks one negative change.
            if (mentalState == immuneMentalState)
            {
                mentalState = "";
                return;
            }
        mentalState = newInfo;
    }
    public string GetMentalState(){ return mentalState; }
    public TacticActor target;
    public void ResetTarget(){ target = null; }
    public void SetTarget(TacticActor newTarget) { target = newTarget; }
    public TacticActor GetTarget()
    {
        if (!TargetAlive()){ResetTarget();}
        return target;
    }
    public bool TargetAlive()
    {
        if (target == null){return false;}
        return target.GetHealth() > 0;
    }
    public TacticActor grappledActor;
    public TacticActor GetGrappledActor(){return grappledActor;}
    public void ResetGrappledActor(){grappledActor = null;}
    public void ReleaseGrapple()
    {
        if (grappledActor != null)
        {
            grappledActor.ResetGrappledByActor();
            ResetGrappledActor();
        }
    }
    public void GrappleActor(TacticActor newGrapple)
    {
        // Break any grapple you have to try to grapple someone else.
        ReleaseGrapple();
        // Can't grabble someone that is already grappled.
        if (newGrapple.Grappled()){return;}
        grappledActor = newGrapple;
        grappledActor.SetGrappledByActor(this);
    }
    public TacticActor grappledByActor;
    public void SetGrappledByActor(TacticActor actor){grappledByActor = actor;}
    public TacticActor GetGrappledByActor(){return grappledByActor;}
    public void ResetGrappledByActor(){grappledByActor = null;}
    public void BreakGrapple()
    {
        if (grappledByActor != null)
        {
            grappledByActor.ResetGrappledActor();
            ResetGrappledByActor();
        }
    }
    public bool Grappled(BattleMap map = null)
    {
        
        if (grappledByActor != null)
        {
            // Dead.
            if (grappledByActor.GetHealth() <= 0)
            {
                BreakGrapple();
                return false;
            }
            // Too heavy to be grappled.
            if (grappledByActor.GetWeight() < GetWeight() - 1)
            {
                BreakGrapple();
                return false;
            }
            // Out of range.
            if (map != null && map.DistanceBetweenActors(this, grappledByActor) > 1)
            {
                BreakGrapple();
                return false;
            }
            return true;
        }
        return false;
    }
    public bool Grappling(BattleMap map = null)
    {
        if (grappledActor != null)
        {
            if (grappledActor.GetHealth() <= 0)
            {
                ReleaseGrapple();
                return false;
            }
            if (grappledActor.GetWeight() > GetWeight() + 1)
            {
                ReleaseGrapple();
                return false;
            }
            if (map != null && map.DistanceBetweenActors(this, grappledActor) > 1)
            {
                ReleaseGrapple();
                return false;
            }
            return true;
        }
        return false;
    }

    public void EndTurn()
    {
        movement = 0;
        // Allow some slight turn manipulation by saving your actions.
        if (actions > 0)
        {
            UpdateTempInitiative(actions * 2);
            ResetActions();
        }
        EndTurnResetStats();
        ResetBonusActions();
        ResetMentalState();
        CheckStatusDuration();
    }

    public List<string> ReturnSpendableStats()
    {
        List<string> stats = new List<string>();
        stats.Add(GetActions().ToString());
        stats.Add(GetEnergy().ToString());
        stats.Add(GetMovement().ToString());
        return stats;
    }

    public string ReturnPersistentStats(string delimiter = "|")
    {
        string healthString = GetHealth().ToString();
        string curses = GetCurseString();
        return healthString + delimiter + curses;
    }
}
