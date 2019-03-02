using System.Collections;
using UnityEngine;

public enum GAME_STATE
{
    MAIN_MENU,
    IN_GAME,
    PAUSED,
    GAME_OVER
}

namespace SweetAndSaltyStudios
{
    public class GameMaster : SingeltonPersistant<GameMaster>
    {
        #region VARIABLES



        #endregion VARIABLES

        #region PROPERTIES

        public GAME_STATE CurrentGameState
        {
            get;
            private set;
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

        public void ChangeGameState(GAME_STATE newGameState)
        {
            CurrentGameState = newGameState;

            switch (CurrentGameState)
            {
                case GAME_STATE.MAIN_MENU:

                    break;

                case GAME_STATE.IN_GAME:

                    break;

                case GAME_STATE.PAUSED:

                    break;

                case GAME_STATE.GAME_OVER:

                    break;

                default:

                    break;
            }
        }

        #endregion CUSTOM_FUNCTIONS

        #region COROUTINES



        #endregion COROUTINES
    }
}