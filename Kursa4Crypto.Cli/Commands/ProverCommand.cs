using Kursa4Crypto.Protocol;

namespace Kursa4Crypto.Cli.Commands;

[AutoRegister]
public class ProverCommand(Program program) : ProtocolEntityCommand<Prover, ProverCommand>(program, "prover")
{
    public override string Name => "prover";
    public override string Description => "Actions with provers";

    protected override Dictionary<int, Prover> Entities => Parent.State.Provers;

    protected override Prover CreateEntity() => new(Parent.State.SignalSpace);

    protected override List<ICommand> CreateChildren() => [
        new ListCommand(this),
        new AddCommand(this),
        new RemoveCommand(this),
        new MoveCommand(this),
        new ProveCommand(this),
    ];

    private class ProveCommand(ProverCommand parent) : Command<ProverCommand>(parent)
    {
        public override string Name => "prove";
        public override string Description => "Starts prove process";
        public override string? OptionsString => "<prover id> <verifier id>";

        public override void Execute(string[] args)
        {
            var state = Parent.Parent.State;
            if (!GetArgument<int>(args, 1, out var proverId) || !state.Provers.TryGetValue(proverId, out var prover))
            {
                Console.WriteLine("Invalid prover id!");
                return;
            }
            if (!GetArgument<int>(args, 2, out var verifierId) || !state.Verifiers.ContainsKey(verifierId))
            {
                Console.WriteLine("Invalid verifier id!");
                return;
            }

            prover.Prove(verifierId);
            Console.WriteLine("Prove process started");
        }
    }
}
