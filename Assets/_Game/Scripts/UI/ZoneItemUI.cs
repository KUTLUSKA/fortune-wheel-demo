using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ZoneItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _zoneNumberText;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _safeSprite;
    [SerializeField] private Sprite _superSprite;
    [SerializeField] private Sprite _currentSprite;

    private Sprite _defaultSprite;

    public void Initialize(int zone, bool isSafe, bool isSuper, bool isCurrent)
    {
        _zoneNumberText.text = zone.ToString();

        _defaultSprite = isSuper ? _superSprite :
                         isSafe  ? _safeSprite  :
                                   _normalSprite;

        _backgroundImage.sprite = isCurrent ? _currentSprite : _defaultSprite;
    }

    public void SetCurrent(bool isCurrent)
    {
        _backgroundImage.sprite = isCurrent ? _currentSprite : _defaultSprite;
    }

    private void OnValidate()
    {
        if (_zoneNumberText == null)
            _zoneNumberText = transform.Find("ui_text_zone_number_value")?.GetComponent<TextMeshProUGUI>();
        if (_backgroundImage == null)
            _backgroundImage = GetComponent<Image>();
    }
}
