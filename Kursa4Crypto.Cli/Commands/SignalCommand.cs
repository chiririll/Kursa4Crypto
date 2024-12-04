using System.Text;
using System.Text.RegularExpressions;
using Kursa4Crypto.Signals;

namespace Kursa4Crypto.Cli.Commands;

[AutoRegister]
public class SignalCommand(Program program) : BaseCompositeCommand(program)
{
    public override string Name => "signal";
    public override string Description => "Actions with signals";

    protected override List<ICommand> CreateChildren() =>
    [
        new ListSubcommand(this),
        new AddSubcommand(this),
        new ShowSubcommand(this),
    ];

    private class ListSubcommand(SignalCommand signalCommand) : Command<SignalCommand>(signalCommand)
    {
        public override string Name => "list";
        public override string Description => "Display list of all active signals";

        public override void Execute(string[] args)
        {
            if (Parent.Parent.State.SignalSpace.ActiveSignalsCount < 1)
            {
                Console.WriteLine("No active signals found.");
                return;
            }

            var index = 1;
            foreach (var signal in Parent.Parent.State.SignalSpace.ActiveSignals)
            {
                Console.WriteLine($"Signal #{index++}: "
                    + $"Source = ({signal.Source.X:0.##}, {signal.Source.Y:0.##}), "
                    + $"Distance = {signal.Distance}, "
                    + $"Force = {signal.Force:0.####}/{signal.InitialForce:0.####}, "
                    + $"DataLength = {signal.Data.Length} bytes");
            }
        }
    }

    private class AddSubcommand(SignalCommand signalCommand) : Command<SignalCommand>(signalCommand)
    {
        public override string Name => "add";
        public override string Description => "Adds new signal to simulation";
        public override string? OptionsString => "<x> <y> <force>";

        public override void Execute(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine($"Usage: {Parent.Name} {Name} {OptionsString}");
                return;
            }

            if (!float.TryParse(args[1], out var x) || !float.TryParse(args[2], out var y))
            {
                Console.WriteLine("Invalid position!");
                return;
            }

            if (!float.TryParse(args[3], out var force))
            {
                Console.WriteLine("Invalid force!");
                return;
            }

            Parent.Parent.State.SignalSpace.Transmit([], new(x, y), force);
            Console.WriteLine("Signal added");
        }
    }

    private class ShowSubcommand(SignalCommand parent) : Command<SignalCommand>(parent)
    {
        public override string Name => "show";
        public override string Description => "Shows signal data";
        public override string? OptionsString => "<signal id>";

        public override void Execute(string[] args)
        {
            if (!GetArgument<int>(args, 1, out var id) && !InvalidArgument(nameof(id)))
                return;

            var signalSpace = Parent.Parent.State.SignalSpace;

            var signal = (IActiveSignal?)null;
            var signalIndex = 1;

            foreach (var signalIterator in signalSpace.ActiveSignals)
            {
                if (signalIndex++ < id) continue;

                signal = signalIterator;
                break;
            }

            if (signal == null)
            {
                Console.WriteLine("Signal not found!");
                return;
            }

            var dataString = Encoding.GetEncoding("ascii").GetString(signal.Data);
            dataString = Regex.Replace(dataString, @"\p{C}+", ".");

            const int lineLength = 16;
            for (var i = 0; i < dataString.Length; i++)
            {
                if (i % lineLength == 0)
                    Console.Write("\n");
                Console.Write(dataString[i]);
            }
        }
    }
}
