using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweetAndSaltyStudios
{
    public class LevelManager : Singelton<LevelManager>
    {
        private Queue<GameObject> createdPlatforms = new Queue<GameObject>();

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

        private float spawnZ = 8f;
        private readonly float platformLenght = 30f;
        private readonly int amountOfTilesOnScreen = 10;
        private readonly float safeZone = 30.0f;

        private int lastPlatformIndex = 0;
        private int collectableCount;

        private PlayerEngine currentPlayer;

        private float score;
        private float scoreModifier;

        private void Awake()
        {
            var levelParent = new GameObject("Level").transform;
            levelParent.SetParent(transform);

            PlatformParent = new GameObject("Platforms").transform;
            PlatformParent.SetParent(levelParent);

            OthersParent = new GameObject("Others").transform;
            OthersParent.SetParent(levelParent);
        }

        private void Start()
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
                    CreatePlatform(0);
                }
                else
                {
                    CreatePlatform();
                }
            }

            currentPlayer = ObjectPoolManager.Instance.SpawnObject(
                ResourceManager.Instance.PlayerPrefab,
                new Vector3(0, 0.6f, 0),
                Quaternion.identity,
                OthersParent).GetComponent<PlayerEngine>();
        }

        private void CreatePlatform(int platformIndex = -1)
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
                newPlatform =  ObjectPoolManager.Instance.SpawnObject (
                   ResourceManager.Instance.PlatformPrefabs[platformIndex],
                   Vector3.forward * spawnZ,
                   Quaternion.identity,
                   PlatformParent
               );          
            }
           
            spawnZ += platformLenght;
            createdPlatforms.Enqueue(newPlatform);
        }

        private void DestroyPlatform()
        {
            ObjectPoolManager.Instance.DespawnObject(createdPlatforms.Dequeue());
        }

        private void Update()
        {
            if (InputManager.Instance.FirstTouch && InputManager.Instance.IsPointerOverUI)
            {
                GameMaster.Instance.ChangeGameState(GAMESTATE.RUNNING);
            }

            if (!GameMaster.Instance.CurrentGamestate.Equals(GAMESTATE.RUNNING))
            {
                return;
            }

            if (currentPlayer == null)
                return;

            if(currentPlayer.PlayerPosition.z - safeZone > (spawnZ - amountOfTilesOnScreen * platformLenght))
            {
                CreatePlatform();
                DestroyPlatform();

                currentPlayer.IncreaseMovementSpeed();
            }
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
    }
}