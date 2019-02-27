using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum UI_STATE
{
    MAIN_MENU,
    PAUSE,
    GAME_OVER,
    RUNNING
}

namespace SweetAndSaltyStudios
{
    public class UIManager : Singelton<UIManager>
    {
        public Scrollbar AccelerationVisualBar;
        public TextMeshProUGUI CollectableCountText;

        public CanvasGroup ScreenFadeImageCanvasGroup;

        public float FadeDuration;

        public GameObject MainMenuPanel;
        public GameObject PausePanel;
        public GameObject GameOverPanel;
        public GameObject HUD;

        private TextMeshProUGUI scoreModifierText;

        private bool isPaused;
        private bool isFading;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            var canvas = transform.GetChild(0);
            MainMenuPanel = canvas.Find("MainMenuPanel").gameObject;
            PausePanel = canvas.Find("PausePanel").gameObject;
            GameOverPanel = canvas.Find("GameOverPanel").gameObject;
            HUD = canvas.Find("HUD").gameObject;

            scoreModifierText = HUD.transform.Find("ScoreModifierText").GetComponent<TextMeshProUGUI>();

            ScreenFadeImageCanvasGroup = canvas.Find("ScreenFadeImage").GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            ScreenFadeImageCanvasGroup.alpha = 1f;
            ScreenFade(0f);
        }

        private void Update()
        {
            var movementDirection = 0f;

#if UNITY_EDITOR

              movementDirection = InputManager.Instance.GetHorizontalAxis;
#else
              movementDirection = InputManager.Instance.GetHorizontalAxisTilt;
#endif

            AccelerationVisualBar.value = Mathf.Lerp(AccelerationVisualBar.value, movementDirection + 0.5f, Time.deltaTime);
        }

        public void ChangeUIState(UI_STATE newUIState)
        {

        }

        public void UpdateCollectableCount(int newAmount)
        {
            CollectableCountText.text = newAmount.ToString();
        }

        public void UpdateScoreModifier(float newDistance)
        {
            scoreModifierText.text = "SCORE: " + newDistance.ToString("0") + " X";
        }

        private void ChangeUIState()
        {

        }

        private void OnPaused()
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                PausePanel.SetActive(true);
                
                Time.timeScale = 0f;
            }
            else
            {
                PausePanel.SetActive(false);
                Time.timeScale = 1f;
            }
        }

        private void OnQuit()
        {
#if UNITY_EDITOR

            EditorApplication.isPlaying = false;

#else

            Application.Quit();

#endif
        }

        public void PauseButton()
        {
            OnPaused();
        }

        public void QuitButton()
        {
            OnQuit();
        }

        public void ScreenFade(float targetAlpha)
        {
            if(isFading == false)
            {
                StartCoroutine(IScreenFade(targetAlpha));
            }
        }

        private IEnumerator IScreenFade(float targetAlpha)
        {
            isFading = true;
            ScreenFadeImageCanvasGroup.blocksRaycasts = true;

            var fadeSpeed = Mathf.Abs(ScreenFadeImageCanvasGroup.alpha - targetAlpha) / FadeDuration;

            while(!Mathf.Approximately(ScreenFadeImageCanvasGroup.alpha, targetAlpha))
            {
                ScreenFadeImageCanvasGroup.alpha = Mathf.MoveTowards(ScreenFadeImageCanvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
                yield return null;
            }

            isFading = false;
            ScreenFadeImageCanvasGroup.blocksRaycasts = false;
        }
    }
}
