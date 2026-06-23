using UnityEngine;
using System.Collections.Generic;

public class RewardInventory : MonoBehaviour
{
    private Dictionary<SliceDataSO, int> _rewards = new();

    private SliceDataSO ResolveKey(SliceDataSO slice) =>
        RewardStackingHelper.ResolveKey(slice, _rewards);

    public void AddReward(SliceDataSO slice, int amount)
    {
        var key = ResolveKey(slice);
        if (_rewards.ContainsKey(key))
            _rewards[key] += amount;
        else
            _rewards[key] = amount;
    }

    public void ClearRewards() => _rewards.Clear();

    public int GetRewardAmount(SliceDataSO slice) =>
        _rewards.TryGetValue(ResolveKey(slice), out int val) ? val : 0;

    public int GetTotalByType(RewardType type)
    {
        int total = 0;
        foreach (var kv in _rewards)
            if (kv.Key.RewardType == type)
                total += kv.Value;
        return total;
    }

    public void SpendReward(RewardType type, int amount)
    {
        int remaining = amount;
        var keys = new List<SliceDataSO>(_rewards.Keys);
        foreach (var key in keys)
        {
            if (key.RewardType != type || remaining <= 0) continue;
            int deduct = Mathf.Min(_rewards[key], remaining);
            _rewards[key] -= deduct;
            remaining -= deduct;
        }
    }

    public Dictionary<SliceDataSO, int> GetSnapshot() => new(_rewards);

    public void RestoreFromSnapshot(Dictionary<SliceDataSO, int> snapshot) =>
        _rewards = new(snapshot);

    public Dictionary<SliceDataSO, int> GetAllRewards() => _rewards;
}
