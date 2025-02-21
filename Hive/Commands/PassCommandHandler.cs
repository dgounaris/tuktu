using Hive.Movement;
using Hive.Pieces;

namespace Hive.Commands;

public class PassCommandHandler : ICommandHandler
{
    public string CommandString { get; } = "pass";

    public void Handle(Game game, string command)
    {
        game.PlayMove("pass");
        game.PrintUHP();
    }
}