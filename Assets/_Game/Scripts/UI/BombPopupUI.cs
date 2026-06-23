using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BombPopupUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _panel;
    [SerializeField] private Image _flashImage;
    [SerializeField] private Button _giveUpButton;
    [SerializeField] private Button _reviveButton;
    [SerializeField] private TextMeshProUGUI _reviveCostText;
    [SerializeField] private Camera _mainCamera;

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
        SoundManager.Instance.Play("BombEffect");

        bool canAfford = GameManager.Instance.CanAffordRevive;
        _reviveButton.interactable = canAfford;
        _reviveCostText.text = CurrencyManager.Instance.ReviveCost.ToString("N0");

        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
        _panel.localScale = Vector3.one * 0.75f;

        _mainCamera.DOShakePosition(0.4f, strength: 18f, vibrato: 14, randomness: 90f);
        _flashImage.DOFade(0.55f, 0.1f).OnComplete(() => _flashImage.DOFade(0f, 0.35f));
        _canvasGroup.DOFade(1f, 0.25f);
        _panel.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
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
