using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster : SingeltonPersistant<GameMaster>
{
    public float Slowness = 10f;
    private bool isLoading;

    public int CurrentSceneIndex
    {
        get
        {
            return SceneManager.GetActiveScene().buildIndex;
        }
    }

    public int NextSceneIndex
    {
        get
        {
            var nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            return nextSceneIndex >= SceneManager.sceneCount ? 0 : nextSceneIndex;
        }
    }

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void RestartScene()
    {
        if(isLoading == false)
        StartCoroutine(ISlowTime(3f));
    }

    private IEnumerator ISlowTime(float slowDuration)
    {
        isLoading = true;

        Time.timeScale = 1f / Slowness;
        Time.fixedDeltaTime /= Slowness;

        yield return new WaitForSeconds(slowDuration / Slowness);

        Time.timeScale = 1f;
        Time.fixedDeltaTime *= Slowness;

        LoadScene(CurrentSceneIndex);

        isLoading = false;
    }
}
