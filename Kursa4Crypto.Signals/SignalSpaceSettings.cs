namespace Kursa4Crypto.Signals;

public class SignalSpaceSettings : ISignalSpaceSettings
{
    public float SignalSpeed { get; set; } = 0.05f;
    public float SignalFade { get; set; } = 0.01f;
    public float SignalFadeThreshold { get; set; } = 0f;

    public float StepDuration { get; set; } = 0.08f;
}

public interface ISignalSpaceSettings
{
    public float SignalSpeed { get; }
    public float SignalFade { get; }
    public float SignalFadeThreshold { get; }

    public float StepDuration { get; }
}
