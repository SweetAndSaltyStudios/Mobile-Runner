using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace SweetAndSaltyStudios
{
    public class UIManager : Singelton<UIManager>
    {
        #region VARIABLES

        [Header("Main variables")]
        public BaseUIScreen StartingScreen;

        private Material dissolveMaterial;
        private int dissolveParameterID;

        public Scrollbar AccelerationVisualBar;
        public TextMeshProUGUI CollectableCountText;
        public TextMeshProUGUI LoadingText;

        private CanvasGroup screenFadeImageCanvasGroup;
        public TextMeshProUGUI ScoreModifierText;

        private readonly float fadeDuration = 0.4f;
        private readonly float fakeLoadTime = 0.5f;

        #endregion VARIABLES

        #region PROPERTIES

        public BaseUIScreen CurrentUIScreen
        {
            get;
            private set;
        }

        public BaseUIScreen PreviousUIScreen
        {
            get;
            private set;
        }

        public bool IsFading
        {
            get;
            private set;
        }

        #endregion PROPERTIES

        #region UNITY_FUNCTIONS

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            StartCoroutine(IStartScene());
        }

        private void Update()
        {
            var movementDirection = 0f;

            movementDirection = InputManager.Instance.GetHorizontalAxis;

            AccelerationVisualBar.value = Mathf.Lerp(AccelerationVisualBar.value, movementDirection + 0.5f, Time.deltaTime);
        }

        #endregion UNITY_FUNCTIONS

        #region CUSTOM_FUNCTIONS

        private void Initialize()
        {
            var canvas = transform.GetChild(0);
            var UIScreenContainer = canvas.Find("UIScreenContainer");

            dissolveMaterial = canvas.Find("ScreenFadeImage").GetComponent<Image>().material;
            dissolveParameterID = Shader.PropertyToID("_Amount");

            screenFadeImageCanvasGroup = canvas.GetComponent<CanvasGroup>();

            var uiScreens = FindObjectsOfType<BaseUIScreen>();

            for (int i = 0; i < uiScreens.Length; i++)
            {
                uiScreens[i].gameObject.SetActive(false);
            }
        }

        public void UpdateCollectableCount(int newAmount)
        {
            CollectableCountText.text = newAmount.ToString();
        }

        public void UpdateScoreModifier(float newDistance)
        {
            ScoreModifierText.text = "SCORE: " + newDistance.ToString("0") + " X";
        }

        private void ScreenFade(float targetAlpha)
        {
            if (IsFading == false)
            {
                StartCoroutine(IScreenFadeShader(targetAlpha));
            }
        }

        public void SwitchScreens(BaseUIScreen newScreen)
        {
            StartCoroutine(ISwitchScreens(newScreen));
        }

        public void SwitchPreviousScreen()
        {
            if (PreviousUIScreen)
            {
                SwitchScreens(PreviousUIScreen);
            }
        }

        public void QuitGame()
        {
            StartCoroutine(IQuit());
        }

        public void CrossFade(Func<bool> func = null)
        {
            if (func == null)
            {
                func = () => true;
            }

            if (IsFading == false)
            {
                StartCoroutine(ICrossFade(func));
            }
        }

        #endregion CUSTOM_FUNCTIONS

        #region COROUTINES

        private IEnumerator ISwitchScreens(BaseUIScreen newScreen)
        {
            if (newScreen)
            {
                if (CurrentUIScreen)
                {
                    CurrentUIScreen.CloseScreen();
                    PreviousUIScreen = CurrentUIScreen;
                }

                CurrentUIScreen = newScreen;
                CurrentUIScreen.OpenScreen();

                yield return null;
            }
        }

        private IEnumerator IScreenFadeShader(float targetAlpha)
        {
            screenFadeImageCanvasGroup.blocksRaycasts = false;
            IsFading = true;

            var currentValue = dissolveMaterial.GetFloat(dissolveParameterID);
            var fadeSpeed = Mathf.Abs(currentValue - targetAlpha) / fadeDuration;

            while (!Mathf.Approximately(currentValue, targetAlpha))
            {
                currentValue = Mathf.MoveTowards(currentValue, targetAlpha, fadeSpeed * Time.unscaledDeltaTime);
                dissolveMaterial.SetFloat(dissolveParameterID, currentValue);
                yield return null;
            }

            IsFading = false;
            screenFadeImageCanvasGroup.blocksRaycasts = true;
        }

        private IEnumerator ICrossFade(Func<bool> func)
        {
            ScreenFade(0f);
            yield return new WaitWhile(() => IsFading);

            LoadingText.gameObject.SetActive(true);

            yield return new WaitUntil(func);

            yield return new WaitForSeconds(fakeLoadTime);
            LoadingText.gameObject.SetActive(false);

            ScreenFade(1f);
            yield return new WaitWhile(() => IsFading);
        }

        private IEnumerator IStartScene()
        {
            dissolveMaterial.SetFloat(dissolveParameterID, 0f);

            LoadingText.gameObject.SetActive(true);

            yield return new WaitForSeconds(fakeLoadTime);

            LoadingText.gameObject.SetActive(false);

            ScreenFade(1);

            yield return new WaitWhile(() => IsFading);

            SwitchScreens(StartingScreen);
        }

        private IEnumerator IQuit()
        {     
            CurrentUIScreen.CloseScreen();

            ScreenFade(0);

            yield return new WaitWhile(() => IsFading);

            LoadingText.text = "GOODBYE!";
            LoadingText.gameObject.SetActive(true);

            yield return new WaitForSeconds(fakeLoadTime);

#if UNITY_EDITOR

            dissolveMaterial.SetFloat(dissolveParameterID, 1f);
            EditorApplication.isPlaying = false;

#else

            Application.Quit();

#endif
        }

        #endregion COROUTINES
    }
}
