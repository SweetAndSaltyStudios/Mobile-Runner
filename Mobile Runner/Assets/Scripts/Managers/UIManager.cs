using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SweetAndSaltyStudios
{
    public class UIManager : Singelton<UIManager>
    {
        public Scrollbar AccelerationVisualBar;
        public TextMeshProUGUI CollectableCountText;

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

        public void UpdateCollectableCount(int newAmount)
        {
            CollectableCountText.text = newAmount.ToString();
        }

        public void QuitButton()
        {
#if UNITY_EDITOR

            EditorApplication.isPlaying = false;

#else

        Application.Quit();

#endif
        }
    }
}
