namespace Hive.Commands;

public interface ICommandHandler // todo support all mzinga commands
{
    public string CommandString { get; }
    public void Handle(Game game, string command);
}