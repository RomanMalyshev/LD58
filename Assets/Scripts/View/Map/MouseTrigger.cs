using System;
using UnityEngine;

namespace View.Map
{
    public class MouseTrigger : MonoBehaviour
    {
        public event Action OnClicked;
        public event Action OnHoverEnter;
        public event Action OnHoverExit;
        
        public void OnMouseEnter()
        {
            OnHoverEnter?.Invoke();
        }

        public void OnMouseExit()
        {
            OnHoverExit?.Invoke();
        }

        public void OnMouseDown()
        {
            OnClicked?.Invoke();
        }
    }
}