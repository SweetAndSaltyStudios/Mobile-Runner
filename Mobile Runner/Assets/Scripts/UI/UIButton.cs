using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SweetAndSaltyStudios
{
    public class UIButton : UIElement
    {
        protected Image backgroundImage, icon;
        protected Vector3 normalScale = Vector3.one;
        protected Vector3 activeScale = new Vector3(1.2f, 1.2f, 1);
        protected Color normalColor;
        protected Color disableColor;

        public UnityEvent ButtonEvent = new UnityEvent();

        protected bool isActive = true;
        private bool isEffectRunning = false;
        private readonly float effectSpeed = 20f;
        private readonly float effectDuration = 4f;

        protected override void Awake()
        {
            base.Awake();

            backgroundImage = transform.Find("Button_Background").GetComponent<Image>();
            icon = backgroundImage.transform.Find("Button_Icon").GetComponent<Image>();

            normalColor = backgroundImage.color;
            disableColor = Color.grey;
        }

        public void ChangeBackgroundColor(Color newBackgroundColor)
        {
            icon.color = newBackgroundColor;
        }

        public void IsActive(bool isActive)
        {
            this.isActive = isActive;
            backgroundImage.color = this.isActive ? normalColor : disableColor;
        }

        public bool IsDisabled()
        {
            return isActive ? false : true;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

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
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            eventData.pointerPress = null;
            backgroundImage.transform.localScale = normalScale;
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (isActive)
            {
                ButtonEvent.Invoke();
                backgroundImage.transform.localScale = normalScale;
            }
        }

        public void StopEffect()
        {
            if (isEffectRunning)
            {
                isEffectRunning = false;
            }
        }
        public void StartScaleEffect(GAME_STATE levelState)
        {
            isEffectRunning = true;
            StartCoroutine(StartEffect(levelState));
        }

        private IEnumerator StartEffect(GAME_STATE levelState)
        {
            while (GameManager.Instance.CurrentGameState.Equals(levelState) && isEffectRunning)
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
