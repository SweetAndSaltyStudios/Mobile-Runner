using TMPro;
using UnityEngine;

namespace SweetAndSaltyStudios
{
    public class GameMaster : SingeltonPersistant<GameMaster>
    {
        #region VARIABLES

        public TextMeshProUGUI GameStateText;

        #endregion VARIABLES

        #region PROPERTIES

        #endregion PROPERTIES

        #region UNITY_FUNCTIONS

        protected override void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            GameStateText.text = "Current Game State: " + GameManager.Instance.CurrentGameState.ToString();
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