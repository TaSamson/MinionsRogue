using System;
using UnityEngine;

public enum GameState
{
    HeroSelection,
    FloorMapView,
    RoomTransition,
    Combat,
    Shop,
    GameOver
}

public class GameStateMachine
{
    public GameState CurrentState { get; private set; }
    public event Action<GameState, GameState> OnStateChanged;

    public void ChangeState(GameState newState)
    {
        var old = CurrentState;
        CurrentState = newState;
        OnStateChanged?.Invoke(old, newState);
    }
}