using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SweetAndSaltyStudios
{
    public class UIManager : Singelton<UIManager>
    {
        #region VARIABLES

        private Material dissolveMaterial;
        private int dissolveParameterID;

        private Action[] actions;

        public Scrollbar AccelerationVisualBar;
        public TextMeshProUGUI CollectableCountText;
        public TextMeshProUGUI LoadingText;

        private CanvasGroup screenFadeImageCanvasGroup;
        private TextMeshProUGUI scoreModifierText;

        private readonly float fadeDuration = 1f;
        private readonly float fakeLoadTime = 0.5f;

        #endregion VARIABLES

        #region PROPERTIES

        public MainMenuPanel MainMenuPanel
        {
            get;
            private set;
        }
        public PausePanel PausePanel
        {
            get;
            private set;
        }
        public GameOverPanel GameOverPanel
        {
            get;
            private set;
        }
        public HUDPanel HudPanel
        {
            get;
            private set;
        }

        public OptionsPanel OptionsPanel
        {
            get;
            private set;
        }
        public AudioPanel AudioPanel
        {
            get;
            private set;
        }
        public GraphicsPanel GraphicsPanel
        {
            get;
            private set;
        }
        public SocialPanel SocialPanel
        {
            get;
            private set;
        }
        public HowToPlayPanel HowToPlayPanel
        {
            get;
            private set;
        }

        public UIPanel CurrentPanel
        {
            get;
            private set;
        }
        public UIPanel PreviousPanel
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

            Invoke("OnStart", 1f);

            ChangePanel(MainMenuPanel);
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
            var panels = canvas.Find("Panels");

            MainMenuPanel = panels.Find("MainMenuPanel").GetComponent<MainMenuPanel>();
            PausePanel = panels.Find("PausePanel").GetComponent<PausePanel>();
            GameOverPanel = panels.Find("GameOverPanel").GetComponent<GameOverPanel>();

            OptionsPanel = panels.Find("OptionsPanel").GetComponent<OptionsPanel>();
            AudioPanel = panels.Find("AudioPanel").GetComponent<AudioPanel>();
            GraphicsPanel = panels.Find("GraphicsPanel").GetComponent<GraphicsPanel>();
            SocialPanel = panels.Find("SocialPanel").GetComponent<SocialPanel>();
            HowToPlayPanel = panels.Find("HowToPlayPanel").GetComponent<HowToPlayPanel>();
           
            HudPanel = panels.Find("HUDPanel").GetComponent<HUDPanel>();

            scoreModifierText = HudPanel.transform.Find("InfoContainer").Find("ScoreModifierText").GetComponent<TextMeshProUGUI>();

            dissolveMaterial = canvas.Find("ScreenFadeImage").GetComponent<Image>().material;
            dissolveParameterID = Shader.PropertyToID("_Amount");

            screenFadeImageCanvasGroup = canvas.GetComponent<CanvasGroup>();

            actions = new Action[]
          {
                new Action(AudioButton),
                new Action(GraphicsButton),
                new Action(HowToPlayButton),
                new Action(PauseButton),
                new Action(StartNewGameButton),
                new Action(OptionsButton),
                new Action(SocialButton),
                new Action(QuitButton),
                new Action(BackButton),
                new Action(RestartButton)
          };         
        }

        public Action GetButtonAction(string actionName)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                if (actions[i].Method.Name == actionName)
                {
                    return actions[i];
                }
            }

            Debug.LogError("!!!");
            return null;
        }

        public void ChangePanel(UIPanel newPanel)
        {
            if (CurrentPanel != null)
            {
                PreviousPanel = CurrentPanel;

                CurrentPanel.Close();
            }

            CurrentPanel = newPanel;

            CurrentPanel.Open();
        }

        public void UpdateCollectableCount(int newAmount)
        {
            CollectableCountText.text = newAmount.ToString();
        }

        public void UpdateScoreModifier(float newDistance)
        {
            scoreModifierText.text = "SCORE: " + newDistance.ToString("0") + " X";
        }

        //public void ScreenFade(float targetAlpha)
        //{
        //    if (IsFading == false)
        //    {
        //        StartCoroutine(IScreenFade(targetAlpha));
        //    }
        //}

        private void ScreenFadeShader(float targetAlpha)
        {
            if (IsFading == false)
            {
                StartCoroutine(IScreenFadeShader(targetAlpha));
            }
        }

        public void FadeInAndOut(bool startFadingToBlack, Func<bool> functionBetweenFades = null)
        {
            functionBetweenFades = functionBetweenFades ?? (() => true); 

            if (IsFading == false)
            {
                StartCoroutine(IFadeScreeInAndOut(startFadingToBlack, functionBetweenFades));
            }
        }

        private void OnStart()
        {
            ScreenFadeShader(1f);
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

        private IEnumerator IFadeScreeInAndOut(bool startFadingToBlack, Func<bool> functionBetweenFades)
        {
            ScreenFadeShader(startFadingToBlack ? 0f : 1f);

            yield return new WaitWhile(() => IsFading);

            LoadingText.gameObject.SetActive(true);

            yield return new WaitUntil(functionBetweenFades);
            
            yield return new WaitForSeconds(fakeLoadTime);

            LoadingText.gameObject.SetActive(false);
            ScreenFadeShader(startFadingToBlack ? 1f : 0f);
        }

        //private IEnumerator IScreenFade(float targetAlpha)
        //{
        //    IsFading = true;
        //    ScreenFadeImageCanvasGroup.blocksRaycasts = true;

        //    var fadeSpeed = Mathf.Abs(ScreenFadeImageCanvasGroup.alpha - targetAlpha) / fadeDuration;

        //    while (!Mathf.Approximately(ScreenFadeImageCanvasGroup.alpha, targetAlpha))
        //    {
        //        ScreenFadeImageCanvasGroup.alpha = Mathf.MoveTowards(ScreenFadeImageCanvasGroup.alpha, targetAlpha, fadeSpeed * Time.unscaledDeltaTime);
        //        yield return null;
        //    }

        //    IsFading = false;
        //    ScreenFadeImageCanvasGroup.blocksRaycasts = false;
        //}

        #endregion COROUTINES

        #region BUTTON_ACTIONS

        private void AudioButton()
        {
            Instance.ChangePanel(Instance.AudioPanel);
        }

        private void GraphicsButton()
        {
            Instance.ChangePanel(GraphicsPanel);
        }

        private void HowToPlayButton()
        {
            Instance.ChangePanel(Instance.HowToPlayPanel);
        }

        private void PauseButton()
        {
            ChangePanel(PausePanel);
        }

        private void StartNewGameButton()
        {
            LevelManager.Instance.StartGame();
        }

        private void OptionsButton()
        {
            ChangePanel(Instance.OptionsPanel);
        }

        private void SocialButton()
        {
           ChangePanel(Instance.SocialPanel);
        }

        private void QuitButton()
        {
            ScreenFadeShader(0f);

            Invoke("OnQuit", 1f);
        }   

        private void BackButton()
        {
            if (CurrentPanel.Equals(PausePanel))
            {
                LevelManager.Instance.EndGame(MainMenuPanel);
            }
            else if (CurrentPanel.Equals(GameOverPanel))
            {
                ChangePanel(MainMenuPanel);
            }
            else if (CurrentPanel.Equals(OptionsPanel))
            {
                ChangePanel(MainMenuPanel);
            }
            else
            {
                ChangePanel(PreviousPanel);
            }
        }

        private void RestartButton()
        {
            LevelManager.Instance.StartGame();
        }

        #endregion BUTTON_ACTIONS
    }
}
