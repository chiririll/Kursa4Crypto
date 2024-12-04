namespace Kursa4Crypto.Protocol;

public class PacketParser
{
    public static bool TryParse(byte[] data, out Packet? packet)
    {
        packet = null;

        if (!CheckMagic(data))
            return false;

        var offset = Packet.Magic.Length;
        if (data.Length < offset + 1)
            return false;

        var type = (PacketType)data[offset++];

        if (data.Length < offset + 4)
            return false;

        var proverId = BitConverter.ToInt32(data, offset);
        offset += 4;

        return type switch
        {
            PacketType.Initialization => TryParseInitPacket(proverId, data, offset, out packet),
            PacketType.Challenge => TryParseChallengePacket(proverId, data, offset, out packet),
            PacketType.Response => TryParseResponsePacket(proverId, data, offset, out packet),
            PacketType.Result => TryParseResultPacket(proverId, data, offset, out packet),
            _ => false,
        };
    }

    private static bool TryParseInitPacket(int proverId, byte[] data, int offset, out Packet? packet)
    {
        packet = null;

        if (!TryParseEncryptedData(data, offset, out var encryptedData))
            return false;

        packet = new InitializationPacket(proverId, encryptedData!);
        return true;
    }

    private static bool TryParseChallengePacket(int proverId, byte[] data, int offset, out Packet? packet)
    {
        packet = null;

        if (data.Length < offset + 8)
            return false;
        var challengeNumber = BitConverter.ToInt64(data, offset);

        packet = new ChallengePacket(proverId, challengeNumber);
        return true;
    }

    private static bool TryParseResponsePacket(int proverId, byte[] data, int offset, out Packet? packet)
    {
        packet = null;

        if (data.Length < offset + 8)
            return false;
        var responseNumber = BitConverter.ToInt64(data, offset);

        packet = new ResponsePacket(proverId, responseNumber);
        return true;
    }

    private static bool TryParseResultPacket(int proverId, byte[] data, int offset, out Packet? packet)
    {
        packet = null;

        if (!TryParseEncryptedData(data, offset, out var encryptedData))
            return false;

        packet = new ResultPacket(proverId, encryptedData!);
        return true;
    }

    private static bool TryParseEncryptedData(byte[] data, int offset, out byte[]? encryptedData)
    {
        encryptedData = null;
        if (data.Length < offset + 4)
            return false;

        var length = BitConverter.ToInt32(data, offset);
        offset += 4;
        if (data.Length < offset + length)
            return false;

        encryptedData = data.AsSpan(offset, length).ToArray();
        return true;
    }

    private static bool CheckMagic(byte[] data, int offset = 0)
    {
        if (data.Length < Packet.Magic.Length + offset)
            return false;

        for (var i = 0; i < Packet.Magic.Length; i++)
        {
            if (data[offset + i] != Packet.Magic[i])
                return false;
        }

        return true;
    }
}
