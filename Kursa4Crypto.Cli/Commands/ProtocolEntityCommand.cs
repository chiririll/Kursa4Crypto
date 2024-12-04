using System.Globalization;
using System.Numerics;
using Kursa4Crypto.Protocol;

namespace Kursa4Crypto.Cli.Commands;

[AutoRegister]
public abstract class ProtocolEntityCommand<TEntity, TCommand>(Program program, string entityName) : BaseCompositeCommand(program)
    where TEntity : ProtocolEntity
    where TCommand : ProtocolEntityCommand<TEntity, TCommand>
{
    private readonly string entityName = entityName;
    private readonly string entityNameTitle = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(entityName);
    protected abstract Dictionary<int, TEntity> Entities { get; }

    protected virtual string ConfigurationOptions => string.Empty;

    protected abstract TEntity CreateEntity();
    protected virtual void ConfigureEntity(TEntity entity, string[] args, int offset) { }

    protected virtual void LogMessage(int entityId, string message)
    {
        Console.WriteLine($"{entityNameTitle} #{entityId}: {message}");
    }

    protected class ListCommand(TCommand parent) : Command<TCommand>(parent)
    {
        public override string Name => "list";
        public override string Description => $"List of all {Parent.entityName}s";

        public override void Execute(string[] args)
        {
            if (Parent.Entities.Count < 1)
            {
                Console.WriteLine($"There are no {Parent.entityName}s");
                return;
            }

            foreach (var entity in Parent.Entities.Values)
            {
                Console.WriteLine(entity.ToString());
            }
        }
    }

    protected class AddCommand(TCommand parent, float defaultForce = 1f, float defaultTimeout = 1f) : Command<TCommand>(parent)
    {
        public override string Name => "add";
        public override string Description => $"Adds new {Parent.entityName}";
        public override string? OptionsString => $"<x> <y> [force={defaultForce:0.##}] [timeout={defaultTimeout:0.##}] " + Parent.ConfigurationOptions;

        protected bool ParseArguments(string[] args, out Vector2 position, out float force, out float timeout)
        {
            if (!TryGetPosition(args, out position))
            {
                force = default;
                timeout = default;
                position = default;
                return false;
            }

            if (!GetArgument(args, 3, out force)) force = defaultForce;
            if (!GetArgument(args, 4, out timeout)) timeout = defaultTimeout;

            return true;
        }

        public override void Execute(string[] args)
        {
            if (!ParseArguments(args, out var pos, out var force, out var timeout))
                return;

            var entity = Parent.CreateEntity();
            entity.Position = pos;
            entity.TransmitForce = force;
            entity.ProveTimeout = timeout;

            Parent.ConfigureEntity(entity, args, 4);

            Parent.Entities.Add(entity.Id, entity);
            Console.WriteLine($"Added new {Parent.entityName} with id {entity.Id}");

            entity.Messenger.Subscribe(_ => Parent.LogMessage(_.proverId, _.message)).AddTo(entity);
        }
    }

    protected class RemoveCommand(TCommand parent) : Command<TCommand>(parent)
    {
        public override string Name => "remove";
        public override string Description => $"Removes {Parent.entityName}";
        public override string? OptionsString => "<id>";

        public override void Execute(string[] args)
        {
            if (!GetArgument<int>(args, 1, out var id) || !Parent.Entities.TryGetValue(id, out var entity))
            {
                Console.WriteLine("Invalid id!");
                return;
            }

            entity.Dispose();
            Parent.Entities.Remove(id);

            Console.WriteLine($"Successfully removed {Parent.entityName} with id {id}!");
        }
    }

    protected class MoveCommand(TCommand parent) : Command<TCommand>(parent)
    {
        public override string Name => "move";
        public override string Description => $"Moves {Parent.entityName}";
        public override string? OptionsString => "<x> <y>";

        public override void Execute(string[] args)
        {
            if (!GetArgument<int>(args, 1, out var id) || !Parent.Entities.TryGetValue(id, out var entity))
            {
                InvalidArgument(nameof(id));
                return;
            }

            if (!TryGetPosition(args, out var position, 1))
                return;

            var lastPos = entity.Position;
            entity.Position = position;

            Console.WriteLine($"Moved {Parent.entityName} from ({lastPos.X}, {lastPos.Y}) to ({position.X}, {position.Y})");
        }
    }
}
