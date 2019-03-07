using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SweetAndSaltyStudios
{
    public abstract class UIElement : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler, IPointerEnterHandler
    {
        public UnityEvent ButtonEvent = new UnityEvent();

        protected Image backgroundImage, icon;
        protected Vector3 normalScale = Vector3.one;
        protected Vector3 activeScale = new Vector3(1.2f, 1.2f, 1);
        protected Color normalColor;
        protected Color disableColor;

        protected bool isActive = true;
        private bool isEffectRunning = false;
        private readonly float effectSpeed = 20f;
        private readonly float effectDuration = 4f;

        public bool IsDisabled()
        {
            return isActive ? false : true;
        }

        protected virtual void Awake()
        {
            backgroundImage = transform.Find("Button_Background").GetComponent<Image>();
            icon = backgroundImage.transform.Find("Button_Icon").GetComponent<Image>();

            normalColor = backgroundImage.color;
            disableColor = Color.grey;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isActive)
            {
                ButtonEvent.Invoke();
                backgroundImage.transform.localScale = normalScale;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            eventData.pointerPress = null;
            backgroundImage.transform.localScale = normalScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isEffectRunning)
            {
                isEffectRunning = false;
            }

            if (isActive)
            {
                eventData.pointerPress = gameObject;
                backgroundImage.transform.localScale = activeScale;
            }
        }

        public void IsActive(bool isActive)
        {
            if (isActive)
            {
                this.isActive = true;
                backgroundImage.color = normalColor;
            }
            else
            {
                this.isActive = false;
                backgroundImage.color = disableColor;
            }
        }

        public void StopEffect()
        {
            if (isEffectRunning)
            {
                isEffectRunning = false;
            }
        }

        public void StartScaleEffect(LEVEL_STATE levelState)
        {
            isEffectRunning = true;
            StartCoroutine(StartEffect(levelState));
        }

        private IEnumerator StartEffect(LEVEL_STATE levelState)
        {
            while (LevelManager.Instance.CurrentLevelState.Equals(levelState) && isEffectRunning)
            {
                yield return RepeatLerp(normalScale, activeScale, effectDuration);
                yield return RepeatLerp(activeScale, normalScale, effectDuration);
            }
        }

        private IEnumerator RepeatLerp(Vector3 startScale, Vector3 endScale, float duration)
        {
            float i = 0f;
            float rate = (1f / duration) * effectSpeed;

            while (i < 1.0f)
            {
                i += Time.deltaTime * rate;

                backgroundImage.transform.localScale = Vector3.Lerp(startScale, endScale, i);

                yield return null;
            }
        }
    }
}