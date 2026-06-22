using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private ZoneManager _zoneManager;
    [SerializeField] private RewardInventory _rewardInventory;
    [SerializeField] private GameStateController _stateController;
    [SerializeField] private WheelController _wheelController;

    [Header("Events")]
    [SerializeField] private GameEventSO _onBombHit;
    [SerializeField] private GameEventSO _onPlayerLeft;
    [SerializeField] private GameEventSO _onZoneAdvanced;

    public event Action<SliceDataSO> OnSpinResultEvaluated;
    public event Action OnBombHitEvent;
    public event Action OnGameReset;

    public ZoneManager ZoneManager => _zoneManager;
    public RewardInventory RewardInventory => _rewardInventory;
    public GameStateController StateController => _stateController;
    public WheelController WheelController => _wheelController;

    private ISpinStrategy _currentStrategy;
    private readonly SpinResultEvaluator _evaluator = new SpinResultEvaluator();
    private SliceDataSO _lastSpinResult;
    private Dictionary<RewardType, int> _inventorySnapshot;
    private int _zoneSnapshot;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        UpdateStrategy();
    }

    private void Start()
    {
        RebuildWheel();
    }

    public void StartSpin()
    {
        if (_stateController.CurrentState != GameState.Idle) return;

        _lastSpinResult = EvaluateSpin();

        _inventorySnapshot = _rewardInventory.GetSnapshot();
        _zoneSnapshot = _zoneManager.CurrentZone;

        OnSpinResultEvaluated?.Invoke(_lastSpinResult);
        _stateController.TransitionTo(GameState.Spinning);
    }

    // Called by WheelSpinHandler when spin animation finishes
    public void OnAnimationComplete()
    {
        _stateController.TransitionTo(GameState.ShowResult);
    }

    // Called by RewardRevealUI when card reveal animation finishes
    public void OnRevealComplete()
    {
        if (_lastSpinResult.IsBomb)
        {
            _rewardInventory.ClearRewards();
            _stateController.TransitionTo(GameState.BombHit);
            _onBombHit?.Raise();
            OnBombHitEvent?.Invoke();
            return;
        }

        _rewardInventory.AddReward(_lastSpinResult.RewardType, _lastSpinResult.RewardAmount);
        _zoneManager.AdvanceZone();
        UpdateStrategy();
        _onZoneAdvanced?.Raise();

        // Eski slicelar stagger ile çıkar → yeni wheel bump ederek girer → zone transition
        _wheelController.ClearSlicesAnimated(() =>
        {
            RebuildWheel();
            _stateController.TransitionTo(GameState.ZoneTransition);
        });
    }

    public void OnZoneTransitionComplete()
    {
        _stateController.TransitionTo(GameState.Idle);
    }

    public void OnPlayerLeft()
    {
        if (!_currentStrategy.CanPlayerLeave) return;
        _onPlayerLeft?.Raise();
        ResetGame();
    }

    public void RequestRevive()
    {
        var cmd = new ReviveCommand(_rewardInventory, _zoneManager, _inventorySnapshot, _zoneSnapshot);
        cmd.Execute();
        UpdateStrategy();
        RebuildWheel();
        _onZoneAdvanced?.Raise();
        _stateController.TransitionTo(GameState.Idle);
    }

    public SliceDataSO EvaluateSpin()
    {
        var shownSlices = _wheelController.CurrentSlices.Select(s => s.Data).ToList();
        return _evaluator.Evaluate(shownSlices, _wheelController.CurrentConfig.SpinChances);
    }

    public ISpinStrategy GetCurrentStrategy() => _currentStrategy;

    public void ResetGame()
    {
        _rewardInventory.ClearRewards();
        _zoneManager.ResetZone();
        UpdateStrategy();
        RebuildWheel();
        _stateController.TransitionTo(GameState.Idle);
        OnGameReset?.Invoke();
    }

    private void RebuildWheel()
    {
        _wheelController.SetActiveWheel(_currentStrategy.WheelType);
        _wheelController.BuildWheel(_currentStrategy.GetWheelConfig(), _currentStrategy.HasBomb);
    }

    private void UpdateStrategy()
    {
        var config = _zoneManager.GetCurrentWheelConfig();

        if (_zoneManager.IsSuperZone)
            _currentStrategy = new GoldenSpinStrategy(config);
        else if (_zoneManager.IsSafeZone)
            _currentStrategy = new SilverSpinStrategy(config);
        else
            _currentStrategy = new BronzeSpinStrategy(config);
    }
}
