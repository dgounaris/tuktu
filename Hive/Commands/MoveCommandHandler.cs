using Hive.Movement;
using Hive.Pieces;

namespace Hive.Commands;

public class MoveCommandHandler : ICommandHandler
{
    private readonly PieceMoveParser _pieceMoveParser = new();
    public string CommandString { get; } = "m";

    public void Handle(Game game, string command)
    {
        try
        {
            var (parsedPiece, parsedPosition) = _pieceMoveParser.Parse(game.Board, command[(command.IndexOf(' ') + 1)..]);
            var realPiece = game.GetPiece(parsedPiece.Color, parsedPiece.GetPieceIdentifier(), parsedPiece.PieceNumber);
            
            if (realPiece.GetValidMoves(game.Board).Contains(parsedPosition) is false)
            {
                throw new InvalidOperationException("Invalid move " + command);
            }
            
            var boardPiece = game.Board.GetPiece(parsedPiece.Color, parsedPiece.GetPieceIdentifier(), parsedPiece.PieceNumber);
            game.Board.Set(boardPiece, parsedPosition);

            Console.WriteLine("OK");
        }
        catch (Exception e)
        {
            Console.WriteLine("Unknown error");
            Console.WriteLine(e.Message);
        }
    }
}