using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HandleStates
{
    public struct InputState
    {
        public Vector2 moveInput;
    }

    public struct TransformStateRW : INetworkSerializable
    {
        public Vector3 finalPosition;
        public Quaternion finalRotation;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out finalPosition);
                reader.ReadValueSafe(out finalRotation);;
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(finalPosition);
                writer.WriteValueSafe(finalRotation);
            }
        }
    }
}
