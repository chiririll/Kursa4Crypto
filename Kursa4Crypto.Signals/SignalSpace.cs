using System.Numerics;
using System.Reactive.Subjects;

namespace Kursa4Crypto.Signals;

public class SignalSpace
{
    private readonly SignalSpaceSettings settings;

    private readonly List<ISignalListener> listeners = new();
    private readonly LinkedList<ActiveSignal> activeSignals = new();

    private readonly Subject<float> onTick = new();
    private readonly Subject<IActiveSignal> onSignalCreated = new();
    private readonly Subject<IActiveSignal> onSignalFadeOut = new();

    public SignalSpace(SignalSpaceSettings settings)
    {
        this.settings = settings;
    }

    public ISignalSpaceSettings Settings => settings;

    public int ActiveSignalsCount => activeSignals.Count;
    public IEnumerable<IActiveSignal> ActiveSignals => activeSignals;

    public IObservable<float> OnTick => onTick;
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
        onTick.OnNext(settings.StepDuration);

        var signals = new List<ActiveSignal>(activeSignals);
        foreach (var signal in signals)
        {
            signal.Tick(settings.SignalSpeed, settings.SignalFade);

            if (signal.Force <= settings.SignalFadeThreshold)
            {
                onSignalFadeOut.OnNext(signal);
                activeSignals.Remove(signal);
                continue;
            }

            signal.UpdateListeners(settings.SignalSpeed, listeners);
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
