using UnityEngine;
using DG.Tweening;

public class WheelSpinHandler : MonoBehaviour
{
    [SerializeField] private WheelController _wheelController;

    private SliceDataSO _pendingResult;
    private float _accumAngle;

    private void Start()
    {
        GameManager.Instance.OnSpinResultEvaluated += OnSpinResultEvaluated;
        GameManager.Instance.StateController.OnStateChanged += OnStateChanged;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnSpinResultEvaluated -= OnSpinResultEvaluated;
        GameManager.Instance.StateController.OnStateChanged -= OnStateChanged;
    }

    private void OnSpinResultEvaluated(SliceDataSO result)
    {
        _pendingResult = result;
    }

    private void OnStateChanged(GameState from, GameState to)
    {
        if (to == GameState.Spinning)
            PlaySpinAnimation();
    }

    private void PlaySpinAnimation()
    {
        if (_pendingResult == null) return;

        var slices = _wheelController.CurrentSlices;
        int winnerIndex = FindWinnerIndex(slices);

        float sliceAngle = 360f / slices.Count;
        float extraAngle = (360f - winnerIndex * sliceAngle) % 360f;
        float spinDuration = Random.Range(
            _wheelController.CurrentConfig.MinSpinDuration,
            _wheelController.CurrentConfig.MaxSpinDuration
        );

        Transform wheelRoot = _wheelController.ActiveWheelTransform;

        // Pullback anticipation
        float pullbackTarget = _accumAngle - 15f;
        DOTween.To(() => _accumAngle, SetWheelAngle, pullbackTarget, 0.15f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                float spinTarget = _accumAngle + (5 * 360f) + extraAngle;
                DOTween.To(() => _accumAngle, SetWheelAngle, spinTarget, spinDuration)
                    .SetEase(Ease.OutQuart)
                    .OnComplete(() =>
                    {
                        _accumAngle %= 360f;
                        slices[winnerIndex].SetHighlight(true);
                        GameManager.Instance.OnAnimationComplete();
                    });
            });

        void SetWheelAngle(float angle)
        {
            _accumAngle = angle;
            wheelRoot.localRotation = Quaternion.Euler(0f, 0f, -angle);
        }
    }

    private int FindWinnerIndex(System.Collections.Generic.IReadOnlyList<SliceView> slices)
    {
        for (int i = 0; i < slices.Count; i++)
            if (slices[i].Data == _pendingResult)
                return i;
        return 0;
    }

    private void OnValidate()
    {
        if (_wheelController == null)
            _wheelController = GetComponentInParent<WheelController>();
    }
}