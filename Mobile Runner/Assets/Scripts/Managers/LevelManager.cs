using System.Collections.Generic;
using UnityEngine;

namespace SweetAndSaltyStudios
{
    public class LevelManager : Singelton<LevelManager>
    {
        private Queue<GameObject> createdPlatforms = new Queue<GameObject>();

        private Transform levelParent;

        private float spawnZ = 8f;
        private readonly float platformLenght = 20f;
        private readonly int amountOfTilesOnScreen = 10;
        private readonly float safeZone = 20.0f;

        private int lastPlatformIndex = 0;
        private int collectableCount;

        private PlayerEngine currentPlayer;

        private void Awake()
        {
            levelParent = new GameObject("Level").transform;
            levelParent.SetParent(transform);
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

            currentPlayer = CreateModel(ResourceManager.Instance.PlayerPrefab, levelParent, new Vector3(0, 0.6f, 0)).GetComponent<PlayerEngine>();
        }

        private void CreatePlatform(int platformIndex = -1)
        {
            GameObject newPlatform;

            if (platformIndex == -1)
            {
                 newPlatform = CreateModel(
                   ResourceManager.Instance.PlatformPrefabs[RandomPlatformIndex()],
                   levelParent,
                   Vector3.forward * spawnZ
               );
            }
            else
            {
                newPlatform = CreateModel(
                   ResourceManager.Instance.PlatformPrefabs[platformIndex],
                   levelParent,
                   Vector3.forward * spawnZ
               );
            }
           
            spawnZ += platformLenght;
            createdPlatforms.Enqueue(newPlatform);
        }

        private void DestroyPlatform()
        {
            var platformToDestroy = createdPlatforms.Dequeue();
            Destroy(platformToDestroy);
        }

        private void Update()
        {
            if(currentPlayer.PlayerPosition.z - safeZone > (spawnZ - amountOfTilesOnScreen * platformLenght))
            {
                CreatePlatform();
                DestroyPlatform();
            }
        }

        private GameObject CreateModel(GameObject modelPrefab, Transform parent = null, Vector3 position = new Vector3(), Quaternion rotation = new Quaternion())
        {
            var newModelPrefab = Instantiate(modelPrefab, position, rotation, parent);
            newModelPrefab.name = modelPrefab.name;
            return newModelPrefab;
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

        public void AddCollectable(int amount)
        {
            collectableCount += amount;
            UIManager.Instance.UpdateCollectableCount(collectableCount);
        }
    }
}