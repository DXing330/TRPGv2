using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMover : MonoBehaviour
{
    public LoadingScreen loadingScreen;
    public SceneTracker sceneTracker;
    public bool loadingRequired = false;

    public void LoadScene(string sceneName)
    {
        sceneTracker.SetPreviousScene(SceneManager.GetActiveScene().name);
        if (loadingRequired)
        {
            StartCoroutine(LoadingScreenMoveScene(sceneName));
        }
        else
        {
            StartCoroutine(LoadAsyncScene(sceneName));
        }
    }

    public void MoveToBattle()
    {
        sceneTracker.SetPreviousScene(SceneManager.GetActiveScene().name);
        if (loadingRequired)
        {
            StartCoroutine(LoadingScreenMoveScene("BattleScene"));
        }
        else
        {
            StartCoroutine(LoadAsyncScene("BattleScene"));
        }
    }

    public void ReturnFromDungeon(bool clear = true)
    {
        // TODO: Go to victory screen.
        // For now just go back to hub.
        if (loadingRequired)
        {
            StartCoroutine(LoadingScreenMoveScene("Hub"));
        }
        else
        {
            StartCoroutine(LoadAsyncScene("Hub"));
        }
    }

    public void ReturnFromBattle(int victory = 0)
    {
        // If you lose go somewhere else.
        //if (victory != 0)
        //else
        if (loadingRequired)
        {
            StartCoroutine(LoadingScreenMoveScene(sceneTracker.GetPreviousScene()));
        }
        else
        {
            StartCoroutine(LoadAsyncScene(sceneTracker.GetPreviousScene()));
        }
    }

    IEnumerator LoadAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    IEnumerator LoadingScreenMoveScene(string sceneName)
    {
        for (int i = 0; i < 2; i++)
        {
            if (i == 0)
            {
                loadingScreen.StartLoadingScreen();
            }
            if (i == 1)
            {
                StartCoroutine(LoadAsyncScene(sceneName));
            }
            yield return new WaitForSeconds(loadingScreen.totalFadeTime);
        }
    }
}
