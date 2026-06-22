using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WheelConfig", menuName = "WheelOfFortune/WheelConfig")]
public class WheelConfigSO : ScriptableObject
{
    [System.Serializable]
    public struct SpinChanceEntry
    {
        public SliceDataSO Slice;
        [Range(0f, 100f)] public float Weight;
    }

    [Header("Wheel Settings")]
    [SerializeField] private WheelType _wheelType;
    [SerializeField] private Sprite _wheelSprite;

    [Header("Spin Settings")]
    [SerializeField] private float _minSpinDuration = 3f;
    [SerializeField] private float _maxSpinDuration = 5f;

    [Header("Slice Pool")]
    [Tooltip("Full slice pool. Weight on each SliceData controls how likely it is to appear in the 8 shown.")]
    [SerializeField] private List<SliceDataSO> _slices = new List<SliceDataSO>();
    [SerializeField] private int _slicesPerSpin = 8;

    [Header("Spin Result Weights")]
    [Tooltip("Defines win probability per slice for this zone. Only slices actually displayed are considered.")]
    [SerializeField] private List<SpinChanceEntry> _spinChances = new List<SpinChanceEntry>();

    public WheelType WheelType => _wheelType;
    public Sprite WheelSprite => _wheelSprite;
    public float MinSpinDuration => _minSpinDuration;
    public float MaxSpinDuration => _maxSpinDuration;
    public List<SliceDataSO> SlicePool => _slices;
    public int SlicesPerSpin => _slicesPerSpin;
    public IReadOnlyList<SpinChanceEntry> SpinChances => _spinChances;
}