using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WheelConfig", menuName = "WheelOfFortune/WheelConfig")]
public class WheelConfigSO : ScriptableObject
{
    [System.Serializable]
    public struct SpinChanceEntry
    {
        public SliceDataSO Slice;
        [Range(0f, 100f)] public float AppearWeight;
        [Range(0f, 100f)] public float WinWeight;
    }

    [Header("Wheel Settings")]
    [SerializeField] private WheelType _wheelType;
    [SerializeField] private Sprite _wheelSprite;

    [Header("Spin Settings")]
    [SerializeField] private float _minSpinDuration = 3f;
    [SerializeField] private float _maxSpinDuration = 5f;

    [Header("Slices & Weights")]
    [Tooltip("Slice pool with win weights. SlicesPerSpin of these are randomly drawn (by weight) each round.")]
    [SerializeField] private List<SpinChanceEntry> _spinChances = new List<SpinChanceEntry>();
    [SerializeField] private int _slicesPerSpin = 8;

    public WheelType WheelType => _wheelType;
    public Sprite WheelSprite => _wheelSprite;
    public float MinSpinDuration => _minSpinDuration;
    public float MaxSpinDuration => _maxSpinDuration;
    public int SlicesPerSpin => _slicesPerSpin;
    public IReadOnlyList<SpinChanceEntry> SpinChances => _spinChances;
}