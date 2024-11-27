using System.Reflection;
using Kursa4Crypto.Cli.Commands;

namespace Kursa4Crypto.Cli;

public class Program
{
    private readonly List<ICommand> commands;

    private bool exitRequested = false;

    public Program()
    {
        var baseCommandType = typeof(ICommand);
        commands = Assembly.GetAssembly(baseCommandType)?.GetTypes()
            .Where(t => !t.IsAbstract && baseCommandType.IsAssignableFrom(t))
            .Select(t => Activator.CreateInstance(t))
            .Cast<ICommand>()
            .ToList() ?? new();
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
        PrintMotd();

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
            command.Dispose();
        }
    }

    private void PrintMotd()
    {
        Console.WriteLine("Distance bounding protocol program.");
        Console.WriteLine("Print help for command list.");
    }

    private void PrintUnknownCommandText(string command)
    {
        Console.WriteLine($"Unknown command '{command}'");
    }

    private void ReadPrompt()
    {
        Console.Write("\n> ");

        var prompt = Console.ReadLine();
        var commandName = string.Empty;

        if (!string.IsNullOrEmpty(prompt))
        {
            var args = prompt.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            commandName = args[0].ToLower();

            foreach (var command in commands)
            {
                if (!command.Name.Equals(commandName))
                    continue;

                command.Execute(args);
                return;
            }
        }

        PrintUnknownCommandText(commandName);
    }
}