using Hive.Movement;
using Hive.Pieces;

namespace Hive.Commands;

public class PlayCommandHandler : ICommandHandler
{
    public string CommandString { get; } = "play";

    public void Handle(Game game, string command)
    {
        game.PlayMove(command[(command.IndexOf(' ') + 1)..]);
        game.PrintUHP();
    }
}