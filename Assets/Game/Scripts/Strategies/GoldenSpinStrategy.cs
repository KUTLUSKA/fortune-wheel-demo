public class GoldenSpinStrategy : ISpinStrategy
{
    private WheelConfigSO _config;

    public GoldenSpinStrategy(WheelConfigSO config)
    {
        _config = config;
    }

    public bool HasBomb => false;
    public bool CanPlayerLeave => true;
    public WheelType WheelType => WheelType.Golden;

    public WheelConfigSO GetWheelConfig() => _config;
}