using UnityEngine;
using State_Machine;
using GameStates;
using Model;
using Model.Map;
using View.Map;
using Presenters;
using UI;
using Configs;

public class GameFlow : MonoBehaviour
{
    [SerializeField] private RedBjorn.ProtoTiles.MapSettings _mapEditorSettings;
    [SerializeField] private RedBjorn.ProtoTiles.MapView _mapEditorView;
    [SerializeField] private Hud _hud;
    [SerializeField] private PlayerStartConfig _playerStartConfig;

    private StateMachine _stateMachine;
    private EnterGame _enterGame;
    private StartTurn _startTurn;
    private RandomEvent _randomEvent;
    private ResourcesUpdate _resourcesUpdate;
    private PlayerTurn _playerTurn;
    private EndTurn _endTurn;
    private TransitionState _transitionState;

    private Player _playerModel;

    private MapModel _mapModel;
    private MapPresenter _mapPresenter;
    private MapView _mapView;

    private void Start()
    {
        InitializePlayer();

        _mapModel = MapEditorToGameMap.GetMapModel(_mapEditorSettings, _mapEditorView);
        _mapView = new MapView(_mapEditorView.Tiles);
        _mapPresenter = new MapPresenter(_mapModel, _mapView);

        InitializeStateMachine();
        _stateMachine.ChangeState(_enterGame);
    }

    private void InitializePlayer()
    {
        _playerModel = new Player();
        _playerModel.OnResourcesChanged += _hud.UpdateResources;
        
        _playerModel.Influence = _playerStartConfig.Influence;
        _playerModel.Power = _playerStartConfig.Power;
        _playerModel.Food = _playerStartConfig.Food;
        _playerModel.Gold = _playerStartConfig.Gold;
        _playerModel.Metal = _playerStartConfig.Metal;
        _playerModel.Wood = _playerStartConfig.Wood;
    }

    private void Update()
    {
        _stateMachine.Execute();
    }

    private void InitializeStateMachine()
    {
        _stateMachine = new StateMachine();
        _enterGame = new EnterGame(_hud, _mapModel, _mapView);
        _randomEvent = new RandomEvent();
        _startTurn = new StartTurn();
        _playerTurn = new PlayerTurn(_mapModel);
        _endTurn = new EndTurn();
        _resourcesUpdate = new ResourcesUpdate();
        _transitionState = new TransitionState();
    }

    private void OnDestroy()
    {
        _playerModel.OnResourcesChanged -= _hud.UpdateResources;
        _mapPresenter.Cleanup();
    }
}