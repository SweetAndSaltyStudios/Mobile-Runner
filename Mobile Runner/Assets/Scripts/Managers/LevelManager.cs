using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SweetAndSaltyStudios
{
    public enum LEVEL_STATE
    {
        START,
        RUNNING,
        PAUSED,
        END
    }

    public class LevelManager : Singelton<LevelManager>
    {
        #region VARIABLES

        [Header("Level events")]
        public UnityEvent OnLevelStart = new UnityEvent();
        public UnityEvent OnLevelEnd = new UnityEvent();

        private Queue<GameObject> createdPlatforms = new Queue<GameObject>();
        private PlayerEngine currentPlayer;

        private float spawnZ = 8f;
        private float scoreModifier;

        private readonly float platformLenght = 30f;
        private readonly float safeZone = 30.0f;
        private readonly float slowModifier = 2f;
        private readonly int amountOfTilesOnScreen = 2;

        private int lastPlatformIndex = 0;
        private int collectableCount;

        #endregion VARIABLES

        #region PROPERTIES

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

        public LEVEL_STATE CurrentLevelState;

        #endregion PROPERTIES

        #region UNITY_FUNCTIONS

        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            if (CurrentLevelState.Equals(LEVEL_STATE.RUNNING) == false)
                return;

            //if (CurrentLevelState.Equals(LEVEL_STATE.START) && InputManager.Instance.FirstTouch)
            //{
            //    ChangeLevelState(LEVEL_STATE.RUNNING);
            //}

            if (currentPlayer.PlayerPosition.z - safeZone > (spawnZ - amountOfTilesOnScreen * platformLenght))
            {
                SpawnPlatform();
                DespawnPlatform();

                currentPlayer.IncreaseMovementSpeed();
            }
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

        public void StartLevel()
        {
            StartCoroutine(IStartLevel());
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

            return true;
        }

        public void ChangeLevelState(LEVEL_STATE newState)
        {
            CurrentLevelState = newState;

            switch (CurrentLevelState)
            {
                case LEVEL_STATE.START:

                    UIManager.Instance.CrossFade(CreateLevelStartingObjects);

                    OnLevelStart.Invoke();

                    break;

                case LEVEL_STATE.RUNNING:

                    break;

                case LEVEL_STATE.PAUSED:

                    break;

                case LEVEL_STATE.END:

                    UIManager.Instance.CrossFade(ClearAllLevelObjects);

                    OnLevelEnd.Invoke();

                    break;

                default:

                    break;
            }
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

        public void EndGame()
        {
            CameraEngine.Instance.Shake(Random.Range(0.15f, 0.25f), Random.Range(0.25f, 0.6f));
            StartCoroutine(ISlowTime(slowModifier));
            StartCoroutine(IEndGame());          
        }

        public void SlowTime()
        {
            if (IsSlowingTime == false)
                StartCoroutine(ISlowTime(slowModifier));
        }

        #endregion CUSTOM_FUNCTIONS

        #region COROUTINES

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

        private IEnumerator IStartLevel()
        {
            ChangeLevelState(LEVEL_STATE.START);

            yield return new WaitWhile(() => CurrentLevelState.Equals(LEVEL_STATE.START));

            yield return null;
        }

        private IEnumerator IEndGame()
        {
            ChangeLevelState(LEVEL_STATE.END);

            yield return new WaitWhile(() => CameraEngine.Instance.IsShaking);
            yield return new WaitWhile(() => IsSlowingTime);

          

            yield return null;
        }   

        #endregion COROUTINES       
    }
}