using UnityEngine;
using System.Collections.Generic;

public class InventoryPanelUI : MonoBehaviour
{
    [SerializeField] private InventoryItemUI _itemPrefab;
    [SerializeField] private Transform _contentParent;

    private readonly Dictionary<RewardType, InventoryItemUI> _items = new();

    public bool HasItem(RewardType type) => _items.ContainsKey(type);

    public Vector3 GetItemWorldPosition(RewardType type)
    {
        if (_items.TryGetValue(type, out var item))
            return item.IconRect.position;
        return _contentParent.position;
    }

    public void AddOrUpdate(SliceDataSO data)
    {
        var type = data.RewardType;
        int currentAmount = GameManager.Instance.RewardInventory.GetRewardAmount(type);

        if (_items.TryGetValue(type, out var existing))
        {
            existing.AnimateAmountTo(currentAmount);
        }
        else
        {
            var item = Instantiate(_itemPrefab, _contentParent);
            item.Initialize(data.Icon, type, currentAmount);
            _items[type] = item;
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
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameReset -= ClearAll;
    }

    private void OnValidate()
    {
        if (_contentParent == null)
            _contentParent = transform.Find("Viewport/Content");
    }
}
