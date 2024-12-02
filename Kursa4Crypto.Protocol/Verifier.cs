using System.Numerics;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using Kursa4Crypto.Signals;

namespace Kursa4Crypto.Protocol;

public class Verifier : ISignalListener, IDisposable
{
    private readonly SignalSpace signalSpace;

    private readonly Dictionary<int, byte[]> keys = new();
    private readonly Subject<(int, string)> messenger = new();
    private readonly CompositeDisposable disp = new();

    public IObservable<(int proverId, string message)> Messenger => messenger;

    public int Id { get; }
    public Vector2 Position { get; set; }

    public Verifier(int id, SignalSpace signalSpace)
    {
        Id = id;

        this.signalSpace = signalSpace;
        signalSpace.AddListener(this);
        disp.Add(signalSpace.OnTick.Subscribe(Tick));
    }

    private void Tick(float deltaTime)
    {
    }

    public void ReceiveSignal(byte[] signal, float force)
    {
    }

    public void RegisterProver(Prover prover)
    {
        if (keys.ContainsKey(prover.Id))
        {
            SendMessage($"Cannot register prover with id: {prover.Id} because it already registered!");
            return;
        }
    }

    private void SendMessage(string message) => messenger.OnNext((Id, message));

    public void Dispose()
    {
        signalSpace.RemoveListener(this);
        disp.Dispose();
    }
}
