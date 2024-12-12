namespace Kursa4Crypto.Cli.Commands;

[AutoRegister]
public class ClearCommand(Program program) : BaseCommand(program)
{
    public override string Name => "clear";
    public override string Description => "Clears console";

    public override void Execute(string[] args)
    {
        Console.Clear();
    }
}
