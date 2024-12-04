using Kursa4Crypto.Protocol;
using Kursa4Crypto.Signals;

namespace Kursa4Crypto.Cli;

public class SimulationState
{
    public SimulationState()
    {
        SignalSpaceSettings = new();
        SignalSpace = new(SignalSpaceSettings);

        Provers = new();
        Verifiers = new();
    }

    public SignalSpace SignalSpace { get; private set; }
    public SignalSpaceSettings SignalSpaceSettings { get; private set; }

    public Dictionary<int, Prover> Provers { get; private set; }
    public Dictionary<int, Verifier> Verifiers { get; private set; }
}
