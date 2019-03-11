using System.Collections;
using UnityEngine;

namespace SweetAndSaltyStudios
{
    public class Collectable : MonoBehaviour
    {
        private Transform graphics;
        private float rotationSpeed;

        private Coroutine iAnimateGraphics;
        private Vector3 originalPosition;

        private void Awake()
        {
            graphics = transform.GetChild(0);
            originalPosition = transform.position;
        }

        private void OnEnable()
        {
            if(iAnimateGraphics == null)
            {
                iAnimateGraphics = StartCoroutine(IAnimateGraphics());
            }

            transform.position = originalPosition;
        }

        private void OnDisable()
        {
            iAnimateGraphics = null;
        }

        private void Start()
        {
            rotationSpeed = Random.Range(350, 450);
            transform.rotation = Quaternion.Euler(45, 45, 0);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer.Equals(11))
            {
                GameManager.Instance.AddCollectable(1);

                AudioManager.Instance.PlaySfxAtPoint("Collect", transform.position);

                ObjectPoolManager.Instance.SpawnObject(ResourceManager.Instance.ShatterEffectPrefab, transform.position += Vector3.forward, Quaternion.identity, null);

                gameObject.SetActive(false);
            }
        }

        private IEnumerator IAnimateGraphics()
        {
            graphics.transform.localScale = Vector3.Lerp(graphics.transform.localScale, Vector3.one * 0.5f, Time.deltaTime);

            while (gameObject.activeSelf)
            {
                graphics.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
                yield return null;
            }           
        }
    }
}
