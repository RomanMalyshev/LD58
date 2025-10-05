using State_Machine;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Model;

namespace GameStates
{
    public class EndGame : IState
    {
        private Hud _hud;
        private GameEndCondition _endCondition;
        private bool _waitingForRestart;

        public EndGame(Hud hud)
        {
            _hud = hud;
        }

        public void SetEndCondition(GameEndCondition condition)
        {
            _endCondition = condition;
        }

        public void Enter()
        {
            _waitingForRestart = false;
            ShowEndGameScreen();
        }

        public void Execute()
        {
            // Game ended, waiting for restart
        }

        public void Exit()
        {
            _hud.OnPopupAccept -= OnRestart;
            _hud.HidePopup();
        }

        private void ShowEndGameScreen()
        {
            string message = "";
            
            switch (_endCondition)
            {
                case GameEndCondition.Victory:
                    message = "VICTORY!\n\nYou have collected all the castles and secured your kingdom!";
                    break;
                case GameEndCondition.DefeatNoFood:
                    message = "DEFEAT - STARVATION\n\nYour people starved. The kingdom has fallen.";
                    break;
                case GameEndCondition.DefeatNoInfluence:
                    message = "DEFEAT - REBELLION\n\nYou lost all influence. The people have rebelled!";
                    break;
                default:
                    message = "Game Over";
                    break;
            }

            _hud.ShowPopup(message, "Restart", null);
            _hud.OnPopupAccept += OnRestart;
            _waitingForRestart = true;
        }

        private void OnRestart()
        {
            if (!_waitingForRestart) return;
            
            _waitingForRestart = false;
            _hud.OnPopupAccept -= OnRestart;
            
            // Reload current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}