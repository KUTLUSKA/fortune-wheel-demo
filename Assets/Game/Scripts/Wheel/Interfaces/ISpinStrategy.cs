public interface ISpinStrategy
{
    bool HasBomb { get; }
    bool CanPlayerLeave { get; }
    WheelConfigSO GetWheelConfig();
}