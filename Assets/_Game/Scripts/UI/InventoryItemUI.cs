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

    private void OnValidate()
    {
        if (_iconImage == null)
            _iconImage = transform.Find("ui_image_inventory_icon")?.GetComponent<Image>();
        if (_amountText == null)
            _amountText = transform.Find("ui_text_inventory_amount_value")?.GetComponent<TextMeshProUGUI>();
    }
}
