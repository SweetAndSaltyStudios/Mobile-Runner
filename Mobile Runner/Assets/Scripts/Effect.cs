using System.Collections;
using UnityEngine;

public class Effect : MonoBehaviour
{
    private Coroutine iLifeTime;
    private new ParticleSystem particleSystem;

    private void Awake()
    {
        particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        LifeTime();
    }

    private void OnDisable()
    {
        iLifeTime = null;
    }

    public void ChangeColor(Color newColor)
    {
        var main = particleSystem.main;
        main.startColor = newColor;
    }

    private void LifeTime()
    {
        if(iLifeTime == null)
        {
           iLifeTime = StartCoroutine(ILifeTime());
        }
    }

    private IEnumerator ILifeTime()
    {
        yield return new WaitUntil(() => particleSystem.isPlaying.Equals(false));

        ObjectPoolManager.Instance.DespawnObject(gameObject);
    }
}
