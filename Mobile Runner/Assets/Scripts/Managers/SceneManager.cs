using System.Collections;
using UnityEngine;

namespace SweetAndSaltyStudios
{
    public class SceneManager : Singelton<SceneManager>
    {
        #region VARIABLES

        private bool isLoading;

        #endregion VARIABLES

        #region PROPERTIES

        public int CurrentSceneIndex
        {
            get
            {
                return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            }
        }

        public int NextSceneIndex
        {
            get
            {
                var nextSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1;

                return nextSceneIndex >= UnityEngine.SceneManagement.SceneManager.sceneCount ? 0 : nextSceneIndex;
            }
        }

        #endregion PROPERTIES

        #region UNITY_FUNCTIONS

        #endregion UNITY_FUNCTIONS

        #region CUSTOM_FUNCTIONS

        private void LoadScene(int sceneIndex)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
        }

        public void RestartScene()
        {
            if (isLoading == false)
            {
                StartCoroutine(ILoadScene());
            }
        }

        #endregion CUSTOM_FUNCTIONS

        #region COROUTINES

        private IEnumerator ILoadScene()
        {
            isLoading = true;

            yield return new WaitWhile(() => GameManager.Instance.IsSlowingTime);
            yield return new WaitWhile(() => UIManager.Instance.IsFading);

            LoadScene(CurrentSceneIndex);

            isLoading = false;
        }

        #endregion COROUTINES
    }
}
