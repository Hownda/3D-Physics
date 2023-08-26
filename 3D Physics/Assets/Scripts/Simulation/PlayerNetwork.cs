using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Threading;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Rendering;

public class PlayerNetwork : NetworkBehaviour
{
    private Vector2 moveInput;
    private MovementControls inputActions;

    private int tick = 0;
    private const int buffer = 1024;
    const ushort MAX_FRAME_BUFFER = 8;
    private GameState current;
    private Dictionary<int, GameState> GameStateDict = new();

    private HandleStates.InputState[] _inputStates = new HandleStates.InputState[buffer];
    private HandleStates.TransformStateRW[] _transformStates = new HandleStates.TransformStateRW[buffer];
    public NetworkVariable<HandleStates.TransformStateRW> currentServerTransformState = new();
    public HandleStates.TransformStateRW previousTransformState;

    private long prev = System.DateTime.UtcNow.Ticks;
    private long lag = 0;
    private int test = -2;
    private bool run = false;

    private void Start()
    {
        if (IsLocalPlayer)
        {
            inputActions = new MovementControls();
            inputActions.Player.Enable();
            current = new GameState(moveInput, transform.position, transform.rotation, tick);
            InitializeGameStateServerRpc(moveInput, transform.position, transform.rotation, tick);
            run = true;
        }
    }

    readonly static long TICKS_PER_FRAME = 166667; //16.67ms for 60fps
    private void Update()
    {
        if (IsLocalPlayer)
        {
            moveInput = inputActions.Player.Movement.ReadValue<Vector2>();

            if (run) //update loop
            {
                long now = System.DateTime.UtcNow.Ticks;
                long elapsed = now - prev;
                prev = now;
                lag += elapsed;
                while (lag >= TICKS_PER_FRAME) //lets us update many times if we lag behind
                {
                    test = tick;
                    lag -= TICKS_PER_FRAME;
                    //handle rollbacks
                    //if (RollbackFrames.Count > 0) current = HandleRollbacks();
                    //get inputs for this frame
                    //FrameInputDictionary.TryGetValue((ushort)current.frameID, out InputSerialization.FrameInfo frameInputs);
                    //predict remote inputs
                    //PredictRemoteInputs(current.tick - LastRemoteFrame);
                    //store gamestate in buffer
                    GameStateDict[current.tick] = new GameState(current);
                    //update gamestate
                    MovePlayerWithServerTickServerRpc(current.tick, moveInput);
                    ProcessLocalPlayerMovement(moveInput);
                    current = current.Tick(moveInput, transform.position, transform.rotation);
                    //cleanup
                    ushort earliestBufferedFrame = (ushort)(current.tick - MAX_FRAME_BUFFER);
                    GameStateDict.Remove(earliestBufferedFrame);

                }
            }
        }
    }
    
    private void ProcessLocalPlayerMovement(Vector2 moveInput)
    {
        transform.position += new Vector3(moveInput.x, 0, moveInput.y) * 5 * TICKS_PER_FRAME / 10000000 ;
    }

    [ServerRpc] private void InitializeGameStateServerRpc(Vector2 moveInput, Vector3 position, Quaternion rotation, int tick)
    {
        current = new GameState(moveInput, transform.position, transform.rotation, tick);
    }

    [ServerRpc] private void MovePlayerWithServerTickServerRpc(int tick, Vector2 moveInput)
    {
        if (GameStateDict.ContainsKey(tick))
        {
            GameStateDict[current.tick] = new GameState(current);
        }
        else
        {
            GameStateDict.Add(current.tick, new GameState(current));
        }
        transform.position += new Vector3(moveInput.x, 0, moveInput.y) * 5 * TICKS_PER_FRAME / 10000000;
        current = current.Tick(moveInput, transform.position, transform.rotation);    
    }
}
