using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Hud : MonoBehaviour
    {
        public Action OnPopupDecline;
        public Action OnPopupAccept;

        [SerializeField] private TMP_Text Influence;
        [SerializeField] private TMP_Text Power;
        [SerializeField] private TMP_Text Food;
        [SerializeField] private TMP_Text Wood;
        [SerializeField] private TMP_Text Iron;
        [SerializeField] private TMP_Text Gold;

        [SerializeField] private GameObject Popup;
        [SerializeField] private TMP_Text PopupMessage;
        [SerializeField] private TMP_Text PopupAcceptLabel;
        [SerializeField] private TMP_Text PopupDeclineLabel;
        [SerializeField] private Button PopupAccept;
        [SerializeField] private Button PopupDecline;

        private void Start()
        {
            PopupAccept.onClick.AddListener(() => OnPopupAccept?.Invoke());
            PopupDecline.onClick.AddListener(() => OnPopupDecline?.Invoke());
        }

        public void ShowPopup(string message, string accept, string decline)
        {
            Popup.gameObject.SetActive(true);
            PopupAccept.gameObject.SetActive(!string.IsNullOrEmpty(accept));
            PopupDecline.gameObject.SetActive(!string.IsNullOrEmpty(decline));

            PopupMessage.text = message;
            PopupAcceptLabel.text = accept;
            PopupDeclineLabel.text = decline;
        }

        public void HidePopup()
        {
            Popup.gameObject.SetActive(false);
        }
    }
}