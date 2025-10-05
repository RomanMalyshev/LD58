using UnityEngine;
using State_Machine;
using GameStates;
using Model;
using Model.Map;
using View.Map;
using Presenters;
using UI;

public class GameFlow : MonoBehaviour
{
    [SerializeField] private RedBjorn.ProtoTiles.MapSettings _mapEditorSettings;
    [SerializeField] private RedBjorn.ProtoTiles.MapView _mapEditorView;
    [SerializeField] private Hud _hud;
    
    private StateMachine _stateMachine;
    private StartTurn _startTurn;
    private RandomEvent _randomEvent;
    private ResourcesUpdate _resourcesUpdate;
    private PlayerTurn _playerTurn;
    private EndTurn _endTurn;

    private Player _playerModel;

    private MapModel _mapModel;
    private MapPresenter _mapPresenter;
    private MapView _mapView;


    private void Start()
    {
        _playerModel = new Player();

        _mapModel = MapEditorToGameMap.GetMapModel(_mapEditorSettings, _mapEditorView);
        _mapView = new MapView(_mapEditorView.Tiles);
        _mapPresenter = new MapPresenter(_mapModel, _mapView);

        InitializeStateMachine();
        _stateMachine.ChangeState(_playerTurn);
    }

    private void Update()
    {
        _stateMachine.Execute();
    }

    private void InitializeStateMachine()
    {
        _stateMachine = new StateMachine();
        _startTurn = new StartTurn();
        _randomEvent = new RandomEvent();
        _resourcesUpdate = new ResourcesUpdate();
        _playerTurn = new PlayerTurn(_mapModel);
        _endTurn = new EndTurn();
    }

    public void TransitionToRandomEvent()
    {
        _stateMachine.ChangeState(_randomEvent);
    }

    public void TransitionToResourcesUpdate()
    {
        _stateMachine.ChangeState(_resourcesUpdate);
    }

    public void TransitionToPlayerTurn()
    {
        _stateMachine.ChangeState(_playerTurn);
    }

    public void TransitionToEndTurn()
    {
        _stateMachine.ChangeState(_endTurn);
    }

    public void TransitionToStartTurn()
    {
        _stateMachine.ChangeState(_startTurn);
    }

    private void OnDestroy()
    {
        _mapPresenter.Cleanup();
    }
}