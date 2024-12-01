using System.Numerics;

namespace Kursa4Crypto.Signals;

public interface ISignalListener
{
    public Vector2 Position { get; }

    public void ReceiveSignal(byte[] signal, float force);
}
