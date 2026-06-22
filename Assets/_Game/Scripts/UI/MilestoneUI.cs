using UnityEngine;
using TMPro;
using DG.Tweening;

public class MilestoneUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nextSafeZoneText;
    [SerializeField] private TextMeshProUGUI _nextSuperZoneText;
    [SerializeField] private RectTransform _safeIcon;
    [SerializeField] private RectTransform _superIcon;

    private void Start()
    {
        GameManager.Instance.StateController.OnStateChanged += OnStateChanged;
        GameManager.Instance.OnGameReset += Refresh;
        Refresh();
    }

    private void OnDestroy()
    {
        GameManager.Instance.StateController.OnStateChanged -= OnStateChanged;
        GameManager.Instance.OnGameReset -= Refresh;
    }

    private void OnStateChanged(GameState from, GameState to)
    {
        if (to == GameState.ZoneTransition)
            Refresh();
    }

    private void Refresh()
    {
        var zm = GameManager.Instance.ZoneManager;
        int current = zm.CurrentZone;

        int nextSafe  = FindNextZone(current, zm.IsZoneSafe);
        int nextSuper = FindNextZone(current, zm.IsZoneSuper);

        SetZoneText(_nextSafeZoneText, _safeIcon, nextSafe);
        SetZoneText(_nextSuperZoneText, _superIcon, nextSuper);
    }

    private void SetZoneText(TextMeshProUGUI label, RectTransform icon, int zone)
    {
        label.text = zone > 0 ? zone.ToString() : "-";
        icon.DOKill();
        icon.localScale = Vector3.one * 1.2f;
        icon.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
    }

    private int FindNextZone(int from, System.Func<int, bool> predicate)
    {
        for (int i = from + 1; i <= from + 50; i++)
            if (predicate(i)) return i;
        return -1;
    }

}
