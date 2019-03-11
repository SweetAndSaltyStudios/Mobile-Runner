using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SweetAndSaltyStudios
{
    public class GameManager : Singelton<GameManager>
    {
        #region VARIABLES

        [Header("Game Events")]
        public UnityEvent OnGameStart = new UnityEvent();
        public UnityEvent OnGamePaused = new UnityEvent();
        public UnityEvent OnGameEnd = new UnityEvent();

        private Queue<GameObject> createdPlatforms = new Queue<GameObject>();
        private PlayerEngine currentPlayer;

        private float spawnZ = 8f;
        private float scoreModifier;

        private readonly float platformLenght = 30f;
        private readonly float safeZone = 30.0f;
        private readonly float slowModifier = 2f;
        private readonly int amountOfTilesOnScreen = 2;

        private bool isLevelCreated;

        private int lastPlatformIndex = 0;
        private int collectableCount;

        #endregion VARIABLES

        #region PROPERTIES

        public GAME_STATE CurrentGameState
        {
            get;
            private set;
        }

        public GAME_STATE PreviousGameState
        {
            get;
            private set;
        }

        public Transform PlatformParent
        {
            get;
            private set;
        }
        public Transform OthersParent
        {
            get;
            private set;
        }

        public bool IsSlowingTime
        {
            get;
            private set;
        }
        public bool WaitForTap
        {
            get
            {
                return InputManager.Instance.FirstTap;
            }
        }
     
        public float CurrentTimeScale
        {
            get
            {
                return Time.timeScale;
            }
        }
        public float LastTimeScale
        {
            get;
            private set;
        }

        #endregion PROPERTIES

        #region UNITY_FUNCTIONS

        private void Awake()
        {
            Initialize();
        }

        #endregion UNITY_FUNCTIONS

        #region CUSTOM_FUNCTIONS

        private void Initialize()
        {
            var levelParent = new GameObject("Level").transform;
            levelParent.SetParent(transform);

            PlatformParent = new GameObject("Platforms").transform;
            PlatformParent.SetParent(levelParent);

            OthersParent = new GameObject("Others").transform;
            OthersParent.SetParent(levelParent);
        }

        private int RandomPlatformIndex()
        {
            if (ResourceManager.Instance.PlatformPrefabs.Length <= 1)
                return 0;

            var randomIndex = lastPlatformIndex;

            do
            {
                randomIndex = Random.Range(0, ResourceManager.Instance.PlatformPrefabs.Length);
            }
            while (randomIndex == lastPlatformIndex);

            lastPlatformIndex = randomIndex;

            return randomIndex;
        }

        private void ChangeTimeScale(float newTimeScale)
        {
            LastTimeScale = CurrentTimeScale;

            Time.timeScale = newTimeScale;
        }

        private void SpawnPlatform(int platformIndex = -1)
        {
            GameObject newPlatform;

            if (platformIndex == -1)
            {
                newPlatform = ObjectPoolManager.Instance.SpawnObject(
                  ResourceManager.Instance.PlatformPrefabs[RandomPlatformIndex()],
                  Vector3.forward * spawnZ,
                  Quaternion.identity,
                  PlatformParent
               );
            }
            else
            {
                newPlatform = ObjectPoolManager.Instance.SpawnObject(
                   ResourceManager.Instance.PlatformPrefabs[platformIndex],
                   Vector3.forward * spawnZ,
                   Quaternion.identity,
                   PlatformParent
               );
            }

            spawnZ += platformLenght;
            createdPlatforms.Enqueue(newPlatform);
        }

        private void DespawnPlatform()
        {
            ObjectPoolManager.Instance.DespawnObject(createdPlatforms.Dequeue());
        }

        private bool CreateLevelStartingObjects()
        {
            spawnZ = 0;

            for (int i = 0; i < amountOfTilesOnScreen; i++)
            {
                if (i < 2)
                {
                    SpawnPlatform(0);
                }
                else
                {
                    SpawnPlatform();
                }
            }

            currentPlayer = ObjectPoolManager.Instance.SpawnObject(
              ResourceManager.Instance.PlayerPrefab,
              new Vector3(0, 0.6f, 0),
              Quaternion.identity,
              OthersParent).GetComponent<PlayerEngine>();

            isLevelCreated = true;

            return true;
        }

        private bool ClearAllLevelObjects()
        {
            ObjectPoolManager.Instance.DespawnObject(currentPlayer.gameObject);

            do
            {
                if (createdPlatforms != null)
                {
                    DespawnPlatform();
                }
            }
            while (createdPlatforms.Count > 0);

            isLevelCreated = false;
            return true;
        }

        public void UpdateScoreModifier(float modifierAmount)
        {
            scoreModifier += modifierAmount;

            UIManager.Instance.UpdateScoreModifier(modifierAmount);
        }

        public void AddCollectable(int amount)
        {
            collectableCount += amount;
            UIManager.Instance.UpdateCollectableCount(collectableCount);
        }

        public void SlowTime()
        {
            if (IsSlowingTime == false)
                StartCoroutine(ISlowTime(slowModifier));
        }

        public void ChangeGameState(GAME_STATE newGameState)
        {
            PreviousGameState = CurrentGameState;       
            CurrentGameState = newGameState;

            switch (CurrentGameState)
            {
                case GAME_STATE.START:

                    if (isLevelCreated == false)
                        StartCoroutine(IStartGame());
                   
                    break;

                case GAME_STATE.PAUSE:

                    StartCoroutine(IPauseGame());

                    break;

                case GAME_STATE.RUNNING:

                    StartCoroutine(IRunGame());

                    break;

                case GAME_STATE.END:

                    if (isLevelCreated)
                        StartCoroutine(IEndGame());

                    break;

                default:

                    break;
            }
        }

        public void ChangePreviousGameState()
        {
            ChangeGameState(PreviousGameState);
        }

        #endregion CUSTOM_FUNCTIONS

        #region COROUTINES

        private IEnumerator IRunGame()
        {
            while(currentPlayer != null)
            {
                if (currentPlayer.PlayerPosition.z - safeZone > (spawnZ - amountOfTilesOnScreen * platformLenght))
                {
                    SpawnPlatform();
                    DespawnPlatform();

                    currentPlayer.IncreaseMovementSpeed();
                }

                yield return null;
            }      
        }

        private IEnumerator IPauseGame()
        {
            ChangeTimeScale(0f);

            yield return new WaitWhile(() => CurrentGameState.Equals(GAME_STATE.PAUSE));

            ChangeTimeScale(LastTimeScale);
        }

        private IEnumerator ISlowTime(float slowDuration)
        {
            IsSlowingTime = true;

            Time.timeScale = 1f / slowModifier;
            Time.fixedDeltaTime /= slowModifier;

            yield return new WaitForSeconds(slowDuration / slowModifier);

            Time.timeScale = 1f;
            Time.fixedDeltaTime *= slowModifier;

            IsSlowingTime = false;
        }

        private IEnumerator IStartGame()
        {
            yield return new WaitUntil(() => CreateLevelStartingObjects());

            yield return new WaitUntil(() => InputManager.Instance.FirstTap);

            ChangeGameState(GAME_STATE.RUNNING);
        }

        private IEnumerator IEndGame()
        {
            yield return new WaitUntil(() => CameraEngine.Instance.IsShaking == false);
            yield return new WaitUntil(() => IsSlowingTime == false);
            yield return new WaitUntil(() => ClearAllLevelObjects());

            OnGameEnd.Invoke();
        }

        #endregion COROUTINES       
    }
}