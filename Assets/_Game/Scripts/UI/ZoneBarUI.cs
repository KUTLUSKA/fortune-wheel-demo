using UnityEngine;
using DG.Tweening;

public class ZoneBarUI : MonoBehaviour
{
    [SerializeField] private ZoneItemUI    _zoneItemPrefab;
    [SerializeField] private Transform     _content;
    [SerializeField] private RectTransform _cursor;
    [SerializeField] private int           _zoneCount    = 50;
    [SerializeField] private int           _paddingCount = 5;

    private ZoneItemUI[]  _items;
    private float         _itemWidth;
    private RectTransform _contentRT;
    private RectTransform _zoneItemPrefabRT;

    private void Start()
    {
        _zoneItemPrefabRT = _zoneItemPrefab.GetComponent<RectTransform>();
        BuildZoneBar();
        GameManager.Instance.StateController.OnStateChanged += OnStateChanged;
        GameManager.Instance.OnGameReset += RefreshAndScroll;
    }

    private void OnDestroy()
    {
        GameManager.Instance.StateController.OnStateChanged -= OnStateChanged;
        GameManager.Instance.OnGameReset -= RefreshAndScroll;
    }

    private void OnStateChanged(GameState from, GameState to)
    {
        if (to == GameState.ZoneTransition)
            RefreshAndScroll();
        else if (from == GameState.BombHit && to == GameState.Idle)
            RefreshAndScroll();
    }

    private void BuildZoneBar()
    {
        _itemWidth  = _zoneItemPrefabRT.sizeDelta.x;
        _contentRT  = (RectTransform)_content;

        var zm = GameManager.Instance.ZoneManager;
        _items = new ZoneItemUI[_zoneCount];

        for (int i = 0; i < _paddingCount; i++)
        {
            var pad = Instantiate(_zoneItemPrefab, _content);
            pad.SetAsPadding();
        }

        for (int i = 0; i < _zoneCount; i++)
        {
            int zone = i + 1;
            var item = Instantiate(_zoneItemPrefab, _content);
            item.Initialize(zone, zm.IsZoneSafe(zone), zm.IsZoneSuper(zone), isCurrent: false);
            _items[i] = item;
        }

        MoveToZone(zm.CurrentZone, animate: false);
    }

    private void RefreshAndScroll()
    {
        int current = GameManager.Instance.ZoneManager.CurrentZone;
        MoveToZone(current, animate: true);
    }

    private float GetCursorViewportLocalX()
    {
        if (_cursor == null) return _paddingCount * _itemWidth;

        var viewportRT = (RectTransform)_content.parent;
        Vector3 local = viewportRT.InverseTransformPoint(_cursor.position);
        return local.x + viewportRT.rect.width * viewportRT.pivot.x;
    }

    private void MoveToZone(int zone, bool animate)
    {
        float cursorX = GetCursorViewportLocalX();
        float targetX = cursorX - (_paddingCount + zone - 1) * _itemWidth;

        if (animate)
            _contentRT.DOAnchorPosX(targetX, 0.4f).SetEase(Ease.OutCubic);
        else
            _contentRT.anchoredPosition = new Vector2(targetX, _contentRT.anchoredPosition.y);
    }
}
