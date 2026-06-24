using UnityEngine;

[CreateAssetMenu(fileName = "SliceData", menuName = "WheelOfFortune/SliceData")]
public class SliceDataSO : ScriptableObject, ISliceData
{
    [Header("Info")]
    [SerializeField] private string _sliceName;
    [SerializeField] private Sprite _icon;

    [Header("Reward")]
    [SerializeField] private RewardType _rewardType;
    [SerializeField] private int _rewardAmount;
    [SerializeField] private bool _isBomb;

    public string SliceName => _sliceName;
    public Sprite Icon => _icon;
    public RewardType RewardType => _rewardType;
    public int RewardAmount => _rewardAmount;
    public bool IsBomb => _isBomb;
}