using UnityEngine;
using DG.Tweening;

public class ZoneTransitionUI : MonoBehaviour
{
    [SerializeField] private float _transitionDuration = 0.8f;

    private void Start()
    {
        gameObject.SetActive(false);
        GameManager.Instance.StateController.OnStateChanged += OnStateChanged;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StateController.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(GameState from, GameState to)
    {
        if (to == GameState.ZoneTransition)
            Transition();
    }

    private void Transition()
    {
        gameObject.SetActive(true);

        DOVirtual.DelayedCall(_transitionDuration, () =>
        {
            gameObject.SetActive(false);
            GameManager.Instance.OnZoneTransitionComplete();
        });
    }
}