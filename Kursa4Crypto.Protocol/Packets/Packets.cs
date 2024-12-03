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
    public static char[] Magic => ['K', 'U', 'R', 'S', 'A', '4'];

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

        // TODO: Add additional data (packet length, ...)

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

    public abstract class BaseData
    {
        protected virtual List<byte> GetBytes() => [];

        public byte[] Serialize()
        {
            var bytes = GetBytes();
            var length = BitConverter.GetBytes(bytes.Count);

            return length.Concat(bytes).ToArray();
        }
    }
}

public class InitializationPacket(int proverId, byte[] encryptedData)
    : EncryptedPacket(PacketType.Initialization, proverId, encryptedData)
{
    public class Data(long randomNumber) : BaseData
    {
        public long RandomNumber { get; } = randomNumber;

        protected override List<byte> GetBytes()
        {
            var bytes = base.GetBytes();

            bytes.AddRange(BitConverter.GetBytes(RandomNumber));

            return bytes;
        }

        public static Data Deserialize(byte[] dataBytes)
        {
            var randomNumber = BitConverter.ToInt64(dataBytes, 0);

            return new(randomNumber);
        }
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
    public class Data(bool success, float distance) : BaseData
    {
        public bool Success { get; } = success;
        public float Distance { get; } = distance;

        protected override List<byte> GetBytes()
        {
            var bytes = base.GetBytes();

            bytes.AddRange(BitConverter.GetBytes(Success));
            bytes.AddRange(BitConverter.GetBytes(Distance));

            return bytes;
        }

        public static Data Deserialize(byte[] dataBytes)
        {
            var success = BitConverter.ToBoolean(dataBytes, 0);
            var distance = BitConverter.ToSingle(dataBytes, 1);

            return new(success, distance);
        }
    }
}