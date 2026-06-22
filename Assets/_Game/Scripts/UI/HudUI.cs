using UnityEngine;
using UnityEngine.UI;

public class HudUI : MonoBehaviour
{
    [SerializeField] private Button _spinButton;
    [SerializeField] private Button _leaveButton;

    private void Start()
    {
        _spinButton.onClick.AddListener(OnSpinClicked);
        _leaveButton.onClick.AddListener(OnLeaveClicked);

        GameManager.Instance.StateController.OnStateChanged += OnStateChanged;
        UpdateButtons(GameState.Idle);
    }

    private void OnDestroy()
    {
        _spinButton.onClick.RemoveListener(OnSpinClicked);
        _leaveButton.onClick.RemoveListener(OnLeaveClicked);

        GameManager.Instance.StateController.OnStateChanged -= OnStateChanged;
    }

    private void OnSpinClicked() => GameManager.Instance.StartSpin();
    private void OnLeaveClicked() => GameManager.Instance.OnPlayerLeft();

    private void OnStateChanged(GameState from, GameState to) => UpdateButtons(to);

    private void UpdateButtons(GameState state)
    {
        bool isIdle = state == GameState.Idle;
        _spinButton.interactable = isIdle;
        _leaveButton.interactable = isIdle && GameManager.Instance.GetCurrentStrategy().CanPlayerLeave;
    }

}