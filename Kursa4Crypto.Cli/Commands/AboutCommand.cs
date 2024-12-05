namespace Kursa4Crypto.Cli.Commands;

[AutoRegister]
public class AboutCommand(Program program) : BaseCommand(program)
{
    public override string Name => "about";
    public override string Description => "Information about program";

    public override void Execute(string[] args)
    {
        Console.WriteLine("Used software:");
        Console.WriteLine("  Dotnet v8: https://dotnet.microsoft.com/");
        Console.WriteLine("  Rx.NET: https://github.com/dotnet/reactive");
        Console.WriteLine("");
        Console.WriteLine("Used assets: ");
        Console.WriteLine("  Antenna icon by Icons8 (https://icons8.com/icon/oWXNXJ11wn0K/satellite-antenna)");
        Console.WriteLine("");
        Console.WriteLine("TSTU 2024");
    }
}
