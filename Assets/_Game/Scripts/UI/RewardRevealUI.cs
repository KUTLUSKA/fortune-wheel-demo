using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RewardRevealUI : MonoBehaviour
{
    [SerializeField] private InventoryPanelUI _inventoryPanel;
    [SerializeField] private Image _flyingIcon;
    [SerializeField] private RectTransform _centerPoint;
    [SerializeField] private float _growDuration = 0.35f;
    [SerializeField] private float _holdDuration = 0.7f;
    [SerializeField] private float _flyDuration  = 0.45f;

    private SliceDataSO _pendingResult;

    private void Start()
    {
        _flyingIcon.gameObject.SetActive(false);
        GameManager.Instance.OnSpinResultEvaluated += result => _pendingResult = result;
        GameManager.Instance.StateController.OnStateChanged += OnStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.Instance.StateController.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(GameState from, GameState to)
    {
        if (to == GameState.ShowResult)
            Show();
    }

    private void Show()
    {
        Debug.Log($"[RewardRevealUI] Show — result: {_pendingResult?.SliceName}, isBomb: {_pendingResult?.IsBomb}");

        if (_pendingResult == null || _pendingResult.IsBomb)
        {
            Debug.Log("[RewardRevealUI] Skipping reveal → OnRevealComplete");
            GameManager.Instance.OnRevealComplete();
            return;
        }

        SliceView winner = FindWinnerSlice();
        Vector3 startPos = winner != null ? winner.IconRect.position : _centerPoint.position;

        _flyingIcon.sprite = _pendingResult.Icon;
        _flyingIcon.gameObject.SetActive(true);
        _flyingIcon.transform.position   = startPos;
        _flyingIcon.transform.localScale = Vector3.one * 0.5f;

        _flyingIcon.transform.DOMove(_centerPoint.position, _growDuration).SetEase(Ease.OutCubic);
        _flyingIcon.transform.DOScale(1f, _growDuration).SetEase(Ease.OutBack)
            .OnComplete(() => DOVirtual.DelayedCall(_holdDuration, FlyToInventory));
    }

    private void FlyToInventory()
    {
        Debug.Log("[RewardRevealUI] FlyToInventory");
        Vector3 target = _inventoryPanel.GetItemWorldPosition(_pendingResult.RewardType);

        _flyingIcon.transform.DOMove(target, _flyDuration).SetEase(Ease.InQuart);
        _flyingIcon.transform.DOScale(0.15f, _flyDuration).SetEase(Ease.InQuart)
            .OnComplete(() =>
            {
                _inventoryPanel.AddOrUpdate(_pendingResult);
                _flyingIcon.gameObject.SetActive(false);
                Debug.Log("[RewardRevealUI] → OnRevealComplete");
                GameManager.Instance.OnRevealComplete();
            });
    }

    private SliceView FindWinnerSlice()
    {
        var slices = GameManager.Instance.WheelController.CurrentSlices;
        foreach (var s in slices)
            if (s.Data == _pendingResult) return s;
        return null;
    }

}
