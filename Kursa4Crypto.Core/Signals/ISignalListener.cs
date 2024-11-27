using System.Numerics;

namespace Kursa4Crypto.Core.Signals;

public interface ISignalListener
{
    public Vector2 Position { get; }

    public void ReceiveSignal(byte[] signal, float force);
    public void Tick(float deltaTime);
}
