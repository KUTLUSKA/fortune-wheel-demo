using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [SerializeField] private int _baseReviveCost = 500;
    [SerializeField] private int _costMultiplier = 5;

    private int _reviveCost;

    public int ReviveCost => _reviveCost;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _reviveCost = _baseReviveCost;
    }

    public bool CanAffordRevive(int goldAmount) => goldAmount >= _reviveCost;

    public void OnReviveSpent() => _reviveCost *= _costMultiplier;

    public void ResetCost() => _reviveCost = _baseReviveCost;
}