using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameManager : MonoBehaviour
{
    public CharacterList party;
    public SavedData savedData;

    public void StartGame()
    {
        savedData.Load();
        party.SplitAndSetLists(savedData.dataList);
    }
}
