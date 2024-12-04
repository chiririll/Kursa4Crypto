using Kursa4Crypto.Signals;

namespace Kursa4Crypto.Protocol;

public class Verifier(SignalSpace signalSpace) : ProtocolEntity(IdService.GetVerifierId(), signalSpace)
{
    private readonly Dictionary<int, ProveProcessState> processes = new();

    public float MaxProverDistance { get; set; }

    protected override void Tick(float deltaTime)
    {
        var proversToRemove = new List<int>();

        foreach (var (id, process) in processes)
        {
            process.ProveTime += deltaTime;

            if (process.ProveTime > ProveTimeout)
            {
                SendMessage($"Prove process for prover {id} has reached timeout");
                proversToRemove.Add(id);
            }
        }

        proversToRemove.Select(id => processes.Remove(id));
    }

    protected override string? HandlePacket(Packet packet) => packet switch
    {
        InitializationPacket initializationPacket => HandleInitializationPacket(initializationPacket),
        ResponsePacket responsePacket => HandleResponsePacket(responsePacket),
        _ => "Unsupported packet",
    };

    private string? HandleInitializationPacket(InitializationPacket packet)
    {
        if (processes.ContainsKey(packet.ProverId))
            return $"Prove process is already running";

        if (!EncryptionService.TryDecrypt(packet.EncryptedData, privateKey, out var dataBytes))
            return "Failed to decrypt packet";

        var data = InitializationPacket.Data.Deserialize(dataBytes);

        var processState = new ProveProcessState(packet.ProverId, data.ChallengeNumber, data.ResponseNumber);
        processes[packet.ProverId] = processState;

        var challengePacket = new ChallengePacket(packet.ProverId, processState.ChallengeNumber);
        Transmit(challengePacket.Serialize());

        return null;
    }

    private string? HandleResponsePacket(ResponsePacket packet)
    {
        if (!processes.TryGetValue(packet.ProverId, out var processState))
            return $"Prove process is not running";

        if (packet.ResponseNumber != processState.ResponseNumber)
            return $"Invalid response number";

        if (!KeysStorage.Instance.TryGetProverKey(packet.ProverId, out var proverKey))
            return "Prover key not found";

        var distance = signalSpace.Settings.SignalSpeed * processState.ProveTime;
        var success = distance <= MaxProverDistance;

        var data = new ResultPacket.Data(success, distance);
        var encryptedData = EncryptionService.Encrypt(data.Serialize(), proverKey);

        var resultPacket = new ResultPacket(packet.ProverId, encryptedData);
        Transmit(resultPacket.Serialize());

        return null;
    }

    private class ProveProcessState(int proverId, long challengeNumber, long responseNumber)
    {
        public int ProverId { get; } = proverId;
        public long ChallengeNumber { get; } = challengeNumber;
        public long ResponseNumber { get; } = responseNumber;

        public float ProveTime { get; set; } = 0f;
    }
}
