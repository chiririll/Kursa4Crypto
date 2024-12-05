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

            return bytes.ToArray();
        }
    }
}

public class InitializationPacket(int proverId, byte[] encryptedData)
    : EncryptedPacket(PacketType.Initialization, proverId, encryptedData)
{
    public class Data(long challengeNumber, long responseNumber) : BaseData
    {
        public long ChallengeNumber { get; } = challengeNumber;
        public long ResponseNumber { get; } = responseNumber;

        protected override List<byte> GetBytes()
        {
            var bytes = base.GetBytes();

            bytes.AddRange(BitConverter.GetBytes(ChallengeNumber));
            bytes.AddRange(BitConverter.GetBytes(ResponseNumber));

            return bytes;
        }

        public static Data Deserialize(byte[] dataBytes)
        {
            var challengeNumber = BitConverter.ToInt64(dataBytes, 0);
            var responseNumber = BitConverter.ToInt64(dataBytes, 8);

            return new(challengeNumber, responseNumber);
        }
    }
}

public class ChallengePacket(int proverId, long challengeNumber) : Packet(PacketType.Challenge, proverId)
{
    public long ChallengeNumber { get; } = challengeNumber;

    protected override List<byte> GetBytes()
    {
        var bytes = base.GetBytes();

        bytes.AddRange(BitConverter.GetBytes(ChallengeNumber));

        return bytes;
    }
}

public class ResponsePacket(int proverId, long responseNumber) : Packet(PacketType.Response, proverId)
{
    public long ResponseNumber { get; } = responseNumber;

    protected override List<byte> GetBytes()
    {
        var bytes = base.GetBytes();

        bytes.AddRange(BitConverter.GetBytes(ResponseNumber));

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
