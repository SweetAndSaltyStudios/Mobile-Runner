using UnityEngine;

namespace SweetAndSaltyStudios
{
    public class GameMaster : SingeltonPersistant<GameMaster>
    {
        #region VARIABLES



        #endregion VARIABLES

        #region PROPERTIES

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

        #endregion CUSTOM_FUNCTIONS

        #region COROUTINES



        #endregion COROUTINES
    }
}