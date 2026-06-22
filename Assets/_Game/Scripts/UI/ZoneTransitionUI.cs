using UnityEngine;
using DG.Tweening;

public class ZoneTransitionUI : MonoBehaviour
{
    [SerializeField] private float _transitionDuration = 0.8f;

    private void Start()
    {
        GameManager.Instance.StateController.OnStateChanged += OnStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.Instance.StateController.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(GameState from, GameState to)
    {
        if (to == GameState.ZoneTransition)
            Transition();
    }

    private void Transition()
    {
        Debug.Log("[ZoneTransitionUI] Transition started");
        gameObject.SetActive(true);

        DOVirtual.DelayedCall(_transitionDuration, () =>
        {
            gameObject.SetActive(false);
            Debug.Log("[ZoneTransitionUI] → OnZoneTransitionComplete");
            GameManager.Instance.OnZoneTransitionComplete();
        });
    }
}
