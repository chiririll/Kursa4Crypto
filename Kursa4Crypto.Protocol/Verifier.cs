using Kursa4Crypto.Signals;

namespace Kursa4Crypto.Protocol;

public class Verifier(SignalSpace signalSpace) : ProtocolEntity(IdService.GetVerifierId(), signalSpace)
{
    protected override void Tick(float deltaTime)
    {
    }

    public override void ReceiveSignal(byte[] signal, float force)
    {
    }
}
