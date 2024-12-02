namespace Hive.Commands;

public interface ICommandHandler
{
    public string CommandString { get; }
    public void Handle(Board board, string command);
}