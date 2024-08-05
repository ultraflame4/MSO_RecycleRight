using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class UIPointerHandler : 
        MonoBehaviour, 
        IPointerEnterHandler, 
        IPointerExitHandler, 
        IPointerDownHandler
    {
        public event Action PointerEnter, PointerExit, PointerDown;
        bool pointer_enter, pointer_exit, pointer_down = false;

        void LateUpdate()
        {
            pointer_enter = HandleEvent(PointerEnter, pointer_enter);
            pointer_exit = HandleEvent(PointerExit, pointer_exit);
            pointer_down = HandleEvent(PointerDown, pointer_down);
        }

        bool HandleEvent(Action callback, bool check)
        {
            if (check) callback?.Invoke();
            return false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            pointer_enter = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            pointer_exit = true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            pointer_down = true;
        }
    }
}

