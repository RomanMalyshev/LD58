using UnityEngine;
using State_Machine;
using GameStates;
using Model;
using UI;
using Configs;
using Map;
using Events;

public class GameFlow : MonoBehaviour
{
    [SerializeField] private RedBjorn.ProtoTiles.MapSettings _mapEditorSettings;
    [SerializeField] private RedBjorn.ProtoTiles.MapView _mapEditorView;
    [SerializeField] private Hud _hud;
    [SerializeField] private PlayerStartConfig _playerStartConfig;
    [SerializeField] private TilesConfig _tilesConfig;
    [SerializeField] private EventsPool _globalEventsPool;
    [SerializeField] private EventsPool _tavernEventsPool;
    [SerializeField] private EventsPool _campEventsPool;

    private StateMachine _stateMachine;
    private EnterGame _enterGame;
    private Occupy _occupy;
    private RandomEvent _randomEvent;
    private TileUpgrade _tileUpgrade;
    private ResourcesUpdate _resourcesUpdate;
    private TransitionState _transitionState;
    private EndGame _endGame; 
    
    private Player _playerModel;
    private MapController _map;
    private EventManager _eventManager;
    private void Start()
    {
        InitializePlayer();

        _map = new(_mapEditorSettings, _mapEditorView, _tilesConfig);
        if (_map.MaxRadius > 5)
        {
            _map.HideAtRadius(_map.MaxRadius-1);
            _map.HideAtRadius(_map.MaxRadius-2);
        }

        InitializeStateMachine();
        _stateMachine.ChangeState(_enterGame);
    }

    private void InitializePlayer()
    {
        _playerModel = new Player();
        _playerModel.OnResourcesChanged += _hud.UpdateResources;
        _playerModel.OnGameStatsChanged += _hud.UpdateGameStats;
        
        _playerModel.Influence = _playerStartConfig.Influence;
        _playerModel.Power = _playerStartConfig.Power;
        _playerModel.Food = _playerStartConfig.Food;
        _playerModel.Gold = _playerStartConfig.Gold;
        _playerModel.Metal = _playerStartConfig.Metal;
        _playerModel.Wood = _playerStartConfig.Wood;
        
        _playerModel.CurrentTurn = 1;
        _playerModel.CapturedCastles = 0;
        _playerModel.TilesCaptured = 0;
    }

    private void Update()
    {
        _stateMachine.Execute();
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
    }

    private void InitializeStateMachine()
    {
        _stateMachine = new StateMachine();
        _eventManager = new EventManager(_globalEventsPool, _tavernEventsPool, _campEventsPool);
        
        _enterGame = new EnterGame(_hud, _map);
        _occupy = new Occupy(_map, _hud, _playerModel, _eventManager);
        _randomEvent = new RandomEvent(_map, _hud, _playerModel, _eventManager);
        _tileUpgrade = new TileUpgrade(_map, _hud, _playerModel);
        _resourcesUpdate = new ResourcesUpdate(_playerModel, _map);
        _transitionState = new TransitionState(_playerModel, _hud);
        _endGame = new EndGame(_hud);

        // Setup state transitions
        _enterGame.OnPlayerStartGame += () =>
        {
            _stateMachine.ChangeState(_randomEvent);
        };

        _randomEvent.OnEventCompleted += () =>
        {
            _stateMachine.ChangeState(_occupy);
        };

        _occupy.OnTileCaptured += () =>
        {
            _stateMachine.ChangeState(_tileUpgrade);
        };

        _tileUpgrade.OnUpgradeCompleted += () =>
        {
            _stateMachine.ChangeState(_resourcesUpdate);
        };

        _resourcesUpdate.OnResourcesUpdated += () =>
        {
            _stateMachine.ChangeState(_transitionState);
        };

        _transitionState.OnNextTurn += () =>
        {
            _stateMachine.ChangeState(_randomEvent);
        };

        _transitionState.OnGameEnd += (condition) =>
        {
            _endGame.SetEndCondition(condition);
            _stateMachine.ChangeState(_endGame);
        };

        _endGame.OnRestart += RestartGame;
    }

    private void RestartGame()
    {
        // Reset player to initial state
        _playerModel.ResetToDefault(
            _playerStartConfig.Influence,
            _playerStartConfig.Power,
            _playerStartConfig.Food,
            _playerStartConfig.Gold,
            _playerStartConfig.Metal,
            _playerStartConfig.Wood
        );

        // Reset map to initial state
        _map.ResetMap();

        // Change state to enter game
        _stateMachine.ChangeState(_enterGame);
    }

    private void OnDestroy()
    {
        _playerModel.OnResourcesChanged -= _hud.UpdateResources;
        _playerModel.OnGameStatsChanged -= _hud.UpdateGameStats;
    }
}