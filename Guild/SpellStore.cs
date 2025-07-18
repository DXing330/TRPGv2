using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpellStore : MonoBehaviour
{
    void Start()
    {
        partyData.SetFullParty();
        CheckIfCastersInParty();
        UpdateCurrency();
        spellStoreOptionsObject.SetActive(false);
    }
    public GeneralUtility utility;
    public PartyDataManager partyData;
    public SpellBook spellBook;
    public CharacterList spellCasterList;
    public TacticActor dummyActor;
    public TMP_Text spellSlots;
    public void UpdateSpellSlots()
    {
        spellSlots.text = dummyActor.GetSpells().Count + "/" + spellBook.ReturnActorSpellSlots(dummyActor);
    }
    public bool OpenSpellSlots()
    {
        return dummyActor.GetSpells().Count < spellBook.ReturnActorSpellSlots(dummyActor);
    }
    public TMP_Text tomes;
    public string tomeString = "Tome";
    public TMP_Text mana;
    public string manaString = "Mana";
    public void UpdateCurrency()
    {
        tomes.text = partyData.inventory.ReturnQuantityOfItem(tomeString).ToString();
        mana.text = partyData.inventory.ReturnQuantityOfItem(manaString).ToString();
    }
    public GameObject casterSelectObject;
    public ActorSpriteHPList partyCasters;
    public GameObject noCasterErrorObject;
    public void CheckIfCastersInParty()
    {
        bool casters = false;
        List<string> casterStats = new List<string>();
        List<string> casterSprites = new List<string>();
        List<string> casterNames = new List<string>();
        List<string> characterStats = partyData.fullParty.GetCharacterStats();
        List<string> characterSprites = partyData.fullParty.GetCharacterSprites();
        List<string> characterNames = partyData.fullParty.GetCharacterNames();
        for (int i = 0; i < characterStats.Count; i++)
        {
            dummyActor.SetStatsFromString(characterStats[i]);
            if (spellBook.ReturnActorSpellSlots(dummyActor) > 0)
            {
                casters = true;
                casterStats.Add(characterStats[i]);
                casterSprites.Add(characterSprites[i]);
                casterNames.Add(characterNames[i]);
            }
        }
        if (casters)
        {
            spellCasterList.SetLists(casterSprites, casterStats, casterNames);
            casterSelectObject.SetActive(true);
            partyCasters.RefreshData();
            noCasterErrorObject.SetActive(false);
        }
        else
        {
            casterSelectObject.SetActive(false);
            noCasterErrorObject.SetActive(true);
        }
    }
    public int selectedCaster;
    public ActorSpriteAndName casterSpriteAndName;
    public GameObject spellStoreOptionsObject;
    public PopUpMessage errorPopUp;
    protected void NoSpellSlotError()
    {
        errorPopUp.SetMessage("No Spell Slots Available");
        ResetState();
    }
    public void SelectCaster()
    {
        selectedCaster = partyCasters.GetSelected();
        dummyActor.SetStatsFromString(partyCasters.GetSelectedData());
        spellStoreOptionsObject.SetActive(true);
        casterSpriteAndName.ShowActorInfo(dummyActor);
        UpdateSpellSlots();
    }
    public List<GameObject> storePanels;
    public int state = -1;
    public void ResetState()
    {
        utility.DisableGameObjects(storePanels);
    }
    public void SetState(int newInfo)
    {
        ResetState();
        if (state == newInfo || newInfo == -1)
        {
            state = -1;
            return;
        }
        state = newInfo;
        storePanels[state].SetActive(true);
    }
    public SelectList spellsList;
    protected List<string> currentActorSpells;
    public SpellDetailViewer spellDetails;
    public void StartViewingSpells()
    {
        // Get the list of spells from the actor.
        // Get the spell names of each spell.
        currentActorSpells = new List<string>(dummyActor.GetSpells());
        spellsList.SetSelectables(dummyActor.GetSpellNames());
        spellDetails.ResetDetails();
    }
    public void ViewSelectedSpell()
    {
        int index = spellsList.GetSelected();
        spellDetails.LoadSpell(currentActorSpells[index]);
    }
    public void StartUpgradingSpells()
    {
        if (!OpenSpellSlots())
        {
            NoSpellSlotError();
            return;
        }
    }
    public void StartLearningSpells()
    {
        if (!OpenSpellSlots())
        {
            NoSpellSlotError();
            return;
        }
    }
    public void StartCombiningSpells()
    {
        if (!OpenSpellSlots())
        {
            NoSpellSlotError();
            return;
        }
    }
}
