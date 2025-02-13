using Hive.Movement;
using Hive.Pieces;

namespace Hive.Commands;

public class PassCommandHandler : ICommandHandler
{
    public string CommandString { get; } = "pass";

    public void Handle(Game game, string command)
    {
        try
        {
            game.PlayMove("pass");
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