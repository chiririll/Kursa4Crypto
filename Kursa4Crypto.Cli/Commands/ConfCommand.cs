using System.Globalization;
using System.Numerics;
using Kursa4Crypto.Cli.Commands.Base;

namespace Kursa4Crypto.Cli.Commands;

[AutoRegister]
public class ConfCommand(Program program) : BaseCompositeCommand(program)
{
    public override string Name => "conf";
    public override string Description => "Modify settings of the simulation";

    protected override string HelpText => $"Type '{Name} {helpCommand.Name}' to print list of available params";

    protected override List<ICommand> CreateChildren()
    {
        var settings = Parent.State.SignalSpaceSettings;

        return [
            new ParamSubcommand<float>("fade", "Signal fade",
                () => settings.SignalFade,
                v => settings.SignalFade = v,
                this),

            new ParamSubcommand<float>("threshold", "Signal fade threshold",
                () => settings.SignalFadeThreshold,
                v => settings.SignalFadeThreshold = v,
                this),

            new ParamSubcommand<float>("speed", "Signal speed",
                () => settings.SignalSpeed,
                v => settings.SignalSpeed = v,
                this),

            new ParamSubcommand<float>("step", "Step duration",
                () => settings.StepDuration,
                v => settings.StepDuration = v,
                this),

            new ResetSubcommand(this),
        ];
    }

    protected override void ExecuteEmptyChild(string[] args)
    {
        Console.WriteLine("Current configuration:");
        foreach (var child in children)
        {
            if (child is not IParamSubcommand paramSubcommand)
                continue;

            paramSubcommand.PrintCurrentValue();
        }
    }

    private interface IParamSubcommand
    {
        public void PrintCurrentValue();
    }

    private class ParamSubcommand<TType>(
        string name, string description,
        Func<TType> getter, Action<TType> setter,
        ConfCommand parent) : Command<ConfCommand>(parent), IParamSubcommand
        where TType : INumber<TType>
    {
        private readonly Func<TType> getter = getter;
        private readonly Action<TType> setter = setter;

        public override string Name => name;
        public override string Description => description;

        public override void Execute(string[] args)
        {
            if (args.Length < 2)
            {
                PrintCurrentValue();
                return;
            }

            if (!TType.TryParse(args[1], CultureInfo.CurrentCulture, out var value))
            {
                Console.WriteLine("Invalid parameter value");
            }

            var prevValue = getter.Invoke();
            setter.Invoke(value!);

            Console.WriteLine($"Value changed: {prevValue} -> {value}");
        }

        public void PrintCurrentValue()
        {
            Console.WriteLine($"{Description}: {getter.Invoke()}");
        }
    }

    private class ResetSubcommand(ConfCommand parent) : Command<ConfCommand>(parent)
    {
        public override string Name => "reset";
        public override string Description => "Reset settings to default";

        public override void Execute(string[] args)
        {
            Parent.Parent.State.SignalSpaceSettings.CopyFrom(new());
            Console.WriteLine("All settings has been reset");
        }
    }
}
