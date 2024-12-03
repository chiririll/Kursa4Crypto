using Kursa4Crypto.Signals;

namespace Kursa4Crypto.Protocol;

public class Prover(SignalSpace signalSpace) : ProtocolEntity(IdService.GetProverId(), signalSpace)
{
    private float proveTime;
    private long? challengeNumber = null;

    public float ProveTimeout { get; set; }

    public void Prove(int verifierId)
    {
        if (challengeNumber.HasValue)
        {
            SendMessage("Prove process already running!");
            return;
        }

        var verifierKey = KeysStorage.Instance.GetVerifierKey(verifierId);
        if (!verifierKey.HasValue)
        {
            SendMessage($"Cannot start prove process because verifier with id {verifierId} does not exists!");
            return;
        }

        proveTime = 0f;
        challengeNumber = random.NextInt64();

        var data = new InitializationPacket.Data(challengeNumber.Value);
        var encryptedData = EncryptionService.Encrypt(data.Serialize(), verifierKey.Value);
        var packet = new InitializationPacket(Id, encryptedData);

        signalSpace.Transmit(packet.Serialize(), Position, TransmitForce);
    }

    public override void ReceiveSignal(byte[] signal, float force)
    {
        if (!challengeNumber.HasValue)
        {
            SendMessage("Ignoring signal because prove process has not been initialized");
            return;
        }

        var packet = PacketParser.Parse(signal);
        if (packet == null)
        {
            SendMessage("Received invalid packet!");
            return;
        }

        if (packet.ProverId != Id)
        {
            SendMessage("Ignoring packet with other id");
            return;
        }

        var result = packet switch
        {
            ChallengePacket challengePacket => HandleChallengePacket(challengePacket),
            ResultPacket resultPacket => HandleResultPacket(resultPacket),
            _ => false,
        };

        if (!result)
        {
            SendMessage($"Cannot handle packet of type {packet.GetType()}");
        }
    }

    protected override void Tick(float deltaTime)
    {
        if (!challengeNumber.HasValue)
            return;

        proveTime += deltaTime;

        if (proveTime < ProveTimeout)
            return;

        StopProveProcess();

        SendMessage("Prove process timeout reached, aborting");
    }

    private bool HandleChallengePacket(ChallengePacket challengePacket)
    {
        if (!challengeNumber.HasValue)
            return false;

        var response = new ResponsePacket(Id, challengePacket.ModifiedNumber - challengeNumber.Value);
        signalSpace.Transmit(response.Serialize(), Position, TransmitForce);

        return true;
    }

    private bool HandleResultPacket(ResultPacket resultPacket)
    {
        if (!challengeNumber.HasValue)
            return false;

        var dataBytes = EncryptionService.Decrypt(resultPacket.EncryptedData, privateKey);
        var data = ResultPacket.Data.Deserialize(dataBytes);

        var resultString = data.Success ? "Success" : "Failed";
        SendMessage($"Received result packet: {resultString}, distance = {data.Distance}");

        StopProveProcess();

        return true;
    }

    private void StopProveProcess()
    {
        challengeNumber = null;
        proveTime = 0f;
    }
}
