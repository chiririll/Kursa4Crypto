using Kursa4Crypto.Cli.Commands.Base;

namespace Kursa4Crypto.Cli.Commands;

[AutoRegister]
public class SimulateCommand(Program program) : BaseCommand(program), ICommand
{
    public override string Name => "simulate";
    public override string Description => "Run simulation for 1 or n steps, or while all signals fade out";
    public override string? OptionsString => "[{<steps>, signals}]";

    public override void Execute(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Running 1 tick of simulation");
            Program.State.SignalSpace.Tick();
            return;
        }

        if (int.TryParse(args[1], out var stepsCount))
        {
            stepsCount = Math.Max(0, stepsCount);
            Console.WriteLine($"Running {stepsCount} ticks of simulation");

            for (var i = 0; i < stepsCount; i++)
            {
                Program.State.SignalSpace.Tick();
            }

            return;
        }

        switch (args[1].ToLower())
        {
            case "help":
                Console.WriteLine($"Use '{Name} <n>' to simulate n steps");
                Console.WriteLine($"Or you can use just '{Name}' to simulate 1 step");
                Console.WriteLine($"Also you can use '{Name} signals' to run simulation until all signals fade out");
                return;

            case "signals":
                while (Program.State.SignalSpace.ActiveSignalsCount > 0)
                {
                    Program.State.SignalSpace.Tick();
                }
                return;

            default:
                Console.WriteLine($"Unknown option '{args[1]}', use '{Name} help' to obtain more info");
                return;
        }
    }
}
