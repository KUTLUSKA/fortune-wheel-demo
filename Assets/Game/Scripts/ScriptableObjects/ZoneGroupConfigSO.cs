using UnityEngine;

[CreateAssetMenu(fileName = "ZoneGroupConfig", menuName = "WheelOfFortune/ZoneGroupConfig")]
public class ZoneGroupConfigSO : ScriptableObject
{
    [Header("Zone Range")]
    [SerializeField] private int _fromZone;
    [SerializeField] private int _toZone;

    [Header("Wheel Config")]
    [SerializeField] private WheelConfigSO _wheelConfig;

    public int FromZone => _fromZone;
    public int ToZone => _toZone;
    public WheelConfigSO WheelConfig => _wheelConfig;
}