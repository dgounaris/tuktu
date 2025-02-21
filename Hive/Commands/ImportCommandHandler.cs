namespace Hive.Commands;

public class ImportCommandHandler : ICommandHandler
{
    public string CommandString { get; } = "i";

    public void Handle(Game game, string command)
    {
        game.LoadFromNotation(command[(command.IndexOf(' ') + 1)..]);
        game.Print();
    }
}