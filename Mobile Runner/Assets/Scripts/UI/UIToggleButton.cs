using UnityEngine;

namespace SweetAndSaltyStudios
{
    public class UIToggleButton : UIButton
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

            icon.sprite = CurrentToggleState ? toggleOn : toggleOff;
        }
    }
}
