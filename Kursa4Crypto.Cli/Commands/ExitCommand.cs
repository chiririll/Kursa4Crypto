namespace Kursa4Crypto.Cli.Commands;

public class ExitCommand : ICommand
{
    public string Name => "exit";

    public void Execute(string[] args)
    {
        Console.WriteLine("\nExiting...");
        Program.Instance?.RequestExit();
    }

    public void Dispose()
    {
    }
}
