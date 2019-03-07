using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SweetAndSaltyStudios
{
    public class UIButton : UIElement
    {
        public void ChangeBackgroundColor(Color newBackgroundColor)
        {
            icon.color = newBackgroundColor;
        }
    }
}
