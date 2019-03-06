using UnityEngine;
using UnityEngine.Events;

namespace SweetAndSaltyStudios
{
    public class UIScreen : MonoBehaviour
    {
        [Header("Screen properties")]
        public GameObject StartSelectableGameObject;
        public GameObject LastSelectableGameObject;

        [Header("Screen events")]
        public UnityEvent OnScreenOpen = new UnityEvent();
        public UnityEvent OnScreenClose = new UnityEvent();

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public virtual void OpenScreen()
        {
            OnScreenOpen.Invoke();

            HandelAnimator("Show");
        }

        public virtual void CloseScreen()
        {          
            HandelAnimator("Hide");
        }

        public void ScreenClose()
        {
            OnScreenClose.Invoke();
        }

        private void HandelAnimator(string animationTrigger)
        {
            if (animator)
            {
                animator.SetTrigger(animationTrigger);
            }
        }    
    }
}
