namespace Kursa4Crypto.Cli.Commands;

public interface ICommand : IDisposable
{
    public string Name { get; }

    public void Execute(string[] args);
}
