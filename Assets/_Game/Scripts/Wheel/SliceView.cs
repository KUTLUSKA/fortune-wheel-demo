using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SliceView : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _rewardText;
    [SerializeField] private Image _highlightOverlay;

    public SliceDataSO Data { get; private set; }
    public RectTransform IconRect => _iconImage.rectTransform;

    public void Initialize(SliceDataSO data)
    {
        Data = data;
        _iconImage.sprite = data.Icon;
        _rewardText.text = data.IsBomb ? "" : FormatReward(data);

        var fitter = _iconImage.GetComponent<AspectRatioFitter>();
        if (fitter == null)
            fitter = _iconImage.gameObject.AddComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        if (data.Icon != null)
            fitter.aspectRatio = (float)data.Icon.rect.width / Mathf.Max(1f, data.Icon.rect.height);
    }

    public void AnimateIn(float delay = 0f)
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack).SetDelay(delay);
    }

    public void AnimateOut(float delay = 0f)
    {
        transform.DOKill();
        transform.DOScale(0f, 0.18f).SetEase(Ease.InBack).SetDelay(delay);
    }

    public void SetHighlight(bool isWinner)
    {
        _highlightOverlay.DOKill();
        transform.DOKill();

        if (!isWinner)
        {
            _highlightOverlay.DOFade(0f, 0.2f);
            return;
        }

        _highlightOverlay.DOFade(1f, 0.3f);
        transform.DOScale(1.15f, 0.25f).SetLoops(4, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    public void ResetVisual()
    {
        _highlightOverlay.DOKill();
        transform.DOKill();
        _highlightOverlay.color = new Color(1f, 1f, 1f, 0f);
        transform.localScale = Vector3.one;
    }

    private string FormatReward(SliceDataSO data)
    {
        return data.RewardAmount > 0 ? $"x{data.RewardAmount}" : data.SliceName;
    }

}
