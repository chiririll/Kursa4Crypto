using System.Security.Cryptography;
using Kursa4Crypto.Protocol;

namespace Kursa4Crypto.Cli.Commands;

[AutoRegister]
public class CryptoCommand(Program program) : BaseCommand(program)
{
    public override string Name => "crypto";
    public override string Description => "Test RSA encryption";
    public override string? OptionsString => "[text]";

    public override void Execute(string[] args)
    {
        var offset = 1;

        if (args.Length > offset && int.TryParse(args[offset], out var keySize))
            offset++;
        else
            keySize = EncryptionService.KeySize;

        if (args.Length > offset && bool.TryParse(args[offset], out var fOAEP))
            offset++;
        else
            fOAEP = EncryptionService.DoOAEPPadding;

        var text = string.Join(' ', args.AsSpan(offset).ToArray());

        Console.WriteLine($"Encrypting text [KeySize={keySize}, OAEP={fOAEP}]: \"{text}\"");

        var a = new CryptoEntity(new(keySize));
        var b = new CryptoEntity(new(keySize));

        var data = System.Text.Encoding.ASCII.GetBytes(text);
        Console.WriteLine($"Bytes:     {BitConverter.ToString(data)}");

        var encryptedData = Encrypt(data, b.publicKey, fOAEP);
        Console.WriteLine($"Encrypted: {BitConverter.ToString(encryptedData)}");

        var decryptedData = b.Decrypt(encryptedData, fOAEP);
        Console.WriteLine($"Decrypted: {BitConverter.ToString(decryptedData)}");

        var decryptedText = System.Text.Encoding.ASCII.GetString(decryptedData);
        Console.WriteLine($"Decrypted text:\"{decryptedText}\"");
    }

    private byte[] Encrypt(byte[] data, RSAParameters publicKey, bool fOAEP)
    {
        var csp = new RSACryptoServiceProvider();
        csp.ImportParameters(publicKey);

        var encryptedData = csp.Encrypt(data, fOAEP);
        csp.Dispose();

        return encryptedData;
    }

    private sealed class CryptoEntity
    {
        public readonly RSAParameters publicKey;
        private readonly RSAParameters privateKey;

        public CryptoEntity(RSACryptoServiceProvider csp)
        {
            publicKey = csp.ExportParameters(false);
            privateKey = csp.ExportParameters(true);
        }

        public byte[] Decrypt(byte[] data, bool fOAEP)
        {
            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(privateKey);

            var decryptedData = csp.Decrypt(data, fOAEP);
            csp.Dispose();

            return decryptedData;
        }
    }
}
