using UnityEngine;
using UnityEngine.EventSystems;

namespace SweetAndSaltyStudios
{
    public class InputManager : Singelton<InputManager>
    {
        #region VARIABLES

        private EventSystem currentEventSystem;
        private readonly bool isFlat = true;

        #endregion VARIABLES

        #region PROPERTIES

        public GameObject SetSelectedObject
        {
            set
            {
                currentEventSystem.SetSelectedGameObject(value);
            }               
        }
        public GameObject CurrentSelectable
        {
            get
            {
                return currentEventSystem.currentSelectedGameObject;
            }
        }

        public Vector3 Tilt
        {
            get;
            private set;
        }
        public float GetHorizontalAxis
        {
            get
            {
#if UNITY_EDITOR
                return Input.GetAxis("Horizontal");
#else
                return GetHorizontalAxisTilt;
#endif
            }
        }
        public float GetVerticalAxis
        {
            get
            {
                return Input.GetAxis("Vertical");
            }
        }
        public float GetHorizontalAxisTilt
        {
            get
            {
                Tilt = Input.acceleration;

                if (isFlat)
                {
                    Tilt = Quaternion.Euler(90, 0, 0) * Tilt;
                }

                return Tilt.x;
            }
        }
        public bool FirstTouch
        {
            get
            {
                return Input.GetMouseButtonDown(0);
            }
        }
        public bool IsPointerOverUI
        {
            get
            {
                return !currentEventSystem.IsPointerOverGameObject();
            }
        }

        #endregion PROPERTIES

        #region UNITY_FUNCTIONS

        private void Awake()
        {
            currentEventSystem = EventSystem.current;
        }

        #endregion UNITY_FUNCTIONS

        #region CUSTOM_FUNCTIONS
        #endregion CUSTOM_FUNCTIONS

        #region COROUTINES
        #endregion COROUTINES


    }
}
