namespace Hive.Commands;

public class PrintCommandHandler : ICommandHandler
{
    public string CommandString { get; } = "p";

    public void Handle(Game game, string command)
    {
        game.Print();
    }
}