using System.Collections.Generic;

public static class RewardStackingHelper
{
    public static bool IsStackingType(RewardType type) =>
        type == RewardType.Gold || type == RewardType.Cash;

    public static SliceDataSO ResolveKey<TValue>(SliceDataSO slice, Dictionary<SliceDataSO, TValue> dict)
    {
        if (!IsStackingType(slice.RewardType)) return slice;
        foreach (var key in dict.Keys)
            if (key.RewardType == slice.RewardType)
                return key;
        return slice;
    }
}
