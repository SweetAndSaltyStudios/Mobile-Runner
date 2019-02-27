using UnityEngine;
using UnityEngine.EventSystems;

namespace SweetAndSaltyStudios
{
    public class InputManager : Singelton<InputManager>
    {
        private readonly bool isFlat = true;

        public float GetHorizontalAxis
        {
            get
            {
                return Input.GetAxis("Horizontal");
            }
        }
        public float GetVerticalAxis
        {
            get
            {
                return Input.GetAxis("Vertical");
            }
        }

        public Vector3 Tilt
        {
            get;
            private set;
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
                Debug.LogError(!EventSystem.current.IsPointerOverGameObject());
                return !EventSystem.current.IsPointerOverGameObject();
            }
        }
    }
}
