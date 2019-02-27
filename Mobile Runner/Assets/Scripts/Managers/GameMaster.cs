using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GAMESTATE
{
    MAINMENU,
    PAUSED,
    RUNNING
}

public class GameMaster : SingeltonPersistant<GameMaster>
{
    public GAMESTATE CurrentGamestate;


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
        private set
        {

        }
    }

    protected override void Awake()
    {
        Initialize();
    }

    private void Initialize()
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

    public void ChangeGameState(GAMESTATE newGamestate)
    {
        CurrentGamestate = newGamestate;

        switch (CurrentGamestate)
        {
            case GAMESTATE.MAINMENU:

                Debug.LogWarning("GAMESTATE: " + CurrentGamestate);

                break;

            case GAMESTATE.PAUSED:

                Debug.LogWarning("GAMESTATE: " + CurrentGamestate);

                break;

            case GAMESTATE.RUNNING:

                Debug.LogWarning("GAMESTATE: " + CurrentGamestate);

                break;

            default:

                Debug.LogWarning("GAMESTATE: Default");

                break;
        }
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
