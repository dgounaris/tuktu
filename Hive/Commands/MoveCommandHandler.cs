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
            var (parsedPiece, parsedPosition) = PieceMoveParsingUtilities.Parse(game.Board, command[(command.IndexOf(' ') + 1)..]);
            game.PlayMove(parsedPiece, parsedPosition);

            Console.WriteLine("OK");
        }
        catch (Exception e)
        {
            Console.WriteLine("Unknown error");
            Console.WriteLine(e.Message);
        }
    }
}