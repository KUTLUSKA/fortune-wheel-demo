using UnityEngine;

public interface ISliceData
{
    string SliceName { get; }
    Sprite Icon { get; }
    RewardType RewardType { get; }
    int RewardAmount { get; }
    bool IsBomb { get; }
}