namespace Kursa4Crypto.Cli.Commands;

public interface ICommand : IExecutable
{
    public string Name { get; }
    public string Description { get; }
}
