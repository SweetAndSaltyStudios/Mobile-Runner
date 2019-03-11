using UnityEngine;
using UnityEngine.EventSystems;

namespace SweetAndSaltyStudios
{
    public abstract class UIElement : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler, IPointerEnterHandler
    {
        protected virtual void Awake()
        {

        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {

        }
        public virtual void OnPointerExit(PointerEventData eventData)
        {

        }
        public virtual void OnPointerDown(PointerEventData eventData)
        {

        }
        public virtual void OnPointerUp(PointerEventData eventData)
        {

        }
    }
}