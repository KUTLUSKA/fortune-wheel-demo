using UnityEngine;
using System;

public class GameStateController : MonoBehaviour
{
    public GameState CurrentState { get; private set; } = GameState.Idle;

    public event Action<GameState, GameState> OnStateChanged;

    public void TransitionTo(GameState next)
    {
        if (!IsValidTransition(CurrentState, next))
        {
            Debug.LogWarning($"[GameStateController] Invalid transition: {CurrentState} → {next}");
            return;
        }

        GameState previous = CurrentState;
        CurrentState = next;
        OnStateChanged?.Invoke(previous, next);
    }

    private bool IsValidTransition(GameState from, GameState to)
    {
        return (from, to) switch
        {
            (GameState.Idle,           GameState.Spinning)       => true,
            (GameState.Spinning,       GameState.ShowResult)     => true,
            (GameState.ShowResult,     GameState.ZoneTransition) => true,
            (GameState.ShowResult,     GameState.BombHit)        => true,
            (GameState.ZoneTransition, GameState.Idle)           => true,
            (GameState.BombHit,        GameState.GameOver)       => true,
            (GameState.BombHit,        GameState.Idle)           => true,
            (GameState.GameOver,       GameState.Idle)           => true,
            _ => false
        };
    }
}
