using System.Numerics;

namespace Kursa4Crypto.Core.Signals;

public class SignalSpace
{
    private readonly List<ISignalListener> listeners = new();
    private readonly LinkedList<ActiveSignal> activeSignals = new();

    public void AddListener(ISignalListener listener) => listeners.Add(listener);
    public void RemoveListener(ISignalListener listener) => listeners.RemoveAll(l => l == null || l.Equals(listener));

    public void Transmit(byte[] data, Vector2 source, float force)
    {
        var signal = new ActiveSignal(data, source, force);
        activeSignals.AddLast(signal);
    }

    public void SimulateStep(float deltaTime)
    {
    }
}
