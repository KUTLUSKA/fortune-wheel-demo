using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [SerializeField] private ZoneConfigSO _zoneConfig;

    private int _currentZone = 1;

    public int CurrentZone => _currentZone;

    public bool IsSafeZone => _currentZone % _zoneConfig.SafeZoneInterval == 0;
    public bool IsSuperZone => _currentZone % _zoneConfig.SuperZoneInterval == 0;

    public void AdvanceZone()
    {
        _currentZone++;
    }

    public WheelConfigSO GetCurrentWheelConfig()
    {
        if (IsSuperZone)
            return _zoneConfig.GoldenWheelConfig;

        if (IsSafeZone)
            return _zoneConfig.SilverWheelConfig;

        foreach (var group in _zoneConfig.ZoneGroups)
        {
            if (_currentZone >= group.FromZone && _currentZone <= group.ToZone)
                return group.WheelConfig;
        }

        return _zoneConfig.ZoneGroups[0].WheelConfig;
    }

    public void ResetZone()
    {
        _currentZone = 1;
    }

    public void SetZone(int zone)
    {
        _currentZone = Mathf.Max(1, zone);
    }
}