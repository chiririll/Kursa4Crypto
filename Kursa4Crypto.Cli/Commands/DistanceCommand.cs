using System.Numerics;

namespace Kursa4Crypto.Cli.Commands;

[AutoRegister]
public class DistanceCommand(Program program) : BaseCommand(program)
{
    public override string Name => "distance";
    public override string Description => "Calculates distance between entities";
    public override string? OptionsString => "<entity> <entity>";

    public override void Execute(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine($"Use: {Name} {OptionsString!}");
            Console.WriteLine($"E.g.: `{Name} p1 v3` - Calculates distance between Prover #1 and Verifier #3");
            return;
        }

        if (!TryGetEntityPosition(args[1], out var pos1) || !TryGetEntityPosition(args[2], out var pos2))
        {
            return;
        }

        Console.WriteLine($"Distance: {Vector2.Distance(pos1, pos2):0.##}");
    }

    private bool TryGetEntityPosition(string entityId, out Vector2 position)
    {
        position = default;

        if (entityId.Length < 2 || !int.TryParse(entityId[1..], out var id))
        {
            Console.WriteLine("Failed to parse entity id");
            return false;
        }

        switch (entityId[0])
        {
            case 'p':
                if (!Parent.State.Provers.TryGetValue(id, out var prover))
                {
                    Console.WriteLine($"Prover with id #{id} is not found!");
                    return false;
                }
                position = prover.Position;
                return true;
            case 'v':
                if (!Parent.State.Verifiers.TryGetValue(id, out var verifier))
                {
                    Console.WriteLine($"Verifier with id #{id} is not found!");
                    return false;
                }
                position = verifier.Position;
                return true;

        }

        return false;
    }
}
