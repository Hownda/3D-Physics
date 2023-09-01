using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PlayerNetwork : NetworkBehaviour
{
    private Vector2 moveInput;
    private MovementControls inputActions;
    private PhysicsObject physicsObject;
    private float movementSpeed = 5;

    private float tickDeltaTime = 0;
    private const int buffer = 1024;
    private const float tickRate = 1f / 60f;
    private int tick = 0;

    private NetworkVariable<TransformState> currentServerTransformState = new();
    private InputState[] inputStates = new InputState[buffer];
    private TransformState[] transformStates = new TransformState[buffer];

    public override void OnNetworkSpawn()
    {
        if (IsLocalPlayer)
        {
            inputActions = new MovementControls();
            inputActions.Player.Enable();
            currentServerTransformState.OnValueChanged += OnServerStateChanged;
            PhysicsWorld.Instance.FindPhysicsObjects();
        }
        
        physicsObject = GetComponent<PhysicsObject>();
    }

    private void Update()
    {
        if (IsLocalPlayer)
        {
            moveInput = inputActions.Player.Movement.ReadValue<Vector2>();
            ProcessLocalInput(moveInput);          
        }
        else
        {
            SimulateOtherPlayer();
        }
    }

    public void ProcessLocalInput(Vector2 _moveInput)
    {
        tickDeltaTime += Time.deltaTime;

        if (tickDeltaTime > tickRate)
        {
            int bufferIndex = tick % buffer;

            if (!IsServer)
            {
                ApplyMovement(_moveInput);
                
            }
            MovePlayerServerRpc(tick, _moveInput);

            inputStates[bufferIndex] = new()
            {
                tick = tick,
                moveInput = _moveInput,
            };

            transformStates[bufferIndex] = new()
            {
                tick = tick,
                finalPosition = transform.position,
                finalRotation = transform.rotation,
                finalVelocity = physicsObject.velocity,
            };

            tickDeltaTime -= tickRate;
            tick++;
        }
    }

    [ServerRpc]
    private void MovePlayerServerRpc(int _tick, Vector2 _moveInput)
    {
        ApplyMovement(_moveInput);

        currentServerTransformState.Value = new()
        {
            tick = _tick,
            finalPosition = transform.position,
            finalRotation = transform.rotation,
            finalVelocity = physicsObject.velocity,
        };
    }    

    public void SimulateOtherPlayer()
    {
        tickDeltaTime += Time.deltaTime;

        if (tickDeltaTime > tickRate)
        {
            if (currentServerTransformState.Value.finalPosition != null)
            {
                if (!IsServer)
                {
                    transform.position = Vector3.Lerp(transform.position, currentServerTransformState.Value.finalPosition, 0.5f);
                    transform.rotation = currentServerTransformState.Value.finalRotation;
                }
            }

            tickDeltaTime -= tickRate;;
            tick++;
        }
    }

    private void OnServerStateChanged(TransformState previousState, TransformState newState)
    {
        if (!IsLocalPlayer) return;

        if (!IsServer)
        {
            TransformState calculatedState = transformStates.First(localState => localState.tick == newState.tick);
            if (calculatedState.finalPosition != newState.finalPosition)
            {
                Debug.Log("Correcting client position");
                Debug.Log(calculatedState.finalVelocity + " : " + newState.finalVelocity);
                TeleportPlayer(newState);

                IEnumerable<InputState> inputs = inputStates.Where(input => input.tick > newState.tick);
                inputs = from input in inputs orderby input.tick select input;

                foreach (InputState inputState in inputs)
                {
                    ApplyMovement(inputState.moveInput);

                    int bufferIndex = inputState.tick % buffer;
                    transformStates[bufferIndex] = new TransformState()
                    {
                        tick = inputState.tick,
                        finalPosition = transform.position,
                        finalRotation = transform.rotation,
                        finalVelocity = physicsObject.velocity,
                    };
                }
            }
        }
    }  
    
    private void ApplyMovement(Vector2 _moveInput)
    {
        physicsObject.AddForce(new Vector3(_moveInput.x * physicsObject.mass * movementSpeed, 0, _moveInput.y * physicsObject.mass * movementSpeed));
        physicsObject.Step(tickRate);
    }

    private void TeleportPlayer(TransformState newState)
    {
        transform.position = newState.finalPosition;
        transform.rotation = newState.finalRotation;
        physicsObject.velocity = newState.finalVelocity;

        int bufferIndex = newState.tick % buffer;
        transformStates[bufferIndex] = newState;
    }
}
