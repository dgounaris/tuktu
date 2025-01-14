namespace Hive.Commands;

public interface ICommandHandler
{
    public string CommandString { get; }
    public void Handle(Game game, string command);
}