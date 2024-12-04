using System.Numerics;
using Kursa4Crypto.Cli.Commands.Base;
using Kursa4Crypto.Protocol;

namespace Kursa4Crypto.Cli.Commands;

[AutoRegister]
public class ProverCommand(Program program) : BaseCompositeCommand(program)
{
    public override string Name => "prover";
    public override string Description => "Actions with provers";

    protected override List<ICommand> CreateChildren() => [
        new ListCommand(this),
        new AddCommand(this),
        new RemoveCommand(this),
        new ProveCommand(this),
    ];

    private class ListCommand(ProverCommand parent) : Command<ProverCommand>(parent)
    {
        public override string Name => "list";
        public override string Description => "List of all provers";

        public override void Execute(string[] args)
        {
            var state = Parent.Parent.State;
            if (state.Provers.Count < 1)
            {
                Console.WriteLine("There are no provers");
                return;
            }

            foreach (var prover in state.Provers.Values)
            {
                Console.WriteLine($"Prover {prover.Id}: "
                    + $"pos: ({prover.Position.X:0.##}, {prover.Position.Y:0.##}) "
                    + $"force: {prover.TransmitForce:0.##} "
                    + $"timeout: {prover.ProveTimeout:0.##}");
            }
        }
    }

    private class AddCommand(ProverCommand parent) : Command<ProverCommand>(parent)
    {
        public override string Name => "add";
        public override string Description => "Adds new prover";
        public override string? OptionsString => "<x> <y> <force> <timeout>";

        public override void Execute(string[] args)
        {
            if (!GetArgument<float>(args, 1, out var x))
            {
                Console.WriteLine("Invalid x argument!");
                return;
            }

            if (!GetArgument<float>(args, 2, out var y))
            {
                Console.WriteLine("Invalid y argument!");
                return;
            }

            if (!GetArgument<float>(args, 3, out var force)) force = 1f;
            if (!GetArgument<float>(args, 4, out var timeout)) timeout = 1f;

            var state = Parent.Parent.State;

            var prover = new Prover(state.SignalSpace)
            {
                Position = new Vector2(x, y),
                TransmitForce = force,
                ProveTimeout = timeout,
            };

            state.Provers.Add(prover.Id, prover);
        }
    }

    private class RemoveCommand(ProverCommand parent) : Command<ProverCommand>(parent)
    {
        public override string Name => "remove";
        public override string Description => "Removes prover";
        public override string? OptionsString => "<prover id>";

        public override void Execute(string[] args)
        {

            var state = Parent.Parent.State;
            if (!GetArgument<int>(args, 1, out var proverId) || !state.Provers.ContainsKey(proverId))
            {
                Console.WriteLine("Invalid prover id!");
                return;
            }

            state.Provers[proverId].Dispose();
            state.Provers.Remove(proverId);
        }
    }

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
