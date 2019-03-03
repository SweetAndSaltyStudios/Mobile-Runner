using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweetAndSaltyStudios
{
    public class LevelManager : Singelton<LevelManager>
    {
        #region VARIABLES
   
        private Queue<GameObject> createdPlatforms = new Queue<GameObject>();
        private PlayerEngine currentPlayer;

        private float spawnZ = 8f;
        private float scoreModifier;

        private readonly float platformLenght = 30f;
        private readonly float safeZone = 30.0f;
        private readonly float slowModifier = 2f;
        private readonly int amountOfTilesOnScreen = 10;

        private int lastPlatformIndex = 0;
        private int collectableCount;

        #endregion VARIABLES

        #region PROPERTIES

        public bool IsLevelCleared
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

        #endregion PROPERTIES

        #region UNITY_FUNCTIONS

        private void Awake()
        {
            var levelParent = new GameObject("Level").transform;
            levelParent.SetParent(transform);

            PlatformParent = new GameObject("Platforms").transform;
            PlatformParent.SetParent(levelParent);

            OthersParent = new GameObject("Others").transform;
            OthersParent.SetParent(levelParent);
        }

        private void Update()
        {
            if (currentPlayer == null)
                return;

            if (currentPlayer.PlayerPosition.z - safeZone > (spawnZ - amountOfTilesOnScreen * platformLenght))
            {
                SpawnPlatform();
                DespawnPlatform();

                currentPlayer.IncreaseMovementSpeed();
            }
        }

        #endregion UNITY_FUNCTIONS

        #region CUSTOM_FUNCTIONS

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

        public void StartNewGame()
        {
            CreateLevel();
        }

        private void CreateLevel()
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

        public void ClearLevel()
        {
            StartCoroutine(IClearLevel());
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

        private IEnumerator IClearLevel()
        {
            IsLevelCleared = false;

            StartCoroutine(ISlowTime(slowModifier));

            yield return new WaitWhile(() => IsSlowingTime);

            // Fadeing...

            UIManager.Instance.ScreenFadeShader(0f);

            yield return new WaitWhile(() => UIManager.Instance.IsFading);

            // Fake loadtime (Show messages like game over or loading...)
            UIManager.Instance.LoadingText.gameObject.SetActive(true);
                
            // Clear level objects
            ObjectPoolManager.Instance.DespawnObject(currentPlayer.gameObject);

            do
            {
                if (createdPlatforms != null)
                {
                    DespawnPlatform();
                }
            }
            while (createdPlatforms.Count > 0);

            yield return new WaitForSeconds(2f);
            UIManager.Instance.LoadingText.gameObject.SetActive(false);

            UIManager.Instance.ChangePanel(UIManager.Instance.GameOverPanel);

            UIManager.Instance.ScreenFadeShader(1f);

            yield return new WaitWhile(() => UIManager.Instance.IsFading);

            // Done fading

            IsLevelCleared = true;

            yield return null;
        }

        #endregion COROUTINES       
    }
}