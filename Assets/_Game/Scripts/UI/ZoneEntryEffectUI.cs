using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ZoneEntryEffectUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _safeEffectGroup;
    [SerializeField] private CanvasGroup _superEffectGroup;
    [SerializeField] private Image _glowImage;
    [SerializeField] private Image _flashImage;
    [SerializeField] private Image _shineImage;

    private void Start()
    {
        GameManager.Instance.StateController.OnStateChanged += OnStateChanged;
        _safeEffectGroup.alpha  = 0f;
        _superEffectGroup.alpha = 0f;
    }

    private void OnDestroy()
    {
        GameManager.Instance.StateController.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(GameState from, GameState to)
    {
        if (to != GameState.ZoneTransition) return;

        var zm = GameManager.Instance.ZoneManager;

        if (zm.IsSuperZone)
            PlayEffect(_superEffectGroup);
        else if (zm.IsSafeZone)
            PlayEffect(_safeEffectGroup);
    }

    private void PlayEffect(CanvasGroup group)
    {
        group.DOKill();
        group.alpha = 0f;

        DOTween.Sequence()
            .Append(group.DOFade(1f, 0.2f))
            .AppendInterval(0.4f)
            .Append(group.DOFade(0f, 0.3f));

        _glowImage.DOKill();
        _glowImage.transform.localScale = Vector3.one * 0.6f;
        _glowImage.transform.DOScale(1.3f, 0.6f).SetEase(Ease.OutCubic);

        _flashImage.DOKill();
        _flashImage.DOFade(0.8f, 0.05f)
                   .OnComplete(() => _flashImage.DOFade(0f, 0.3f));

        _shineImage.DOKill();
        _shineImage.transform.localEulerAngles = Vector3.zero;
        _shineImage.transform.DORotate(new Vector3(0f, 0f, 360f), 0.8f, RotateMode.FastBeyond360)
                   .SetEase(Ease.OutCubic);
    }

}
