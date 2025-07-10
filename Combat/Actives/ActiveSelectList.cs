using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActiveSelectList : SelectList
{
    public BattleManager battle;
    public ActiveManager activeManager;
    public ActiveDescriptionViewer descriptionViewer;
    public TMP_Text activeDescription;
    public int state;
    public string spellState = "Spell";
    public Inventory inventory;
    public List<string> useableItems;
    public List<int> useableQuantities;
    public void UpdateUseableItems()
    {
        useableItems.Clear();
        useableQuantities.Clear();
        for (int i = 0; i < inventory.items.Count; i++)
        {
            if (activeManager.activeData.KeyExists(inventory.items[i]))
            {
                useableItems.Add(inventory.items[i]);
                useableQuantities.Add(int.Parse(inventory.quantities[i]));
            }
        }
        for (int i = useableItems.Count - 1; i >= 0; i--)
        {
            if (useableQuantities[i] <= 0)
            {
                useableItems.RemoveAt(i);
                useableQuantities.RemoveAt(i);
            }
        }
    }
    public List<GameObject> statePanels;
    protected void ResetPanels()
    {
        utility.DisableGameObjects(statePanels);
    }

    public void SetState(int newState)
    {
        ResetPanels();
        state = newState;
        statePanels[state].SetActive(true);
    }

    public override void Select(int index)
    {
        base.Select(index);
        if (battle.GetState() == spellState)
        {
            SelectSpell();
            return;
        }
        IncrementState();
        ShowSelected();
        activeManager.SetSkillFromName(selected);
        activeDescription.text = descriptionViewer.ReturnActiveDescription(activeManager.active);
        // Check cost here.
        if (!activeManager.CheckSkillCost())
        {
            // Show an error message instead of just returning?
            if (!activeManager.CheckActionCost()) { errorText.text = errorMessages[0]; }
            else { errorText.text = errorMessages[1]; }
            ErrorMessage();
            return;
        }
        activeManager.GetTargetableTiles(battle.GetTurnActor().GetLocation(), battle.moveManager.actorPathfinder);
        activeManager.ResetTargetedTiles();
        activeManager.CheckIfSingleTargetableTile();
        battle.map.UpdateHighlights(activeManager.ReturnTargetableTiles());
    }

    protected void SelectSpell()
    {
        IncrementState();
        // Show the spell by name.
        activeManager.SetSpell(selected);
        UpdateSelectedText(activeManager.magicSpell.GetSkillType());
        Debug.Log(selected);
        Debug.Log(activeManager.magicSpell.GetSkillType());
        if (!activeManager.CheckSpellCost())
        {
            // Show an error message instead of just returning?
            errorText.text = "Not enough resources.";
            ErrorMessage();
            return;
        }
        activeDescription.text = descriptionViewer.ReturnSpellDescription(activeManager.magicSpell);
        activeManager.GetTargetableTiles(battle.GetTurnActor().GetLocation(), battle.moveManager.actorPathfinder, true);
        activeManager.ResetTargetedTiles();
        activeManager.CheckIfSingleTargetableTile();
        battle.map.UpdateHighlights(activeManager.ReturnTargetableTiles());
    }

    public void Deselect()
    {
        DecrementState();
        battle.map.ResetHighlights();
        //StartingPage();
    }

    protected void DecrementState()
    {
        int newState = state - 1;
        SetState(newState);
    }

    protected void IncrementState()
    {
        int newState = state + 1;
        SetState(newState);
    }

    public void ResetState()
    {
        SetState(0);
        ErrorMessage(false);
    }

    public void StartSelecting()
    {
        if (battle.GetTurnActor().ActiveSkillCount() <= 0 || battle.GetTurnActor().GetActions() <= 0) { return; }
        IncrementState();
        SetSelectables(battle.GetTurnActor().GetActiveSkills());
        activeManager.SetSkillUser(battle.GetTurnActor());
        activeManager.ResetTargetedTiles();
        StartingPage();
    }

    public void StartSelectingItems()
    {
        UpdateUseableItems();
        if (useableItems.Count <= 0 || battle.GetTurnActor().GetActions() <= 0) { return; }
        IncrementState();
        SetSelectables(useableItems);
        activeManager.SetSkillUser(battle.GetTurnActor());
        activeManager.ResetTargetedTiles();
        StartingPage();
    }

    public void StartSelectingSpells()
    {
        if (battle.GetTurnActor().SpellCount() <= 0 || battle.GetTurnActor().GetActions() <= 0) { return; }
        IncrementState();
        SetSelectables(battle.GetTurnActor().GetSpells());
        activeManager.SetSkillUser(battle.GetTurnActor());
        activeManager.ResetTargetedTiles();
        StartingPage();
    }

    public void ActivateSkill()
    {
        // Don't do anything unless a target has been selected.
        if (!activeManager.ExistTargetedTiles())
        {
            errorText.text = errorMessages[2];
            ErrorMessage();
            return;
        }
        // Its a spell then do something a little different
        if (battle.GetState() == spellState)
        {
            ActivateSpell();
            ResetState();
            return;
        }
        battle.ActivateSkill(selected);
        activeManager.ActivateSkill(battle);
        battle.turnNumber = battle.map.RemoveActorsFromBattle(battle.GetTurnIndex());
        // Check if the skill you just used was an item.
        if (inventory.ItemExists(selected))
        {
            inventory.RemoveItemQuantity(1, selected);
        }
        ResetState();
    }

    public void ActivateSpell()
    {
        battle.ActivateSpell();
        activeManager.ActivateSpell(battle);
    }
}
