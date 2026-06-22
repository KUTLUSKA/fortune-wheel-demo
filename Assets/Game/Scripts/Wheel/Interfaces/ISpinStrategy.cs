public interface ISpinStrategy
{
    bool HasBomb { get; }
    bool CanPlayerLeave { get; }
    WheelType WheelType { get; }
    WheelConfigSO GetWheelConfig();
}