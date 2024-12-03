using Kursa4Crypto.Signals;

namespace Kursa4Crypto.Protocol;

public class Prover(SignalSpace signalSpace) : ProtocolEntity(IdService.GetProverId(), signalSpace)
{
    private ProveProcessState? proveProcess;

    public float ProveTimeout { get; set; }

    public void Prove(int verifierId)
    {
        if (proveProcess != null)
        {
            SendMessage("Prove process already running!");
            return;
        }

        if (!KeysStorage.Instance.TryGetVerifierKey(verifierId, out var verifierKey))
        {
            SendMessage($"Cannot start prove process because verifier with id {verifierId} does not exists!");
            return;
        }

        proveProcess = new(random.NextInt64());

        var data = new InitializationPacket.Data(proveProcess.ChallengeNumber);
        var encryptedData = EncryptionService.Encrypt(data.Serialize(), verifierKey);
        var packet = new InitializationPacket(Id, encryptedData);

        Transmit(packet.Serialize());
    }

    protected override string? HandlePacket(Packet packet)
    {
        return packet.ProverId != Id
            ? "Ignoring packet with other prover id"
            : packet switch
            {
                ChallengePacket challengePacket => HandleChallengePacket(challengePacket),
                ResultPacket resultPacket => HandleResultPacket(resultPacket),
                _ => "Unsupported packet",
            };
    }

    protected override void Tick(float deltaTime)
    {
        if (proveProcess == null)
            return;

        proveProcess.ProveTime += deltaTime;

        if (proveProcess.ProveTime < ProveTimeout)
            return;

        proveProcess = null;
        SendMessage("Prove process timeout reached, aborting");
    }

    private string? HandleChallengePacket(ChallengePacket challengePacket)
    {
        if (proveProcess == null)
            return "Prove process has not being started";

        var response = new ResponsePacket(Id, challengePacket.ModifiedNumber - proveProcess.ChallengeNumber);
        Transmit(response.Serialize());

        return null;
    }

    private string? HandleResultPacket(ResultPacket resultPacket)
    {
        if (proveProcess == null)
            return "Prove process has not being started";

        if (!EncryptionService.TryDecrypt(resultPacket.EncryptedData, privateKey, out var dataBytes))
            return "Decryption failed";

        var data = ResultPacket.Data.Deserialize(dataBytes);

        var resultString = data.Success ? "Success" : "Failed";
        SendMessage($"Received result packet: {resultString}, distance = {data.Distance}");

        proveProcess = null;

        return null;
    }

    private class ProveProcessState(long challengeNumber)
    {
        public long ChallengeNumber { get; } = challengeNumber;

        public float ProveTime { get; set; }
    }
}
