namespace Kursa4Crypto.Signals;

public class SignalSpaceSettings : ISignalSpaceSettings
{
    public float SignalSpeed { get; set; } = 1000f;
    public float SignalFade { get; set; } = 0.1f;
    public float SignalFadeThreshold { get; set; } = 0f;

    public float StepDuration { get; set; } = 0.01f;

    public SignalSpaceSettings()
    {
    }

    public void CopyFrom(SignalSpaceSettings other)
    {
        SignalSpeed = other.SignalSpeed;
        SignalFade = other.SignalFade;
        SignalFadeThreshold = other.SignalFadeThreshold;

        StepDuration = other.StepDuration;
    }
}

public interface ISignalSpaceSettings
{
    public float SignalSpeed { get; }
    public float SignalFade { get; }
    public float SignalFadeThreshold { get; }

    public float StepDuration { get; }
}
