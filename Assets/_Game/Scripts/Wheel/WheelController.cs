using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WheelController : MonoBehaviour
{
    private const float PlacementRadiusRatio = 0.62f;

    [SerializeField] private SliceFactory _sliceFactory;
    [SerializeField] private GameObject _bronzeWheelRoot;
    [SerializeField] private GameObject _silverWheelRoot;
    [SerializeField] private GameObject _goldenWheelRoot;

    private readonly List<SliceView> _currentSlices = new List<SliceView>();
    private Transform _activeWheelTransform;
    private WheelConfigSO _currentConfig;

    public IReadOnlyList<SliceView> CurrentSlices => _currentSlices;
    public Transform ActiveWheelTransform => _activeWheelTransform;
    public WheelConfigSO CurrentConfig => _currentConfig;

    public void SetActiveWheel(WheelType type)
    {
        _bronzeWheelRoot.SetActive(type == WheelType.Bronze);
        _silverWheelRoot.SetActive(type == WheelType.Silver);
        _goldenWheelRoot.SetActive(type == WheelType.Golden);

        _activeWheelTransform = type switch
        {
            WheelType.Silver => _silverWheelRoot.transform,
            WheelType.Golden => _goldenWheelRoot.transform,
            _                => _bronzeWheelRoot.transform
        };
    }

    public void BuildWheel(WheelConfigSO config, bool hasBomb)
    {
        _currentConfig = config;
        _sliceFactory.ClearSlices(_activeWheelTransform);
        _currentSlices.Clear();

        var selected = PickSlices(config.SlicePool, config.SlicesPerSpin, hasBomb);

        float wheelRadius = _activeWheelTransform.GetComponent<RectTransform>().rect.width * 0.5f;
        float placementRadius = wheelRadius * PlacementRadiusRatio;
        float sliceAngle = 360f / selected.Count;

        for (int i = 0; i < selected.Count; i++)
        {
            SliceView view = _sliceFactory.CreateSlice(selected[i], _activeWheelTransform, i * sliceAngle, placementRadius);
            _currentSlices.Add(view);
        }
    }

    public void ResetAllHighlights()
    {
        foreach (var slice in _currentSlices)
            slice.ResetVisual();
    }

    // Bomb always included when hasBomb=true. Non-bombs picked by weight (SliceDataSO.Weight).
    private List<SliceDataSO> PickSlices(List<SliceDataSO> pool, int count, bool hasBomb)
    {
        var bombs    = pool.Where(s => s.IsBomb).ToList();
        var nonBombs = pool.Where(s => !s.IsBomb).ToList();

        var selected = new List<SliceDataSO>();

        if (hasBomb && bombs.Count > 0)
        {
            selected.Add(bombs[Random.Range(0, bombs.Count)]);
            count--;
        }

        selected.AddRange(WeightedPickWithoutReplacement(nonBombs, count));
        Shuffle(selected);

        return selected;
    }

    private List<SliceDataSO> WeightedPickWithoutReplacement(List<SliceDataSO> pool, int count)
    {
        var available = new List<SliceDataSO>(pool);
        var result    = new List<SliceDataSO>();
        count = Mathf.Min(count, available.Count);

        for (int i = 0; i < count; i++)
        {
            float total = available.Sum(s => s.Weight);
            float roll  = Random.Range(0f, total);
            float cumulative = 0f;

            for (int j = 0; j < available.Count; j++)
            {
                cumulative += available[j].Weight;
                if (roll <= cumulative)
                {
                    result.Add(available[j]);
                    available.RemoveAt(j);
                    break;
                }
            }
        }

        return result;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private void OnValidate()
    {
        if (_bronzeWheelRoot == null)
            _bronzeWheelRoot = transform.Find("ui_image_spin_bronze")?.gameObject;
        if (_silverWheelRoot == null)
            _silverWheelRoot = transform.Find("ui_image_spin_silver")?.gameObject;
        if (_goldenWheelRoot == null)
            _goldenWheelRoot = transform.Find("ui_image_spin_golden")?.gameObject;
    }
}
