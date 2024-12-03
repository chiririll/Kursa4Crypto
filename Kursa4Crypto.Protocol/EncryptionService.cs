using System.Security.Cryptography;

namespace Kursa4Crypto.Protocol;

public class EncryptionService
{
    public const int KeySize = 256;
    public const bool DoOAEPPadding = false;

    public static byte[] Encrypt(byte[] data, RSAParameters publicKey)
    {
        var rsa = new RSACryptoServiceProvider(KeySize);
        rsa.ImportParameters(publicKey);

        var encryptedData = rsa.Encrypt(data, DoOAEPPadding);

        rsa.Dispose();
        return encryptedData;
    }

    public static byte[] Decrypt(byte[] data, RSAParameters privateKey)
    {
        var rsa = new RSACryptoServiceProvider(KeySize);
        rsa.ImportParameters(privateKey);

        var encryptedData = rsa.Decrypt(data, DoOAEPPadding);

        rsa.Dispose();
        return encryptedData;
    }
}
