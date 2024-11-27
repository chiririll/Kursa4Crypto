using Kursa4Crypto.Core.Signals;

namespace Kursa4Crypto.Cli;

public class SimulationState
{
    public SimulationState()
    {
        SignalSpace = new();
    }

    public SignalSpace SignalSpace { get; private set; }
}
