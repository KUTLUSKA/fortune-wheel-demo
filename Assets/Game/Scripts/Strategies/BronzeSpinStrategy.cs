public class BronzeSpinStrategy : ISpinStrategy
{
    private WheelConfigSO _config;

    public BronzeSpinStrategy(WheelConfigSO config)
    {
        _config = config;
    }

    public bool HasBomb => true;
    public bool CanPlayerLeave => false;

    public WheelConfigSO GetWheelConfig() => _config;
}