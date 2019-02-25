using System.Collections;
using UnityEngine;

namespace SweetAndSaltyStudios
{
    public class Collectable : MonoBehaviour
    {
        private Effect shatterEffect;
        private Transform graphics;
        private float rotationSpeed;

        private void Awake()
        {
            shatterEffect = Instantiate(ResourceManager.Instance.ShatterEffectPrefab, transform).GetComponent<Effect>();
            shatterEffect.gameObject.SetActive(false);
            graphics = transform.GetChild(0);
        }

        private void Start()
        {
            rotationSpeed = Random.Range(350, 450);
            transform.rotation = Quaternion.Euler(45, 45, 0);

            StartCoroutine(IAnimateGraphics());
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer.Equals(11))
            {
                LevelManager.Instance.AddCollectable(1);
                shatterEffect.transform.position += Vector3.forward;
                shatterEffect.transform.SetParent(null);
                shatterEffect.gameObject.SetActive(true);

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
