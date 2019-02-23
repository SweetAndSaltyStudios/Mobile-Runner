using UnityEngine;

namespace SweetAndSaltyStudios
{
    public class InputManager : Singelton<InputManager>
    {
        private bool isFlat = true;

        public float GetHorizontalAxis { get { return Input.GetAxis("Horizontal"); } }
        public float GetVerticalAxis { get { return Input.GetAxis("Vertical"); } }

        public Vector3 Tilt { get; private set; }

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

        private void Awake()
        {

        }
    }
}
