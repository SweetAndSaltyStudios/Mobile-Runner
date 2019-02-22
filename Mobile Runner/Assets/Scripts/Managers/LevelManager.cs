using System.Collections;
using UnityEngine;

public class LevelManager : Singelton<LevelManager>
{
    public GameObject PlayerPrefab;
    public GameObject[] PlatformPrefabs;

    private PlayerEngine currentPlayer;

    private readonly int platformCount = 6;

    private float createdPlatforms = 0;
    private float zLocation = 0;

    private void Start()
    {
        StartCoroutine(StartLevel());
    }

    private GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        var newObject = Instantiate(prefab, position, rotation, parent);
        newObject.name = prefab.name;
        return newObject;
    }

    private IEnumerator StartLevel()
    {
        currentPlayer = SpawnObject(PlayerPrefab, new Vector3(0, 1, 1), Quaternion.identity, transform).GetComponent<PlayerEngine>();

        

        while (createdPlatforms < platformCount)
        {
            SpawnObject(PlatformPrefabs[Random.Range(0, PlatformPrefabs.Length)], new Vector3(0, 0.25f, zLocation), Quaternion.identity, transform);

            zLocation += 20;
            createdPlatforms++;

            if(currentPlayer.transform.position.z >= zLocation)
            {

            }

            yield return null;
        }

        yield return null;
    }

}
