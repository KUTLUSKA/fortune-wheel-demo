using UnityEngine;
using System.Collections.Generic;

public class InventoryPanelUI : MonoBehaviour
{
    [SerializeField] private InventoryItemUI _itemPrefab;
    [SerializeField] private Transform _contentParent;

    private readonly Dictionary<SliceDataSO, InventoryItemUI> _items = new();

    private SliceDataSO ResolveKey(SliceDataSO data) =>
        RewardStackingHelper.ResolveKey(data, _items);

    public bool HasItem(SliceDataSO slice) => _items.ContainsKey(ResolveKey(slice));

    public Vector3 GetItemWorldPosition(SliceDataSO slice)
    {
        if (_items.TryGetValue(ResolveKey(slice), out var item))
            return item.IconRect.position;
        return _contentParent.position;
    }

    public void AddOrUpdate(SliceDataSO data)
    {
        var key = ResolveKey(data);
        int newAmount = GameManager.Instance.RewardInventory.GetRewardAmount(data) + data.RewardAmount;

        if (_items.TryGetValue(key, out var existing))
        {
            existing.AnimateAmountTo(newAmount);
        }
        else
        {
            var item = Instantiate(_itemPrefab, _contentParent);
            item.Initialize(data.Icon, data.RewardType, newAmount);
            _items[key] = item;
        }
    }

    public void ClearAll()
    {
        foreach (Transform child in _contentParent)
            Destroy(child.gameObject);
        _items.Clear();
    }

    private void Start()
    {
        GameManager.Instance.OnGameReset += ClearAll;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGameReset -= ClearAll;
    }
}
