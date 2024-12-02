namespace Kursa4Crypto.Protocol;

public enum PacketType : byte
{
    None = 0,
    Initialization = 1,
    Challenge = 2,
    Response = 3,
    Result = 4,
}

public abstract class Packet(PacketType type, int proverId)
{
    public char[] Magic => ['K', 'U', 'R', 'S', 'A', '4'];
    public PacketType Type { get; } = type;
    public int ProverId { get; } = proverId;

    protected virtual List<byte> GetBytes()
    {
        var bytes = new List<byte>();

        bytes.AddRange(Magic.Select(c => (byte)c));
        bytes.Add((byte)Type);

        bytes.AddRange(BitConverter.GetBytes(ProverId));

        return bytes;
    }

    public byte[] Serialize()
    {
        var bytes = GetBytes();
        return bytes.ToArray();
    }
}

public abstract class EncryptedPacket(PacketType type, int proverId, byte[] encryptedData) : Packet(type, proverId)
{
    public byte[] EncryptedData { get; } = encryptedData;

    protected override List<byte> GetBytes()
    {
        var bytes = base.GetBytes();

        bytes.AddRange(BitConverter.GetBytes(EncryptedData.Length));
        bytes.AddRange(EncryptedData);

        return bytes;
    }
}

public class InitializationPacket(int proverId, byte[] encryptedData)
    : EncryptedPacket(PacketType.Initialization, proverId, encryptedData)
{
    public class Data
    {
        public long RandomNumber { get; }
    }
}

public class ChallengePacket(int proverId, long modifiedNumber) : Packet(PacketType.Challenge, proverId)
{
    public long ModifiedNumber { get; } = modifiedNumber;

    protected override List<byte> GetBytes()
    {
        var bytes = base.GetBytes();

        bytes.AddRange(BitConverter.GetBytes(ModifiedNumber));

        return bytes;
    }
}

public class ResponsePacket(int proverId, long numberDelta) : Packet(PacketType.Response, proverId)
{
    public long NumberDelta { get; } = numberDelta;

    protected override List<byte> GetBytes()
    {
        var bytes = base.GetBytes();

        bytes.AddRange(BitConverter.GetBytes(NumberDelta));

        return bytes;
    }
}

public class ResultPacket(int proverId, byte[] encryptedData)
    : EncryptedPacket(PacketType.Result, proverId, encryptedData)
{
    public class Data(bool success, float distance)
    {
        public bool Success { get; } = success;
        public float Distance { get; } = distance;
    }
}
