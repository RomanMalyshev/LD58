using Model.Map;
using State_Machine;
using UI;
using UnityEngine;
using View.Map;

namespace GameStates
{
    public class EnterGame : IState
    {
        private Hud _hud;
        private MapModel _mapModel;
        private MapView _mapView;

        public EnterGame(Hud hud, MapModel mapModel, MapView mapView)
        {
            _hud = hud;
            _mapModel = mapModel;
            _mapView = mapView;
        }

        public void Enter()
        {
            _mapView.SetTilesInteractable(false);
            _mapView.HighlightTile(Vector2Int.zero, Color.green);
            
            _hud.ShowPopup("Start Game","Ok",null);
            _hud.OnPopupAccept += () =>
            {
                
            };
        }

        public void Execute()
        {
        }

        public void Exit()
        {
            _mapView.SetTilesInteractable(true);
            _mapView.ClearTileHighlight(Vector2Int.zero);
            
            _hud.HidePopup();
        }
    }
}