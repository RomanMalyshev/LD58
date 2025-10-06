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

        [SerializeField] private TMP_Text CurrentTurn;
        [SerializeField] private TMP_Text CapturedCastles;

        [SerializeField] private GameObject Popup;
        [SerializeField] private TMP_Text PopupMessage;
        [SerializeField] private TMP_Text PopupAcceptLabel;
        [SerializeField] private TMP_Text PopupDeclineLabel;
        [SerializeField] private Button PopupAccept;
        [SerializeField] private Button PopupDecline;

        [SerializeField] private GameObject TileInfoPanel;
        [SerializeField] private TMP_Text TileInfoText;

        public event Action OnMassageChange;

        private void Start()
        {
            PopupAccept.onClick.AddListener(() => OnPopupAccept?.Invoke());
            PopupDecline.onClick.AddListener(() => OnPopupDecline?.Invoke());
        }

        public void UpdateResources(int influence, int power, int food, int gold, int metal, int wood)
        {
            Influence.text = influence.ToString();
            Power.text = power.ToString();
            Food.text = food.ToString();
            Wood.text = wood.ToString();
            Iron.text = metal.ToString();
            Gold.text = gold.ToString();
        }

        public void UpdateGameStats(int currentTurn, int capturedCastles)
        {
            if (CurrentTurn != null)
                CurrentTurn.text = $"Turn: {currentTurn}";
            if (CapturedCastles != null)
                CapturedCastles.text = $"Castles: {capturedCastles}";
        }

        public void ShowPopup(string message, string accept, string decline)
        {
            Popup.gameObject.SetActive(true);
            PopupAccept.gameObject.SetActive(!string.IsNullOrEmpty(accept));
            PopupDecline.gameObject.SetActive(!string.IsNullOrEmpty(decline));

            PopupMessage.text = message;
            PopupAcceptLabel.text = accept;
            PopupDeclineLabel.text = decline;
            
            OnMassageChange?.Invoke();
        }

        public void HidePopup()
        {
            Popup.gameObject.SetActive(false);
        }

        public void ShowTileInfo(string info)
        {
            if (TileInfoPanel != null && TileInfoText != null)
            {
                TileInfoPanel.SetActive(true);
                TileInfoText.text = info;
            }
        }

        public void HideTileInfo()
        {
            if (TileInfoPanel != null)
            {
                TileInfoPanel.SetActive(false);
            }
        }
    }
}