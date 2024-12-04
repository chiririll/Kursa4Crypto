namespace Kursa4Crypto.Cli.Commands;

public abstract class CompositeCommand<TBase> : Command<TBase>
{
    protected readonly List<ICommand> children;
    protected readonly HelpCommand helpCommand;

    protected CompositeCommand(TBase parent) : base(parent)
    {
        children = CreateChildren();
        helpCommand = new(children);
    }

    public override string? OptionsString => "<subcommand>";

    protected virtual string HelpText => $"Type '{Name} {helpCommand.Name}' to print list of available subcommands";

    protected abstract List<ICommand> CreateChildren();

    public sealed override void Execute(string[] args)
    {
        if (args.Length < 2)
        {
            ExecuteEmptyChild(args);
            return;
        }

        var childName = args[1].ToLower();
        var child = childName switch
        {
            HelpCommand.ConstName => helpCommand,
            _ => children.Find(c => c.Name.Equals(childName)),
        };

        if (child != null)
        {
            child.Execute(args.Skip(1).ToArray());
            return;
        }

        ExecuteUnknownChild(args);
    }

    protected virtual void ExecuteEmptyChild(string[] args)
    {
        Console.WriteLine(HelpText);
    }

    protected virtual void ExecuteUnknownChild(string[] args)
    {
        Console.WriteLine($"Unknown subcommand '{args[1]}'. " + HelpText);
    }
}

public abstract class BaseCompositeCommand(Program program) : CompositeCommand<Program>(program)
{
}
