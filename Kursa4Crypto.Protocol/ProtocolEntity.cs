using System.Numerics;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Security.Cryptography;
using Kursa4Crypto.Signals;

namespace Kursa4Crypto.Protocol;

public abstract class ProtocolEntity : ISignalListener, IDisposable
{
    protected readonly SignalSpace signalSpace;

    private readonly Subject<(int, string)> messenger = new();
    protected readonly CompositeDisposable disp = new();

    protected readonly Random random = new();
    protected readonly RSAParameters publicKey;
    protected readonly RSAParameters privateKey;

    public ProtocolEntity(int id, SignalSpace signalSpace)
    {
        Id = id;

        this.signalSpace = signalSpace;

        var rsa = new RSACryptoServiceProvider(EncryptionService.KeySize);
        publicKey = rsa.ExportParameters(false);
        privateKey = rsa.ExportParameters(true);
        rsa.Dispose();

        signalSpace.AddListener(this);
        signalSpace.OnTick.Subscribe(Tick).AddTo(disp);

        KeysStorage.Instance.RegisterEntity(this);
    }

    public int Id { get; }
    public RSAParameters PublicKey => publicKey;

    public Vector2 Position { get; set; }
    public float TransmitForce { get; set; }

    public IObservable<(int proverId, string message)> Messenger => messenger;

    protected abstract void Tick(float deltaTime);

    public virtual void ReceiveSignal(byte[] signal, float force)
    {
        var packet = PacketParser.Parse(signal);
        if (packet == null)
        {
            SendMessage("Received invalid packet!");
            return;
        }

        var message = HandlePacket(packet);
        if (!string.IsNullOrEmpty(message))
        {
            SendMessage($"Cannot handle packet of type {packet.GetType()} from prover {packet.ProverId}: {message}");
        }
    }

    protected abstract string? HandlePacket(Packet packet);

    protected void Transmit(byte[] data) => signalSpace.Transmit(data, Position, TransmitForce);
    protected void SendMessage(string message) => messenger.OnNext((Id, message));

    public virtual void Dispose()
    {
        signalSpace.RemoveListener(this);
        disp.Dispose();
    }
}
