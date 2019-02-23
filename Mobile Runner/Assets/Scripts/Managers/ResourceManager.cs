using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singelton<ResourceManager>
{
    [Header("Model prefabs")]
    public GameObject PlayerPrefab;

    [Header("Platform prefabs")]
    public GameObject[] PlatformPrefabs;
}
