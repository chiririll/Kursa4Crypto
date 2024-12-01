using Kursa4Crypto.Signals;

namespace Kursa4Crypto.Protocol;

public class Prover(SignalSpace signalSpace)
{
    private readonly SignalSpace signalSpace = signalSpace;

    public void Prove()
    {
        // Send handshake packet
    }
}
