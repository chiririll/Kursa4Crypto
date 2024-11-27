using System.Numerics;

namespace Kursa4Crypto.Core.Signals;

public class ActiveSignal
{
    public readonly byte[] data;
    public readonly float initialForce;
    public readonly Vector2 source;

    public ActiveSignal(byte[] data, Vector2 source, float force)
    {
        this.data = data;
        this.source = source;
        this.initialForce = force;

        Distance = 0f;
        Force = initialForce;
    }

    public float Distance { get; private set; }
    public float Force { get; private set; }

    public void SimulateStep(float distance, float fade)
    {
        Distance += distance;
        Force -= fade;
    }

    public void UpdateListeners(float stepSize, IEnumerable<ISignalListener> listeners)
    {
        var sqrCurrentDistance = Distance * Distance;

        foreach (var listener in listeners)
        {
            var sqrDistance = Vector2.DistanceSquared(listener.Position, source);

            // Half step size
            if (MathF.Abs(sqrDistance - sqrCurrentDistance) > stepSize)
                continue;

            listener.ReceiveSignal(data, Force);
        }
    }
}
