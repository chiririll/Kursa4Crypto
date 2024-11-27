namespace Kursa4Crypto.Cli.Commands;

public class HelpCommand(IEnumerable<ICommand> commands) : ICommand
{
    public const string ConstName = "help";
    private readonly IEnumerable<ICommand> commands = commands;

    public string Name => ConstName;
    public string Description => "Displays list of all available commands";
    public string? OptionsString => null;

    public void Execute(string[] args)
    {
        Console.WriteLine("Here list of all available commands:");

        foreach (var command in commands)
        {
            Console.Write($"{command.Name} ");
            if (!string.IsNullOrEmpty(command.OptionsString))
            {
                Console.Write($"{command.OptionsString} ");
            }
            Console.WriteLine($"- {command.Description};");
        }
    }
}
