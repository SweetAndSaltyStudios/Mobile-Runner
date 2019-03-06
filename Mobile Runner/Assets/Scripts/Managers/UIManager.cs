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
        public UIScreen StartingScreen;

        [Header("Scene start & end events")]
        public UnityEvent OnSceneStart = new UnityEvent();
        public UnityEvent OnSceneEnd = new UnityEvent();

        [Header("Screen events")]
        public UnityEvent OnScreenSwitched = new UnityEvent();

        private Material dissolveMaterial;
        private int dissolveParameterID;

        public Scrollbar AccelerationVisualBar;
        public TextMeshProUGUI CollectableCountText;
        public TextMeshProUGUI LoadingText;

        private CanvasGroup screenFadeImageCanvasGroup;
        public TextMeshProUGUI ScoreModifierText;

        private readonly float fadeDuration = 0.4f;
        private readonly float fakeLoadTime = 2f;
        private readonly float sceneStartTime = 1f;

        #endregion VARIABLES

        #region PROPERTIES

        public UIScreen CurrentUIScreen
        {
            get;
            private set;
        }

        public UIScreen PreviousUIScreen
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
            dissolveMaterial.SetFloat(dissolveParameterID, 0f);
            Invoke("OnStart", sceneStartTime);
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
        }

        public void UpdateCollectableCount(int newAmount)
        {
            CollectableCountText.text = newAmount.ToString();
        }

        public void UpdateScoreModifier(float newDistance)
        {
            ScoreModifierText.text = "SCORE: " + newDistance.ToString("0") + " X";
        }

        public void ScreenFade(float targetAlpha)
        {
            if (IsFading == false)
            {
                StartCoroutine(IScreenFadeShader(targetAlpha));
            }
        }

        private void OnStart()
        {
            OnSceneStart.Invoke();
            SwitchScreens(StartingScreen);
        }

        private void OnQuit()
        {
#if UNITY_EDITOR

            dissolveMaterial.SetFloat(dissolveParameterID, 1f);
            EditorApplication.isPlaying = false;

#else

            Application.Quit();

#endif
        }

        public void SwitchScreens(UIScreen newScreen)
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

                OnScreenSwitched.Invoke();
            }
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
            OnSceneEnd.Invoke();
            Invoke( "OnQuit", sceneStartTime);
        }

        public void CrossFade()
        {
            StartCoroutine(ICrossFade());
        }

        #endregion CUSTOM_FUNCTIONS

        #region COROUTINES

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

        private IEnumerator ICrossFade()
        {
            ScreenFade(0f);
            yield return new WaitWhile(() => IsFading);

            LoadingText.gameObject.SetActive(true);
            yield return new WaitForSeconds(fakeLoadTime);
            LoadingText.gameObject.SetActive(false);

            ScreenFade(1f);
            yield return new WaitWhile(() => IsFading);

        }

        #endregion COROUTINES
    }
}
