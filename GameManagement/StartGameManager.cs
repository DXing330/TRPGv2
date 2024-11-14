using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameManager : MonoBehaviour
{
    public SavedData savedData;

    public void Start()
    {
        savedData.Load();
    }

    public void StartGame()
    {
        savedData.Load();
    }
}
