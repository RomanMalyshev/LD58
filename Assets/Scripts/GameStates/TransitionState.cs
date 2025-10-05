using System;
using State_Machine;
using Model;
using UI;

namespace GameStates
{
    public class TransitionState : IState
    {
        public Action<GameEndCondition> OnGameEnd;
        public Action OnNextTurn;

        private Player _player;
        private Hud _hud;
        private bool _transitionComplete;
        private GameEndCondition _gameCondition;

        public TransitionState(Player player, Hud hud)
        {
            _player = player;
            _hud = hud;
        }

        public void Enter()
        {
            _transitionComplete = false;
            CheckGameConditions();
        }

        public void Execute()
        {
            if (_transitionComplete)
            {
                if (_gameCondition != GameEndCondition.Continue)
                {
                    OnGameEnd?.Invoke(_gameCondition);
                }
                else
                {
                    OnNextTurn?.Invoke();
                }
            }
        }

        public void Exit()
        {
            _hud.OnPopupAccept -= OnContinue;
            _hud.HidePopup();
        }

        private void CheckGameConditions()
        {
            _gameCondition = GameConditionsChecker.CheckGameConditions(_player);

            if (_gameCondition != GameEndCondition.Continue)
            {
                // Game ended
                _transitionComplete = true;
            }
            else
            {
                // Show end of turn message
                ShowTurnEndMessage();
            }
        }

        private void ShowTurnEndMessage()
        {
            string message = $"Turn {_player.CurrentTurn} Complete!\n\n";
            message += $"Castles Captured: {_player.CapturedCastles}/{GameConditionsChecker.CASTLES_TO_WIN}\n";
            message += $"Turns Remaining: {GameConditionsChecker.MAX_TURNS - _player.CurrentTurn}";

            _hud.ShowPopup(message, "Next Turn", null);
            _hud.OnPopupAccept += OnContinue;
        }

        private void OnContinue()
        {
            _hud.OnPopupAccept -= OnContinue;
            _hud.HidePopup();

            // Increment turn counter
            _player.CurrentTurn++;

            _transitionComplete = true;
        }
    }
}