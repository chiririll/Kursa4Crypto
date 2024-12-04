using Kursa4Crypto.Protocol;

namespace Kursa4Crypto.Cli.Commands;

[AutoRegister]
public class VerifierCommand(Program program) : ProtocolEntityCommand<Verifier, VerifierCommand>(program, "verifier")
{
    private const float DefaultMaxDistance = 1f;

    public override string Name => "verifier";
    public override string Description => "Actions with verifiers";

    protected override string ConfigurationOptions => $"[maxDistance={DefaultMaxDistance}]";

    protected override Dictionary<int, Verifier> Entities => Parent.State.Verifiers;

    protected override List<ICommand> CreateChildren() => [
        new ListCommand(this),
        new AddCommand(this),
        new RemoveCommand(this),
        new MoveCommand(this),
    ];

    protected override Verifier CreateEntity() => new(Parent.State.SignalSpace);

    protected override void ConfigureEntity(Verifier entity, string[] args, int offset)
    {
        if (!GetArgument<float>(args, offset + 1, out var maxDistance)) maxDistance = DefaultMaxDistance;

        entity.MaxProverDistance = maxDistance;
    }
}
