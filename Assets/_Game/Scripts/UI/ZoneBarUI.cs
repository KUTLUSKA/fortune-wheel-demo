using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ZoneBarUI : MonoBehaviour
{
    [SerializeField] private ZoneItemUI _zoneItemPrefab;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private Transform _content;
    [SerializeField] private int _zoneCount = 50;

    private ZoneItemUI[] _items;

    private void Start()
    {
        BuildZoneBar();
        GameManager.Instance.StateController.OnStateChanged += OnStateChanged;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StateController.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(GameState from, GameState to)
    {
        if (to == GameState.ZoneTransition)
            RefreshAndScroll();
    }

    private void BuildZoneBar()
    {
        var zm = GameManager.Instance.ZoneManager;
        _items = new ZoneItemUI[_zoneCount];

        for (int i = 0; i < _zoneCount; i++)
        {
            int zone = i + 1;
            var item = Instantiate(_zoneItemPrefab, _content);
            item.Initialize(zone, zm.IsZoneSafe(zone), zm.IsZoneSuper(zone), zone == zm.CurrentZone);
            _items[i] = item;
        }

        ScrollToZone(zm.CurrentZone, animate: false);
    }

    private void RefreshAndScroll()
    {
        int current = GameManager.Instance.ZoneManager.CurrentZone;

        for (int i = 0; i < _items.Length; i++)
            _items[i].SetCurrent(i + 1 == current);

        ScrollToZone(current, animate: true);
    }

    private void ScrollToZone(int zone, bool animate)
    {
        float normalized = Mathf.Clamp01((float)(zone - 1) / Mathf.Max(1, _zoneCount - 1));

        if (animate)
        {
            DOTween.To(
                () => _scrollRect.horizontalNormalizedPosition,
                x => _scrollRect.horizontalNormalizedPosition = x,
                normalized, 0.4f).SetEase(Ease.OutCubic);
        }
        else
        {
            _scrollRect.horizontalNormalizedPosition = normalized;
        }
    }

    private void OnValidate()
    {
        if (_scrollRect == null)
            _scrollRect = GetComponent<ScrollRect>();
        if (_content == null && _scrollRect != null)
            _content = _scrollRect.content;
    }
}
