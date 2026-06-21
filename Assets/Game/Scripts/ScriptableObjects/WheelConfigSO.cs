using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WheelConfig", menuName = "WheelOfFortune/WheelConfig")]
public class WheelConfigSO : ScriptableObject
{
    [Header("Wheel Settings")]
    [SerializeField] private WheelType _wheelType;
    [SerializeField] private Sprite _wheelSprite;

    [Header("Spin Settings")]
    [SerializeField] private float _minSpinDuration = 3f;
    [SerializeField] private float _maxSpinDuration = 5f;

    [Header("Slices")]
    [SerializeField] private List<SliceDataSO> _slices = new List<SliceDataSO>();

    public WheelType WheelType => _wheelType;
    public Sprite WheelSprite => _wheelSprite;
    public float MinSpinDuration => _minSpinDuration;
    public float MaxSpinDuration => _maxSpinDuration;
    public List<SliceDataSO> Slices => _slices;
}