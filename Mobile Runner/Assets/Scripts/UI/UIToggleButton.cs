using UnityEngine;

namespace SweetAndSaltyStudios
{
    public class UIToggleButton : UIElement
    {
        [HideInInspector]
        public Sprite toggleOn, toggleOff;

        public bool CurrentToggleState { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            icon.sprite = toggleOn;
        }

        public void SetToggleState(bool state)
        {
            CurrentToggleState = state;

            if (state)
            {
                icon.sprite = toggleOn;
            }
            else
            {
                icon.sprite = toggleOff;
            }
        }
    }
}
