using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameManager : MonoBehaviour
{
    public PartyDataManager partyManager;
    public List<SavedData> gameData;

    public void StartGame()
    {
        partyManager.Load();
        for (int i = 0; i < gameData.Count; i++)
        {
            gameData[i].Load();
        }
    }
}
