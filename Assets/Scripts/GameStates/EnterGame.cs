using System;
using State_Machine;
using UI;
using Map;
using Model;

namespace GameStates
{
    public class EnterGame : IState
    {
        public Action OnPlayerStartGame;
        
        private Hud _hud;
        private MapController _map;

        public EnterGame(Hud hud, MapController map)
        {
            _hud = hud;
            _map = map;
        }

        public void Enter()
        {
            string welcomeMessage = "Welcome, my Lord!\n\n";
            welcomeMessage += "The time has come to expand our kingdom. ";
            welcomeMessage += "We must collect the castles in the surrounding lands.\n\n";
            welcomeMessage += $"Your goal: Capture {GameConditionsChecker.CASTLES_TO_WIN} castles before resources run out.\n";
            welcomeMessage += "Good luck!";
            
            _hud.ShowPopup(welcomeMessage, "Begin", null);
            _hud.OnPopupAccept += StartGame;
            _map.SetTilesInteractionState(false);
        }

        public void StartGame()
        {
            OnPlayerStartGame?.Invoke();
        }

        public void Execute()
        {
        }

        public void Exit()
        {
            _hud.OnPopupAccept -= StartGame;
            _hud.HidePopup();
        }
    }
}