using System.Globalization;
using System.Numerics;

namespace Kursa4Crypto.Cli.Commands.Base;

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
}

public abstract class BaseCommand(Program program) : Command<Program>(program)
{
};
