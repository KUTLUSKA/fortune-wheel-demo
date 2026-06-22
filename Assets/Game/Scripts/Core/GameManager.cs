using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private ZoneManager _zoneManager;
    [SerializeField] private RewardInventory _rewardInventory;
    [SerializeField] private GameStateController _stateController;

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

    private ISpinStrategy _currentStrategy;
    private readonly SpinResultEvaluator _evaluator = new SpinResultEvaluator();
    private SliceDataSO _lastSpinResult;
    private Dictionary<RewardType, int> _inventorySnapshot;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateStrategy();
    }

    public void StartSpin()
    {
        if (_stateController.CurrentState != GameState.Idle) return;

        _lastSpinResult = EvaluateSpin();

        // Snapshot inventory BEFORE any changes so revive can restore it
        _inventorySnapshot = _rewardInventory.GetSnapshot();

        _stateController.TransitionTo(GameState.Spinning);
        OnSpinResultEvaluated?.Invoke(_lastSpinResult);
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
        _stateController.TransitionTo(GameState.ZoneTransition);
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

    public void RequestRevive(Dictionary<RewardType, int> snapshot, int zone)
    {
        _rewardInventory.RestoreFromSnapshot(snapshot);
        _zoneManager.SetZone(zone);
        UpdateStrategy();
        _onZoneAdvanced?.Raise();
        _stateController.TransitionTo(GameState.Idle);
    }

    public Dictionary<RewardType, int> GetInventorySnapshot() => _inventorySnapshot;
    public int GetSnapshotZone() => _zoneManager.CurrentZone;

    public SliceDataSO EvaluateSpin()
    {
        var config = _currentStrategy.GetWheelConfig();
        return _evaluator.Evaluate(config.Slices);
    }

    public ISpinStrategy GetCurrentStrategy() => _currentStrategy;

    public void ResetGame()
    {
        _rewardInventory.ClearRewards();
        _zoneManager.ResetZone();
        UpdateStrategy();
        _stateController.TransitionTo(GameState.Idle);
        OnGameReset?.Invoke();
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
