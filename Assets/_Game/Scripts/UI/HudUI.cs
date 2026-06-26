using UnityEngine;
using UnityEngine.UI;

public class HudUI : MonoBehaviour
{
    [SerializeField] private Button _spinButton;
    [SerializeField] private Button _leaveButton;

#if UNITY_EDITOR
    private void OnValidate()
    {
        _spinButton ??= GetButton("ui_button_spin_action");
        _leaveButton ??= GetButton("ui_button_leave_action");
    }

    private Button GetButton(string childName)
    {
        foreach (var b in GetComponentsInChildren<Button>(true))
            if (b.gameObject.name == childName) return b;
        return null;
    }
#endif

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
    private void OnLeaveClicked()
    {
        SoundManager.Instance.Play(SoundKeys.ClickSound);
        GameManager.Instance.OnPlayerLeft();
    }

    private void OnStateChanged(GameState from, GameState to) => UpdateButtons(to);

    private void UpdateButtons(GameState state)
    {
        bool isIdle = state == GameState.Idle;
        _spinButton.interactable = isIdle;
        _leaveButton.interactable = isIdle && GameManager.Instance.GetCurrentStrategy().CanPlayerLeave;
    }

}