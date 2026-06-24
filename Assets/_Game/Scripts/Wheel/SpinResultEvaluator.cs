using UnityEngine;
using System.Collections.Generic;

public class SpinResultEvaluator
{
    // bombWinChanceOverride: when >= 0, replaces the bomb slice's SpinChances weight.
    // Pass -1 (default) to use the static SpinChances value from the WheelConfigSO.
    public SliceDataSO Evaluate(List<SliceDataSO> displayedSlices, IReadOnlyList<WheelConfigSO.SpinChanceEntry> spinChances, float bombWinChanceOverride = -1f)
    {
        var weights = new float[displayedSlices.Count];
        float total = 0f;

        for (int i = 0; i < displayedSlices.Count; i++)
        {
            float w = 0f;
            if (bombWinChanceOverride >= 0f && displayedSlices[i].IsBomb)
            {
                w = bombWinChanceOverride;
            }
            else
            {
                foreach (var entry in spinChances)
                {
                    if (entry.Slice == displayedSlices[i])
                    {
                        w = entry.WinWeight;
                        break;
                    }
                }
            }
            weights[i] = w;
            total += w;
        }

        if (total <= 0f)
            return displayedSlices[Random.Range(0, displayedSlices.Count)];

        float roll = Random.Range(0f, total);
        float cumulative = 0f;
        for (int i = 0; i < displayedSlices.Count; i++)
        {
            cumulative += weights[i];
            if (roll <= cumulative)
                return displayedSlices[i];
        }

        return displayedSlices[displayedSlices.Count - 1];
    }
}