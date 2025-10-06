using System;
using Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UI
{
    public class Hud : MonoBehaviour
    {
        public Action OnPopupDecline;
        public Action OnPopupAccept;

        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField] private TMP_Text Influence;
        [SerializeField] private TMP_Text Power;
        [SerializeField] private TMP_Text Food;
        [SerializeField] private TMP_Text Wood;
        [SerializeField] private TMP_Text Iron;
        [SerializeField] private TMP_Text Gold;

        [SerializeField] private TMP_Text CurrentTurn;
        [SerializeField] private TMP_Text CapturedCastles;

        [Header("Income Display")] [SerializeField]
        private TMP_Text IncomeFood;

        [SerializeField] private TMP_Text IncomePower;
        [SerializeField] private TMP_Text IncomeWood;
        [SerializeField] private TMP_Text IncomeGold;
        [SerializeField] private TMP_Text IncomeMetal;

        [SerializeField] private GameObject Popup;
        [SerializeField] private TMP_Text PopupMessage;
        [SerializeField] private TMP_Text PopupAcceptLabel;
        [SerializeField] private TMP_Text PopupDeclineLabel;
        [SerializeField] private Button PopupAccept;
        [SerializeField] private Button PopupDecline;

        [SerializeField] private GameObject TileInfoPanel;
        [SerializeField] private TMP_Text TileInfoText;
        [SerializeField] private Vector2 TileInfoOffset = new Vector2(10, 10);

        public event Action OnMassageChange;

        private RectTransform _tileInfoRectTransform;
        private Canvas _canvas;

        private void Start()
        {
            PopupAccept.onClick.AddListener(() => OnPopupAccept?.Invoke());
            PopupDecline.onClick.AddListener(() => OnPopupDecline?.Invoke());

            if (TileInfoPanel != null)
            {
                _tileInfoRectTransform = TileInfoPanel.GetComponent<RectTransform>();
                _canvas = GetComponentInParent<Canvas>();
            }
        }

        private void Update()
        {
            if (TileInfoPanel != null && TileInfoPanel.activeSelf && _tileInfoRectTransform != null)
            {
                UpdateTileInfoPosition();
            }
        }

        private void UpdateTileInfoPosition()
        {
            Vector2 mousePosition = Input.mousePosition;
            
            if (_canvas != null && _tileInfoRectTransform != null)
            {
                // Обновляем layout для получения актуальной высоты
                LayoutRebuilder.ForceRebuildLayoutImmediate(_tileInfoRectTransform);
                
                // Получаем актуальную высоту панели после обновления layout
                float panelHeight = _tileInfoRectTransform.rect.height;
                
                // Корректируем оффсет с учётом высоты панели (позиционируем от нижнего края)
                Vector2 adjustedOffset = new Vector2(TileInfoOffset.x, TileInfoOffset.y + panelHeight);
                
                if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    _tileInfoRectTransform.position = mousePosition + adjustedOffset;
                }
                else
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        _canvas.transform as RectTransform,
                        mousePosition,
                        _canvas.worldCamera,
                        out Vector2 localPoint);
                    _tileInfoRectTransform.localPosition = localPoint + adjustedOffset;
                }
            }
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
                CapturedCastles.text =
                    $"Your majeste \nyou need to collect  \n{GameConditionsChecker.CASTLES_TO_WIN - capturedCastles}  enemy castle's\n";
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
                
                // Принудительно обновляем layout чтобы получить актуальный размер
                Canvas.ForceUpdateCanvases();
                if (_tileInfoRectTransform != null)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(_tileInfoRectTransform);
                }
            }
        }

        public void HideTileInfo()
        {
            if (TileInfoPanel != null)
            {
                TileInfoPanel.SetActive(false);
            }
        }

        public void UpdateIncome(int foodIncome, int powerIncome, int woodIncome, int goldIncome, int metalIncome,
            int foodConsumption)
        {
            int netFoodIncome = foodIncome - foodConsumption;

            IncomeFood.gameObject.SetActive(netFoodIncome != 0);
            IncomePower.gameObject.SetActive(powerIncome != 0);
            IncomeWood.gameObject.SetActive(woodIncome != 0);
            IncomeGold.gameObject.SetActive(goldIncome != 0);
            IncomeMetal.gameObject.SetActive(metalIncome != 0);
            IncomeFood.text = FormatIncome(netFoodIncome);
            IncomePower.text = FormatIncome(powerIncome);
            IncomeWood.text = FormatIncome(woodIncome);
            IncomeGold.text = FormatIncome(goldIncome);
            IncomeMetal.text = FormatIncome(metalIncome);
        }

        private string FormatIncome(int income)
        {
            if (income > 0)
                return $"+{income}";
            else if (income < 0)
                return $"{income}";
            else
                return "0";
        }
    }
}