using System.Numerics;
using System.Reactive.Subjects;

namespace Kursa4Crypto.Signals;

public class SignalSpace
{
    private readonly SignalSpaceSettings settings;

    private readonly List<ISignalListener> listeners = new();
    private readonly LinkedList<ActiveSignal> activeSignals = new();

    private readonly Subject<IActiveSignal> onSignalCreated = new();
    private readonly Subject<IActiveSignal> onSignalFadeOut = new();

    public SignalSpace(SignalSpaceSettings settings)
    {
        this.settings = settings;
    }

    public ISignalSpaceSettings Settings => settings;

    public int ActiveSignalsCount => activeSignals.Count;
    public IEnumerable<IActiveSignal> ActiveSignals => activeSignals;

    public IObservable<IActiveSignal> OnSignalCreated => onSignalCreated;
    public IObservable<IActiveSignal> OnSignalFadeOut => onSignalFadeOut;

    public float Time { get; private set; }

    public void AddListener(ISignalListener listener) => listeners.Add(listener);
    public void RemoveListener(ISignalListener listener) => listeners.RemoveAll(l => l == null || l.Equals(listener));

    public void Transmit(byte[] data, Vector2 source, float force)
    {
        var signal = new ActiveSignal(data, source, force);
        activeSignals.AddLast(signal);
        onSignalCreated.OnNext(signal);
    }

    public void Tick()
    {
        foreach (var listener in listeners)
        {
            listener.Tick(settings.StepDuration);
        }

        var fadedSignals = new List<ActiveSignal>();
        foreach (var signal in activeSignals)
        {
            signal.Tick(settings.SignalSpeed, settings.SignalFade);

            if (signal.Force <= settings.SignalFadeThreshold)
            {
                fadedSignals.Add(signal);
                continue;
            }

            signal.UpdateListeners(settings.SignalSpeed, listeners);
        }

        foreach (var signal in fadedSignals)
        {
            onSignalFadeOut.OnNext(signal);
            activeSignals.Remove(signal);
        }
    }

    public void Reset()
    {
        foreach (var signal in activeSignals)
        {
            onSignalFadeOut.OnNext(signal);
        }

        activeSignals.Clear();
    }
}
