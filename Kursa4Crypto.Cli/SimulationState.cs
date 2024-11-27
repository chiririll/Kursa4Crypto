using Kursa4Crypto.Core.Signals;

namespace Kursa4Crypto.Cli;

public class SimulationState
{
    public SimulationState()
    {
        SignalSpaceSettings = new();
        SignalSpace = new(SignalSpaceSettings);
    }

    public SignalSpace SignalSpace { get; private set; }
    public SignalSpaceSettings SignalSpaceSettings { get; private set; }
}
