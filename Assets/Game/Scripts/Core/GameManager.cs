using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private ZoneManager _zoneManager;
    [SerializeField] private RewardInventory _rewardInventory;

    [Header("Events")]
    [SerializeField] private GameEventSO _onBombHit;
    [SerializeField] private GameEventSO _onPlayerLeft;
    [SerializeField] private GameEventSO _onZoneAdvanced;

    private ISpinStrategy _currentStrategy;
    private SpinResultEvaluator _evaluator = new SpinResultEvaluator();

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

    public void OnSpinCompleted(SliceDataSO result)
    {
        if (result.IsBomb)
        {
            _rewardInventory.ClearRewards();
            _onBombHit?.Raise();
            return;
        }

        _rewardInventory.AddReward(result.RewardType, result.RewardAmount);
        _zoneManager.AdvanceZone();
        UpdateStrategy();
        _onZoneAdvanced?.Raise();
    }

    public void OnPlayerLeft()
    {
        _onPlayerLeft?.Raise();
        ResetGame();
    }

    public SliceDataSO EvaluateSpin()
    {
        var config = _currentStrategy.GetWheelConfig();
        return _evaluator.Evaluate(config.Slices);
    }

    public ISpinStrategy GetCurrentStrategy() => _currentStrategy;

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

    private void ResetGame()
    {
        _rewardInventory.ClearRewards();
        _zoneManager.ResetZone();
        UpdateStrategy();
    }
}