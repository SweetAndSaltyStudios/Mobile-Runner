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

        public Image ScreenFadeImage;
        private Material dissolveMaterial;
        private int dissolveParameterID;

        private Action[] actions;

        public Sprite[] PauseButtonSprites;

        public Scrollbar AccelerationVisualBar;
        public TextMeshProUGUI CollectableCountText;

        public CanvasGroup ScreenFadeImageCanvasGroup;

        public MainMenuPanel MainMenuPanel;
        public PausePanel PausePanel;
        public GameOverPanel GameOverPanel;
        public HUDPanel HUDPanel;

        public OptionsPanel OptionsPanel;
        public AudioPanel AudioPanel;
        public GraphicsPanel GraphicsPanel;
        public SocialPanel SocialPanel;
        public HowToPlayPanel HowToPlayPanel;

        private TextMeshProUGUI scoreModifierText;

        private readonly float fadeDuration = 2f;

        #endregion VARIABLES

        #region PROPERTIES

        public UIPanel CurrentPanel
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


            //ScreenFadeImageCanvasGroup.alpha = 1f;
            //ScreenFade(0f);

            ChangePanel(MainMenuPanel);
        }

        public void ChangePanel(UIPanel newPanel)
        {
            if(CurrentPanel != null)
            {
                CurrentPanel.Close();
            }

            CurrentPanel = newPanel;

            CurrentPanel.Open();
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
           
            HUDPanel = panels.Find("HUDPanel").GetComponent<HUDPanel>();

            scoreModifierText = HUDPanel.transform.Find("InfoContainer").Find("ScoreModifierText").GetComponent<TextMeshProUGUI>();

            ScreenFadeImage = canvas.Find("ScreenFadeImage").GetComponent<Image>();

            dissolveMaterial = ScreenFadeImage.GetComponent<Image>().material;
            dissolveParameterID = Shader.PropertyToID("_Amount");

            // !!!
            ScreenFadeImage.enabled = false;

            ScreenFadeImageCanvasGroup = canvas.GetComponent<CanvasGroup>();

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

        public void UpdateCollectableCount(int newAmount)
        {
            CollectableCountText.text = newAmount.ToString();
        }

        public void UpdateScoreModifier(float newDistance)
        {
            scoreModifierText.text = "SCORE: " + newDistance.ToString("0") + " X";
        }

        public void ScreenFade(float targetAlpha)
        {
            if (IsFading == false)
            {
                StartCoroutine(IScreenFade(targetAlpha));
            }
        }

        public void ScreenFadeShader(float targetAlpha)
        {
            dissolveMaterial.SetFloat(dissolveParameterID, 0f);

            if (IsFading == false)
            {
                StartCoroutine(IScreenFadeShader(targetAlpha));
            }
        }

        #endregion CUSTOM_FUNCTIONS

        #region COROUTINES

        private IEnumerator IScreenFadeShader(float targetAlpha)
        {
            IsFading = true;
            ScreenFadeImageCanvasGroup.blocksRaycasts = false;
            ScreenFadeImage.enabled = true;

            var currentValue = dissolveMaterial.GetFloat(dissolveParameterID);
            var fadeSpeed = Mathf.Abs(currentValue - targetAlpha) / fadeDuration;

            while (!Mathf.Approximately(currentValue, targetAlpha))
            {
                currentValue = Mathf.MoveTowards(currentValue, targetAlpha, fadeSpeed * Time.unscaledDeltaTime);
                dissolveMaterial.SetFloat(dissolveParameterID, currentValue);
                yield return null;
            }

            IsFading = false;
            ScreenFadeImageCanvasGroup.blocksRaycasts = true;
            ScreenFadeImage.enabled = false;
        }

        private IEnumerator IScreenFade(float targetAlpha)
        {
            IsFading = true;
            ScreenFadeImageCanvasGroup.blocksRaycasts = true;

            var fadeSpeed = Mathf.Abs(ScreenFadeImageCanvasGroup.alpha - targetAlpha) / fadeDuration;

            while (!Mathf.Approximately(ScreenFadeImageCanvasGroup.alpha, targetAlpha))
            {
                ScreenFadeImageCanvasGroup.alpha = Mathf.MoveTowards(ScreenFadeImageCanvasGroup.alpha, targetAlpha, fadeSpeed * Time.unscaledDeltaTime);
                yield return null;
            }

            IsFading = false;
            ScreenFadeImageCanvasGroup.blocksRaycasts = false;
        }

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
            LevelManager.Instance.StartNewGame();
            ChangePanel(Instance.HUDPanel);
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
#if UNITY_EDITOR

            EditorApplication.isPlaying = false;

#else

            Application.Quit();

#endif
        }

        private void BackButton()
        {
            // !!!
            if (CurrentPanel.Equals(PausePanel))
            {
                LevelManager.Instance.ClearLevel();
            }

            ChangePanel(MainMenuPanel);
        }

        private void RestartButton()
        {
            LevelManager.Instance.StartNewGame();
            ChangePanel(Instance.HUDPanel);
        }

        #endregion BUTTON_ACTIONS
    }
}
