using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct TransformState : INetworkSerializable
{
    public int tick;
    public Vector3 finalPosition;
    public Quaternion finalRotation;
    public Vector3 finalVelocity;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out tick);
            reader.ReadValueSafe(out finalPosition);
            reader.ReadValueSafe(out finalRotation);
            reader.ReadValueSafe(out finalVelocity);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(tick);
            writer.WriteValueSafe(finalPosition);
            writer.WriteValueSafe(finalRotation);
            writer.WriteValueSafe(finalVelocity);
        }
    }
}
