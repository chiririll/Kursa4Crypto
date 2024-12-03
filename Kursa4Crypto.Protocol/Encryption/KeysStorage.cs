
using System.Security.Cryptography;
namespace Kursa4Crypto.Protocol;

public class KeysStorage
{
    private readonly Dictionary<int, RSAParameters> proverKeys = new();
    private readonly Dictionary<int, RSAParameters> verifierKeys = new();

    public static KeysStorage Instance { get; }

    static KeysStorage()
    {
        Instance = new();
    }

    private KeysStorage()
    {
    }

    public RSAParameters? GetProverKey(int proverId) => GetKey(proverId, proverKeys);
    public RSAParameters? GetVerifierKey(int verifierId) => GetKey(verifierId, verifierKeys);

    private static RSAParameters? GetKey(int id, Dictionary<int, RSAParameters> keys) => keys.TryGetValue(id, out var key) ? key : null;

    public void RegisterEntity(ProtocolEntity entity)
    {
        var keys = entity switch
        {
            Prover => proverKeys,
            Verifier => verifierKeys,
            _ => throw new NotImplementedException($"Cannot register key for {entity.GetType().Name}!"),
        };

        if (keys.ContainsKey(entity.Id))
            throw new InvalidOperationException($"Cannot register {entity.GetType().Name} with id {entity.Id} because it already registered!");

        keys.Add(entity.Id, entity.PublicKey);
    }
}
