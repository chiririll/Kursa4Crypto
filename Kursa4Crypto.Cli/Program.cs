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
        var baseCommandType = typeof(ICommand);

        commands = Assembly.GetAssembly(baseCommandType)?.GetTypes()
            .Where(t => !t.IsAbstract && baseCommandType.IsAssignableFrom(t))
            .Where(t => t.GetCustomAttribute<AutoRegisterAttribute>() != null)
            .Select(t => Activator.CreateInstance(t))
            .Cast<ICommand>()
            .ToList() ?? new();

        commands.Add(new ExitCommand());
        helpCommand = new();
    }

    public static Program? Instance { get; private set; }

    public static void Main()
    {
        if (Instance != null)
            return;

        Instance = new Program();
        Instance.Run();

        Instance = null;
    }

    public void Run()
    {
        Console.WriteLine("Distance bounding protocol program.");
        Console.WriteLine($"Type '{HelpCommand.Name}' for command list.");

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

        IExecutable? command = commandName switch
        {
            HelpCommand.Name => helpCommand,
            _ => commands.Find(c => c.Name.Equals(commandName)),
        };

        if (command == null)
        {
            Console.WriteLine($"Unknown command '{commandName}'. Type '{HelpCommand.Name}' to print list of available commands");
        }
        else
        {
            command.Execute(args);
        }
    }

    private class HelpCommand : IExecutable
    {
        public const string Name = "help";

        public void Execute(string[] args)
        {
            Console.WriteLine("Here list of all available commands:");

            foreach (var command in Instance!.commands)
            {
                Console.WriteLine($"{command.Name} - {command.Description};");
            }
        }
    }

    private class ExitCommand : ICommand
    {
        public string Name => "exit";
        public string Description => "Exit program";

        public void Execute(string[] args)
        {
            Console.WriteLine("Exiting...");
            Instance!.RequestExit();
        }
    }
}