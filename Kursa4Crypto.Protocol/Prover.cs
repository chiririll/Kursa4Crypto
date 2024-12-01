using System.Reactive.Disposables;
using Kursa4Crypto.Signals;

namespace Kursa4Crypto.Protocol;

public class Prover : IDisposable
{
    private readonly SignalSpace signalSpace;

    private readonly CompositeDisposable disp = new();

    public Prover(SignalSpace signalSpace)
    {
        this.signalSpace = signalSpace;

        disp.Add(signalSpace.onTick.Subscribe(Tick));
    }

    public void Tick(float deltaTime)
    {
    }

    public void Prove()
    {
        // Send handshake packet
    }

    public void Dispose()
    {
        disp.Dispose();
    }
}
