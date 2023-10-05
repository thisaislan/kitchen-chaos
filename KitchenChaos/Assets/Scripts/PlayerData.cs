using System;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public int colorId;

    public bool Equals(PlayerData other) =>
        this.clientId.Equals(other.clientId) &&
        this.colorId.Equals(other.colorId);

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
    }
}
