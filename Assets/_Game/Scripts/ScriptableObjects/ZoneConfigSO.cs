using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ZoneConfig", menuName = "WheelOfFortune/ZoneConfig")]
public class ZoneConfigSO : ScriptableObject
{
    [Header("Zone Settings")]
    [SerializeField] private int _safeZoneInterval = 5;
    [SerializeField] private int _superZoneInterval = 30;
    [SerializeField] private int _totalZones = 50;

    [Header("Bomb Progression")]
    [Tooltip("Bomb win chance (%) at zone 1")]
    [Range(0f, 100f)] [SerializeField] private float _bombWinChanceMin = 5f;
    [Tooltip("Bomb win chance (%) at the final zone")]
    [Range(0f, 100f)] [SerializeField] private float _bombWinChanceMax = 35f;

    [Header("Wheel Configs")]
    [SerializeField] private List<ZoneGroupConfigSO> _zoneGroups;
    [SerializeField] private WheelConfigSO _silverWheelConfig;
    [SerializeField] private WheelConfigSO _goldenWheelConfig;

    public int SafeZoneInterval => _safeZoneInterval;
    public int SuperZoneInterval => _superZoneInterval;
    public int TotalZones => _totalZones;
    public float BombWinChanceMin => _bombWinChanceMin;
    public float BombWinChanceMax => _bombWinChanceMax;
    public List<ZoneGroupConfigSO> ZoneGroups => _zoneGroups;
    public WheelConfigSO SilverWheelConfig => _silverWheelConfig;
    public WheelConfigSO GoldenWheelConfig => _goldenWheelConfig;
}