using System.Numerics;

namespace Kursa4Crypto.Signals;

public class ActiveSignal : IActiveSignal
{
    public ActiveSignal(byte[] data, Vector2 source, float force)
    {
        Data = data;
        Source = source;
        InitialForce = force;

        Distance = 0f;
        Force = InitialForce;
    }

    public byte[] Data { get; private set; }
    public float InitialForce { get; private set; }
    public Vector2 Source { get; private set; }

    public float Distance { get; private set; }
    public float Force { get; private set; }

    public void Tick(float distance, float fade)
    {
        Distance += distance;
        Force -= fade;
    }

    public void UpdateListeners(float stepSize, IEnumerable<ISignalListener> listeners)
    {
        foreach (var listener in listeners)
        {
            var distance = Vector2.Distance(listener.Position, Source);

            if (MathF.Abs(Distance - distance) <= (stepSize / 2f))
                continue;

            listener.ReceiveSignal(Data, Force);
        }
    }
}

public interface IActiveSignal
{
    public byte[] Data { get; }
    public float InitialForce { get; }
    public Vector2 Source { get; }

    public float Distance { get; }
    public float Force { get; }
}
