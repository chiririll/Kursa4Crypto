using System.Globalization;
using System.Numerics;

namespace Kursa4Crypto.Cli.Commands;

public abstract class Command<TParent>(TParent parent) : ICommand
{
    protected TParent Parent { get; } = parent;

    public abstract string Name { get; }
    public abstract string Description { get; }
    public virtual string? OptionsString { get; } = null;

    public abstract void Execute(string[] args);

    protected static bool GetArgument<T>(string[] args, int index, out T value)
        where T : struct, INumber<T>
    {
        value = default;
        return args.Length > index
            && T.TryParse(args[index], CultureInfo.CurrentCulture, out value);
    }

    protected bool TryGetPosition(string[] args, out Vector2 position, int offset = 0)
    {
        if ((GetArgument<float>(args, offset + 1, out var x) || InvalidArgument(nameof(x))) &&
            (GetArgument<float>(args, offset + 2, out var y) || InvalidArgument(nameof(y))))
        {
            position = new(x, y);
            return true;
        }

        position = default;
        return false;
    }

    protected bool InvalidArgument(string name)
    {
        Console.WriteLine($"Invalid '{name}' argument!");

        var parentString = Parent is ICommand parentCommand ? $"{parentCommand.Name} " : string.Empty;
        Console.WriteLine($"Use: {parentString}{Name} {OptionsString}");

        return false;
    }
}

public abstract class BaseCommand(Program program) : Command<Program>(program)
{
};
