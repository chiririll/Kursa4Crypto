using Kursa4Crypto.Cli.Commands.Base;

namespace Kursa4Crypto.Cli.Commands;

[AutoRegister]
public class SignalCommand : BaseCommand, ICommand
{
    private readonly HelpCommand helpCommand;
    private readonly List<ICommand> subcommands;

    public SignalCommand(Program program) : base(program)
    {
        subcommands = new()
        {
            new ListSubcommand(this),
            new AddSubcommand(this),
        };

        helpCommand = new(subcommands);
    }

    public override string Name => "signal";
    public override string Description => "Actions with signals";
    public override string? OptionsString => "<subcommand>";

    public override void Execute(string[] args)
    {
        var helpText = $"Type '{Name} {helpCommand.Name}' to print list of available subcommands";

        if (args.Length < 2)
        {
            Console.WriteLine(helpText);
            return;
        }

        var subcommandName = args[1].ToLower();
        var subcommand = subcommandName switch
        {
            HelpCommand.ConstName => helpCommand,
            _ => subcommands.Find(c => c.Name.Equals(subcommandName)),
        };

        if (subcommand == null)
        {
            Console.WriteLine($"Unknown command '{subcommandName}'. " + helpText);
        }
        else
        {
            subcommand.Execute(args);
        }
    }

    private class ListSubcommand(SignalCommand signalCommand) : ICommand
    {
        private readonly SignalCommand signalCommand = signalCommand;

        public string Name => "list";
        public string Description => "Display list of all active signals";
        public string? OptionsString => null;

        public void Execute(string[] args)
        {
            if (signalCommand.Program.State.SignalSpace.ActiveSignalsCount < 1)
            {
                Console.WriteLine("No active signals found.");
                return;
            }

            var index = 1;
            foreach (var signal in signalCommand.Program.State.SignalSpace.ActiveSignals)
            {
                Console.WriteLine($"Signal #{index++}: "
                    + $"Source = ({signal.Source.X:0.##}, {signal.Source.Y:0.##}), "
                    + $"Distance = {signal.Distance}, "
                    + $"Force = {signal.Force:0.####}/{signal.InitialForce:0.####}, "
                    + $"DataLength = {signal.Data.Length} bytes");
            }
        }
    }

    private class AddSubcommand(SignalCommand signalCommand) : ICommand
    {
        private readonly SignalCommand signalCommand = signalCommand;

        public string Name => "add";
        public string Description => "Adds new signal to simulation";
        public string? OptionsString => "<x> <y> <force>";

        public void Execute(string[] args)
        {
            if (args.Length < 5)
            {
                Console.WriteLine($"Usage: {signalCommand.Name} {Name} {OptionsString}");
                return;
            }

            if (!float.TryParse(args[2], out var x) || !float.TryParse(args[3], out var y))
            {
                Console.WriteLine("Invalid position!");
                return;
            }

            if (!float.TryParse(args[4], out var force))
            {
                Console.WriteLine("Invalid force!");
                return;
            }

            signalCommand.Program.State.SignalSpace.Transmit([], new(x, y), force);
        }
    }
}
