using System.Numerics;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Security.Cryptography;
using Kursa4Crypto.Signals;

namespace Kursa4Crypto.Protocol;

public class Prover : IDisposable, ISignalListener
{
    private readonly RSACryptoServiceProvider csp = new();
    private readonly Random random = new();

    private readonly SignalSpace signalSpace;

    private readonly Subject<(int, string)> messenger = new();
    private readonly CompositeDisposable disp = new();

    private long? challengeNumber = null;
    private float proveTime;

    public int Id { get; }
    public Vector2 Position { get; set; }

    public float TransmitForce { get; set; }
    public float ProveTimeout { get; set; }

    public IObservable<(int proverId, string message)> Messenger => messenger;

    public Prover(int id, SignalSpace signalSpace)
    {
        Id = id;

        this.signalSpace = signalSpace;
        signalSpace.AddListener(this);
        disp.Add(signalSpace.OnTick.Subscribe(Tick));
    }

    private void Tick(float deltaTime)
    {
        if (!challengeNumber.HasValue)
            return;

        proveTime += deltaTime;

        if (proveTime < ProveTimeout)
            return;

        StopProveProcess();

        SendMessage("Prove process timeout reached, aborting");
    }

    public void Prove()
    {
        if (challengeNumber.HasValue)
        {
            SendMessage("Prove process already running!");
            return;
        }

        proveTime = 0f;
        challengeNumber = random.NextInt64();

        var data = new InitializationPacket.Data(challengeNumber.Value);
        var packet = new InitializationPacket(Id, csp.Encrypt(data.Serialize(), false));

        signalSpace.Transmit(packet.Serialize(), Position, TransmitForce);
    }

    public void ReceiveSignal(byte[] signal, float force)
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

        var dataBytes = csp.Decrypt(resultPacket.EncryptedData, false);
        var data = ResultPacket.Data.Deserialize(dataBytes);

        var resultString = data.Success ? "Success" : "Failed";
        SendMessage($"Received result packet: {resultString}, distance = {data.Distance}");

        StopProveProcess();

        return true;
    }

    private void StopProveProcess()
    {

        challengeNumber = null;
        proveTime = -0f;
    }

    private void SendMessage(string message) => messenger.OnNext((Id, message));

    public void Dispose()
    {
        signalSpace.RemoveListener(this);
    }
}
