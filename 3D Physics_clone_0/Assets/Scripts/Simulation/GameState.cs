using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameState
{
    public HandleStates.InputState inputState = new();
    public HandleStates.TransformStateRW transformStateRW = new();
    public int tick = 0;

    public GameState(Vector2 moveInput, Vector3 position, Quaternion rotation, int tick)
    {
        this.inputState.moveInput = moveInput;
        this.transformStateRW.finalPosition = position;
        this.transformStateRW.finalRotation = rotation;
        this.tick = tick;
    }

    public GameState(GameState gameState)
    {
        this.tick = gameState.tick;
        this.inputState = gameState.inputState;
        this.transformStateRW = gameState.transformStateRW;
    }

    public GameState Tick(Vector2 moveInput, Vector3 position, Quaternion rotation)
    {
        this.tick++;
        this.inputState.moveInput = moveInput;
        this.transformStateRW.finalPosition = position;
        this.transformStateRW.finalRotation = rotation;

        return this;

    }
}
