using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StSEvent", menuName = "ScriptableObjects/RogueLike/StSEvent", order = 1)]
public class StSEvent : SavedData
{
    // Stuff to track the event.
    public StatDatabase eventData;
    public string eventName;
    public string GetEventName()
    {
        return eventName;
    }
    public string eventDetails;
    public string SceneChangeEvent()
    {
        if (choices.Count == 1)
        {
            SelectChoice(0);
            if (eventSpecifics.Count == 1)
            {
                // Reset the event for next time.
                eventName = "";
                Save();
                return eventSpecifics[0];
            }
        }
        return "";
    }
    public string currentChoice;
    public List<string> eventTarget;
    public List<string> eventEffect;
    public List<string> eventSpecifics;
    public List<string> choices;
    public List<string> GetChoices()
    {
        return new List<string>(choices);
    }
    // Stuff to apply skill effects.
    public SkillEffect skillEffect;

    public override void Save()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        allData = eventName;
        File.WriteAllText(dataPath, allData);
    }

    public override void Load()
    {
        dataPath = Application.persistentDataPath + "/" + filename;
        if (File.Exists(dataPath))
        {
            allData = File.ReadAllText(dataPath);
            if (allData == "")
            {
                return;
            }
            else
            {
                eventName = allData;
                eventDetails = eventData.ReturnValue(eventName);
                choices = eventDetails.Split("&").ToList();
            }
        }
        else
        {
            return;
        }
    }

    public void ForceGenerate()
    {
        eventName = eventData.ReturnRandomKey();
        eventDetails = eventData.ReturnValue(eventName);
        choices = eventDetails.Split("&").ToList();
    }

    public void GenerateEvent()
    {
        Load();
        if (eventName == "")
        {
            eventName = eventData.ReturnRandomKey();
            eventDetails = eventData.ReturnValue(eventName);
            choices = eventDetails.Split("&").ToList();
            Save();
        }
    }

    public void SelectChoice(int index)
    {
        currentChoice = choices[index];
        string[] blocks = currentChoice.Split("|");
        eventTarget = blocks[1].Split(",").ToList();
        eventEffect = blocks[2].Split(",").ToList();
        eventSpecifics = blocks[3].Split(",").ToList();
    }

    public void ApplyEventEffects(PartyDataManager partyData, TacticActor actor = null, int selectedIndex = -1)
    {
        for (int i = 0; i < eventTarget.Count; i++)
        {
            switch (eventTarget[i])
            {
                case "Inventory":
                    partyData.inventory.AddItemQuantity(eventEffect[i], int.Parse(eventSpecifics[i]));
                    break;
                case "Chosen Actor":
                    // Apply the effect.
                    ApplyEffectToActor(partyData, actor, selectedIndex, eventEffect[i], eventSpecifics[i]);
                    partyData.RemoveDeadPartyMembers();
                    break;
                case "All Actors":
                    // Apply the effect.
                    for (int j = 0; j < partyData.ReturnTotalPartyCount(); j++)
                    {
                        ApplyEffectToActor(partyData, partyData.ReturnActorAtIndex(j), j, eventEffect[i], eventSpecifics[i]);
                    }
                    partyData.RemoveDeadPartyMembers();
                    break;
            }
        }
        partyData.SetFullParty();
        // Reset the event for next time.
        eventName = "";
        Save();
    }

    public void ApplyEffectToActor(PartyDataManager partyData, TacticActor actor, int partyIndex, string effect, string specifics)
    {
        if (actor == null){ return; }
        switch (effect)
        {
            case "Remove":
                partyData.RemovePartyMember(partyIndex);
                return;
        }
        skillEffect.AffectActor(actor, effect, specifics);
        partyData.UpdatePartyMember(actor, partyIndex);
    }
}
