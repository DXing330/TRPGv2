using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMover : MonoBehaviour
{
    public SceneTracker sceneTracker;

    public void LoadScene(string sceneName)
    {
        sceneTracker.SetPreviousScene(SceneManager.GetActiveScene().name);
        StartCoroutine(LoadAsyncScene(sceneName));
    }

    public void ReturnFromBattle(int victory = 0)
    {
        StartCoroutine(LoadAsyncScene(sceneTracker.GetPreviousScene()));
    }

    IEnumerator LoadAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
