using Hive.Movement;
using Hive.Pieces;

namespace Hive.Commands;

public class PlayCommandHandler : ICommandHandler
{
    public string CommandString { get; } = "play";

    public void Handle(Game game, string command)
    {
        try
        {
            game.PlayMove(command[(command.IndexOf(' ') + 1)..]);
            game.PrintUHP();
            Console.WriteLine("ok");
        }
        catch (Exception e)
        {
            Console.WriteLine("Unknown error");
            Console.WriteLine(e.Message);
        }
    }
}