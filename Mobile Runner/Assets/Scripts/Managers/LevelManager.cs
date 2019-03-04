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

        public void StartGame()
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

        public void EndGame(UIPanel showPanel)
        {
            StartCoroutine(IEndGame(showPanel));          
        }

        public void SlowTime()
        {
            if (IsSlowingTime == false)
                StartCoroutine(ISlowTime(slowModifier));
        }

        private bool ClearAllLevelObjects()
        {
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

            return true;
        }

        private void CreateStartingPlatforms()
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
            yield return new WaitWhile(() => IsSlowingTime);

            UIManager.Instance.FadeInAndOut(true);

            yield return new WaitWhile(() => UIManager.Instance.IsFading);

            UIManager.Instance.ChangePanel(UIManager.Instance.HudPanel);

            CreateStartingPlatforms();

            currentPlayer = ObjectPoolManager.Instance.SpawnObject(
                ResourceManager.Instance.PlayerPrefab,
                new Vector3(0, 0.6f, 0),
                Quaternion.identity,
                OthersParent).GetComponent<PlayerEngine>();
        }

        private IEnumerator IEndGame(UIPanel showPanel)
        {
            yield return new WaitWhile(() => IsSlowingTime);

            UIManager.Instance.FadeInAndOut(true, ClearAllLevelObjects);

            yield return new WaitWhile(() => UIManager.Instance.IsFading);

            UIManager.Instance.ChangePanel(showPanel);

            yield return null;
        }   

        #endregion COROUTINES       
    }
}