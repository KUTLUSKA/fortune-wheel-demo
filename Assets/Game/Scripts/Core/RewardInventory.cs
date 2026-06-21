using UnityEngine;
using System.Collections.Generic;

public class RewardInventory : MonoBehaviour
{
    private Dictionary<RewardType, int> _rewards = new Dictionary<RewardType, int>();

    public void AddReward(RewardType type, int amount)
    {
        if (_rewards.ContainsKey(type))
            _rewards[type] += amount;
        else
            _rewards[type] = amount;
    }

    public void ClearRewards()
    {
        _rewards.Clear();
    }

    public Dictionary<RewardType, int> GetAllRewards()
    {
        return _rewards;
    }

    public int GetRewardAmount(RewardType type)
    {
        return _rewards.ContainsKey(type) ? _rewards[type] : 0;
    }
}