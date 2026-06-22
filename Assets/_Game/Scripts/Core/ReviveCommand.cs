using System.Collections.Generic;

public class ReviveCommand : ICommand
{
    private readonly RewardInventory _inventory;
    private readonly ZoneManager _zoneManager;
    private readonly Dictionary<RewardType, int> _inventorySnapshot;
    private readonly int _zoneSnapshot;

    public ReviveCommand(RewardInventory inventory, ZoneManager zoneManager,
        Dictionary<RewardType, int> inventorySnapshot, int zoneSnapshot)
    {
        _inventory = inventory;
        _zoneManager = zoneManager;
        _inventorySnapshot = inventorySnapshot;
        _zoneSnapshot = zoneSnapshot;
    }

    public void Execute()
    {
        _inventory.RestoreFromSnapshot(_inventorySnapshot);
        _zoneManager.SetZone(_zoneSnapshot);
    }
}
