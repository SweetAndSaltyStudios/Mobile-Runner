using UnityEditor;
using UnityEngine;

namespace SweetAndSaltyStudios
{
    public class MainMenuPanel : UIPanel
    {
        public override void Open()
        {
            base.Open();

            UIManager.Instance.ScreenFadeShader(1f);
        }
    }
}
