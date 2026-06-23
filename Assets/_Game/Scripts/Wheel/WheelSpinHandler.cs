using UnityEngine;
using DG.Tweening;

public class WheelSpinHandler : MonoBehaviour
{
    [SerializeField] private WheelController _wheelController;

    private SliceDataSO _pendingResult;
    private float _accumAngle;
    private float _lastTickAngle;

    private void Start()
    {
        GameManager.Instance.OnSpinResultEvaluated += OnSpinResultEvaluated;
        GameManager.Instance.StateController.OnStateChanged += OnStateChanged;
    }

    private void OnDestroy()
    {
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
        float spinDuration = Random.Range(
            _wheelController.CurrentConfig.MinSpinDuration,
            _wheelController.CurrentConfig.MaxSpinDuration
        );

        Transform wheelRoot = _wheelController.ActiveWheelTransform;

        // Slot i starts at i*sliceAngle clockwise from top.
        // To land slot i at the top, wheel must rotate clockwise by (360 - i*sliceAngle) % 360.
        float targetMod = (360f - winnerIndex * sliceAngle) % 360f;

        _lastTickAngle = _accumAngle;
        float pullbackTarget = _accumAngle - 15f;
        DOTween.To(() => _accumAngle, SetWheelAngle, pullbackTarget, 0.15f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // Delta from current position to target, calculated after pullback
                float currentMod = ((_accumAngle % 360f) + 360f) % 360f;
                float delta = (targetMod - currentMod + 360f) % 360f;
                if (delta < 1f) delta += 360f;

                float spinTarget = _accumAngle + (4 * 360f) + delta;
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

            if (Mathf.Abs(angle - _lastTickAngle) >= 44f)
            {
                _lastTickAngle = Mathf.Floor(angle / 44f) * 44f;
                SoundManager.Instance.Play(SoundKeys.WheelTurn);
            }
        }
    }

    private int FindWinnerIndex(System.Collections.Generic.IReadOnlyList<SliceView> slices)
    {
        for (int i = 0; i < slices.Count; i++)
            if (slices[i].Data == _pendingResult)
                return i;
        return 0;
    }

}