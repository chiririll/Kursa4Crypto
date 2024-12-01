namespace Kursa4Crypto.Cli.Commands.Base;

public abstract class Command<TParent>(TParent parent) : ICommand
{
    protected TParent Parent { get; } = parent;

    public abstract string Name { get; }
    public abstract string Description { get; }
    public virtual string? OptionsString { get; } = null;

    public abstract void Execute(string[] args);
}

public abstract class BaseCommand(Program program) : Command<Program>(program)
{
};
