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
        // OnRevealComplete'ten önce çağrıldığı için inventory henüz güncellenmemiş;
        // gelen miktarı manuel ekleyerek doğru değeri hesapla.
        int newAmount = GameManager.Instance.RewardInventory.GetRewardAmount(type) + data.RewardAmount;

        if (_items.TryGetValue(type, out var existing))
        {
            existing.AnimateAmountTo(newAmount);
        }
        else
        {
            var item = Instantiate(_itemPrefab, _contentParent);
            item.Initialize(data.Icon, type, newAmount);
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
        GameManager.Instance.OnGameReset -= ClearAll;
    }

}
