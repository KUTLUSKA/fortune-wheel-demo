public class SilverSpinStrategy : ISpinStrategy
{
    private WheelConfigSO _config;

    public SilverSpinStrategy(WheelConfigSO config)
    {
        _config = config;
    }

    public bool HasBomb => false;
    public bool CanPlayerLeave => true;

    public WheelConfigSO GetWheelConfig() => _config;
}