using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpellStore : MonoBehaviour
{
    void Start()
    {
        CheckIfCastersInParty();
        UpdateCurrency();
        spellStoreOptionsObject.SetActive(false);
    }
    public PartyDataManager partyData;
    public TacticActor dummyActor;
    public TMP_Text tomes;
    public string tomeString = "Tome";
    public TMP_Text mana;
    public string manaString = "Mana";
    public void UpdateCurrency()
    {
        tomes.text = partyData.inventory.ReturnQuantityOfItem(tomeString).ToString();
        mana.text = partyData.inventory.ReturnQuantityOfItem(manaString).ToString();
    }
    public List<string> spellCasterClasses;
    public GameObject casterSelectObject;
    public ActorSpriteHPList partyCasters;
    public GameObject noCasterErrorObject;
    public void CheckIfCastersInParty()
    {
        bool casters = false;
        for (int i = 0; i < spellCasterClasses.Count; i++)
        {
            if (partyData.PartyMemberClassExists(spellCasterClasses[i]))
            {
                casters = true;
                break;
            }
        }
        if (casters)
        {
            casterSelectObject.SetActive(true);
            partyCasters.RefreshData(spellCasterClasses);
            noCasterErrorObject.SetActive(false);
        }
        else
        {
            casterSelectObject.SetActive(false);
            noCasterErrorObject.SetActive(true);
        }
    }
    public int selectedCaster;
    public GameObject spellStoreOptionsObject;
    public void SelectCaster()
    {
        selectedCaster = partyCasters.GetSelected();
        dummyActor.SetStatsFromString(partyCasters.GetSelectedData());
        spellStoreOptionsObject.SetActive(true);
    }
    public SelectList spellsList;
}
