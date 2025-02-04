using Hive.Movement;
using Hive.Pieces;

namespace Hive.Commands;

public class MoveCommandHandler : ICommandHandler
{
    public string CommandString { get; } = "m";

    public void Handle(Game game, string command)
    {
        try
        {
            game.PlayMove(command[(command.IndexOf(' ') + 1)..]);

            Console.WriteLine("OK");
        }
        catch (Exception e)
        {
            Console.WriteLine("Unknown error");
            Console.WriteLine(e.Message);
        }
    }
}