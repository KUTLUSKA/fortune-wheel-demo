using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [SerializeField] private int _initialRevives = 1;

    private int _revives;

    public int Revives => _revives;
    public bool CanRevive => _revives > 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        _revives = _initialRevives;
    }

    public void SpendRevive() => _revives = Mathf.Max(0, _revives - 1);
    public void AddRevive(int amount = 1) => _revives += amount;
}
