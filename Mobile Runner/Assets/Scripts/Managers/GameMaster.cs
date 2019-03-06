using UnityEngine;

namespace SweetAndSaltyStudios
{
    public class GameMaster : SingeltonPersistant<GameMaster>
    {
        #region VARIABLES



        #endregion VARIABLES

        #region PROPERTIES

        public float CurrentTimeScale
        {          
            get
            {
                return Time.timeScale;
            }
            private set
            {
                Time.timeScale = value;
            }
        }

        #endregion PROPERTIES

        #region UNITY_FUNCTIONS

        protected override void Awake()
        {
            Initialize();
        }

        #endregion UNITY_FUNCTIONS

        #region CUSTOM_FUNCTIONS

        private void Initialize()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        public void ChangeTimeScale(float newTimeScale)
        {
            CurrentTimeScale = newTimeScale;
        }

        #endregion CUSTOM_FUNCTIONS

        #region COROUTINES



        #endregion COROUTINES
    }
}