using Kursa4Crypto.Cli.Commands.Base;

namespace Kursa4Crypto.Cli.Commands;

[AutoRegister]
public class VerifierCommand : BaseCommand
{
    private readonly HelpCommand helpCommand;
    private readonly List<ICommand> subcommands;

    public VerifierCommand(Program program) : base(program)
    {
        subcommands = new()
        {
            new AddSubcommand(this),
        };

        helpCommand = new(subcommands);
    }

    public override string Name => "verifier";
    public override string Description => "Actions with verifiers";

    public override void Execute(string[] args)
    {
    }

    private class AddSubcommand(VerifierCommand command) : Command<VerifierCommand>(command)
    {
        public override string Name => "add";
        public override string Description => "Adds new verifier";

        public override void Execute(string[] args)
        {
            // TODO
        }
    }
}
