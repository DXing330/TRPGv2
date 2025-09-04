using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StSEventScene : MonoBehaviour
{
    public GeneralUtility utility;
    public StSEvent stsEvent;
    public PartyDataManager partyData;
    public TMP_Text eventName;
    public TMP_Text eventDescription;
    public List<GameObject> choiceObjects;
    public void ResetChoices()
    {
        utility.DisableGameObjects(choiceObjects);
    }
    public List<TMP_Text> eventChoices;
    public List<TMP_Text> choiceEffects;
    public ActiveDescriptionViewer descriptionViewer;
    public GameObject actorSelect;

    void Start()
    {
        stsEvent.GenerateEvent();
        DisplayEvent();
    }

    public void DisplayEvent()
    {
        eventName.text = stsEvent.GetEventName();
        ResetChoices();
        List<string> choices = stsEvent.GetChoices();
        for (int i = 0; i < choices.Count; i++)
        {
            choiceObjects[i].SetActive(true);
            string[] blocks = choices[i].Split("|");
            eventChoices[i].text = blocks[0];
        }
    }

    public void SelectChoice(int index)
    {

    }
}
