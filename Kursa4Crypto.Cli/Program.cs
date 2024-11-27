using System.Reflection;
using Kursa4Crypto.Cli.Commands;

namespace Kursa4Crypto.Cli;

public class Program
{
    private readonly HelpCommand helpCommand;
    private readonly List<ICommand> commands;

    private bool exitRequested = false;

    public Program()
    {
        State = new();

        var baseCommandType = typeof(ICommand);
        commands = Assembly.GetAssembly(baseCommandType)?.GetTypes()
            .Where(t => !t.IsAbstract && baseCommandType.IsAssignableFrom(t))
            .Where(t => t.GetCustomAttribute<AutoRegisterAttribute>() != null)
            .Select(t => Activator.CreateInstance(t, this))
            .Cast<ICommand>()
            .ToList() ?? new();

        commands.Add(new ExitCommand(this));
        helpCommand = new(commands);
    }

    public SimulationState State { get; }

    public static void Main()
    {
        var instance = new Program();
        instance.Run();
    }

    public void Run()
    {
        Console.WriteLine("Distance bounding protocol program.");
        Console.WriteLine($"Type '{helpCommand.Name}' for command list.");

        while (!exitRequested)
        {
            ReadPrompt();
        }

        Exit();
    }

    public void RequestExit() => exitRequested = true;

    private void Exit()
    {
        foreach (var command in commands)
        {
            if (command is IDisposable disposable)
                disposable.Dispose();
        }
    }

    private void ReadPrompt()
    {
        Console.Write("\n> ");

        var prompt = Console.ReadLine();

        if (string.IsNullOrEmpty(prompt))
            return;

        var args = prompt.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var commandName = args[0].ToLower();

        var command = commandName switch
        {
            HelpCommand.ConstName => helpCommand,
            _ => commands.Find(c => c.Name.Equals(commandName)),
        };

        if (command == null)
        {
            Console.WriteLine($"Unknown command '{commandName}'. Type '{helpCommand.Name}' to print list of available commands");
        }
        else
        {
            command.Execute(args);
        }
    }

    private class ExitCommand(Program program) : ICommand
    {
        private readonly Program program = program;

        public string Name => "exit";
        public string Description => "Exit program";
        public string? OptionsString => null;

        public void Execute(string[] args)
        {
            Console.WriteLine("Exiting...");
            program.RequestExit();
        }
    }
}