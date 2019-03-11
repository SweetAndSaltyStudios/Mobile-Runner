using UnityEngine;

namespace SweetAndSaltyStudios
{
    public abstract class BaseUIScreen : MonoBehaviour
    {
        private RectTransform startPosition;
        private RectTransform endPosition;


        protected virtual void Awake()
        {
           
        }

        protected virtual void OnEnable()
        {
           
        }

        protected virtual void OnDisable()
        {

        }

        public virtual void OpenScreen()
        {
            gameObject.SetActive(true);
        }

        public virtual void CloseScreen()
        {
            gameObject.SetActive(false);
        }

        public void ScreenClose()
        {
           
        }

        private void HandelAnimator(string animationTrigger)
        {
            
        }    
    }
}
