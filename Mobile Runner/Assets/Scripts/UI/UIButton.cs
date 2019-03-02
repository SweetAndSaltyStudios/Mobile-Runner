using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SweetAndSaltyStudios
{
    public class UIButton : UIElement
    {
        private Vector2 defaultIconScale;
        private Vector2 activeScale = new Vector2(1.1f, 1.1f);

        private Color defaultColor;
        private Color activeColor = Color.cyan;

        private Action buttonAction;

        private Image icon;
        private TextMeshProUGUI titleText;

        private void Awake()
        {
            icon = GetComponentInChildren<Image>();

            titleText = GetComponentInChildren<TextMeshProUGUI>();       
        }

        private void Start()
        {
            // Timing issue ...?
            buttonAction = UIManager.Instance.GetButtonAction(gameObject.name);

            defaultIconScale = transform.localScale;

            if (titleText)
                defaultColor = titleText.color;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (titleText)
                titleText.color = activeColor;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            icon.transform.localScale = activeScale;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            icon.transform.localScale = defaultIconScale;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (titleText)
                titleText.color = defaultColor;

            buttonAction();
        }
    }
}
