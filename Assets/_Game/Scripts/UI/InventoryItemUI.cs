using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _amountText;

    private RewardType _type;
    private int _displayedAmount;

    public RewardType RewardType => _type;
    public RectTransform IconRect => _iconImage.rectTransform;

    public void Initialize(Sprite icon, RewardType type, int amount)
    {
        _type = type;
        _iconImage.sprite = icon;
        _iconImage.transform.localScale = type == RewardType.Points ? Vector3.one * 0.8f : Vector3.one;
        _displayedAmount = amount;
        _amountText.text = amount > 0 ? $"x{amount}" : "";

        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }

    public void AnimateAmountTo(int newAmount)
    {
        DOTween.To(() => _displayedAmount, x =>
        {
            _displayedAmount = x;
            _amountText.text = $"x{x}";
        }, newAmount, 0.4f).SetEase(Ease.OutQuad);

        transform.DOScale(1.2f, 0.15f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

}
