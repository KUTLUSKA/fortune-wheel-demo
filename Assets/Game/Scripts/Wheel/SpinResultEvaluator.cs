using UnityEngine;
using System.Collections.Generic;

public class SpinResultEvaluator
{
    public SliceDataSO Evaluate(List<SliceDataSO> slices)
    {
        float totalWeight = 0f;
        foreach (var slice in slices)
            totalWeight += slice.Weight;

        float random = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var slice in slices)
        {
            cumulative += slice.Weight;
            if (random <= cumulative)
                return slice;
        }

        return slices[slices.Count - 1];
    }
}