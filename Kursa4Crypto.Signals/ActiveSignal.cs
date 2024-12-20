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

    public void Tick(float stepSize, float fade)
    {
        Distance += stepSize;
        Force -= fade;
    }

    public void UpdateListeners(float stepSize, IEnumerable<ISignalListener> listeners)
    {
        var halfStep = stepSize / 2f;
        foreach (var listener in listeners)
        {
            var distance = Vector2.Distance(Source, listener.Position);

            if (Math.Abs(distance - Distance) > halfStep)
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
