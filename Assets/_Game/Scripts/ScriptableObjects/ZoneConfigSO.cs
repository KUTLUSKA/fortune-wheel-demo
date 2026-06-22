using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ZoneConfig", menuName = "WheelOfFortune/ZoneConfig")]
public class ZoneConfigSO : ScriptableObject
{
    [Header("Zone Settings")]
    [SerializeField] private int _safeZoneInterval = 5;
    [SerializeField] private int _superZoneInterval = 30;

    [Header("Wheel Configs")]
    [SerializeField] private List<ZoneGroupConfigSO> _zoneGroups;
    [SerializeField] private WheelConfigSO _silverWheelConfig;
    [SerializeField] private WheelConfigSO _goldenWheelConfig;

    public int SafeZoneInterval => _safeZoneInterval;
    public int SuperZoneInterval => _superZoneInterval;
    public List<ZoneGroupConfigSO> ZoneGroups => _zoneGroups;
    public WheelConfigSO SilverWheelConfig => _silverWheelConfig;
    public WheelConfigSO GoldenWheelConfig => _goldenWheelConfig;
}