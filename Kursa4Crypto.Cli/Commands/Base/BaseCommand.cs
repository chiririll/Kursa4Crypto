namespace Kursa4Crypto.Cli.Commands.Base;

public abstract class BaseCommand(Program program) : ICommand
{
    protected Program Program { get; private set; } = program;

    public abstract string Name { get; }
    public abstract string Description { get; }
    public virtual string? OptionsString { get; } = null;

    public abstract void Execute(string[] args);
}
