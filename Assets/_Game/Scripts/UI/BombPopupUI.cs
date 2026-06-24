using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BombPopupUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup      _canvasGroup;
    [SerializeField] private RectTransform    _panel;
    [SerializeField] private Image            _flashImage;
    [SerializeField] private Button           _giveUpButton;
    [SerializeField] private Button           _reviveButton;
    [SerializeField] private TextMeshProUGUI  _reviveCostText;
    [SerializeField] private Camera           _mainCamera;

#if UNITY_EDITOR
    private void OnValidate()
    {
        _canvasGroup    ??= GetComponent<CanvasGroup>();
        _flashImage     ??= FindChild<Image>("ui_image_bomb_flash");
        _panel          ??= FindChild<RectTransform>("ui_panel_bomb_popup");
        _giveUpButton   ??= FindChild<Button>("ui_button_give_up_action");
        _reviveButton   ??= FindChild<Button>("ui_button_revive_action");
        _reviveCostText ??= FindChild<TextMeshProUGUI>("ui_text_revive_count_value");
        _mainCamera     ??= Camera.main;
    }

    private T FindChild<T>(string childName) where T : Component
    {
        var t = transform.Find(childName);
        if (t == null)
        {
            var all = GetComponentsInChildren<T>(true);
            foreach (var c in all)
                if (c.gameObject.name == childName) return c;
            return null;
        }
        return t.GetComponent<T>();
    }
#endif

    private const float PanelInitialScale = 0.75f;
    private const float PanelInDuration   = 0.3f;
    private const float ShakeDuration     = 0.4f;
    private const float ShakeStrength     = 18f;
    private const int   ShakeVibrato      = 14;
    private const float ShakeRandomness   = 90f;
    private const float FlashPeakAlpha    = 0.55f;
    private const float FlashInDuration   = 0.1f;
    private const float FlashOutDuration  = 0.35f;

    private void Start()
    {
        _giveUpButton.onClick.AddListener(OnGiveUpClicked);
        _reviveButton.onClick.AddListener(OnReviveClicked);
        GameManager.Instance.StateController.OnStateChanged += OnStateChanged;

        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _flashImage.color = new Color(1f, 0f, 0f, 0f);
    }

    private void OnDestroy()
    {
        _giveUpButton.onClick.RemoveListener(OnGiveUpClicked);
        _reviveButton.onClick.RemoveListener(OnReviveClicked);

        GameManager.Instance.StateController.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(GameState from, GameState to)
    {
        if (to == GameState.BombHit)
            Show();
        else if (from == GameState.BombHit)
            Hide();
    }

    private void Show()
    {
        SoundManager.Instance.Play(SoundKeys.BombEffect);

        bool canAfford = GameManager.Instance.CanAffordRevive;
        _reviveButton.interactable = canAfford;
        _reviveCostText.text = CurrencyManager.Instance.ReviveCost.ToString("N0");

        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
        _panel.localScale = Vector3.one * PanelInitialScale;

        _mainCamera.DOShakePosition(ShakeDuration, strength: ShakeStrength, vibrato: ShakeVibrato, randomness: ShakeRandomness);
        _flashImage.DOFade(FlashPeakAlpha, FlashInDuration).OnComplete(() => _flashImage.DOFade(0f, FlashOutDuration));
        _canvasGroup.DOFade(1f, 0.25f);
        _panel.DOScale(1f, PanelInDuration).SetEase(Ease.OutBack);
    }

    private void Hide()
    {
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        _canvasGroup.DOFade(0f, 0.15f);
    }

    private void OnGiveUpClicked() => GameManager.Instance.ResetGame();

    private void OnReviveClicked()
    {
        if (!GameManager.Instance.CanAffordRevive) return;
        GameManager.Instance.RequestRevive();
    }
}
