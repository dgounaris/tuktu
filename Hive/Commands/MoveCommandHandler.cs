using Hive.Movement;
using Hive.Pieces;

namespace Hive.Commands;

public class MoveCommandHandler : ICommandHandler
{
    private readonly PieceMoveParser _pieceMoveParser = new();
    public string CommandString { get; } = "m";

    public void Handle(Board board, string command)
    {
        try
        {
            IPiece piece = _pieceMoveParser.Parse(board, command[(command.IndexOf(' ') + 1)..]);
            var boardPiece = board.GetPiece(piece.Color, piece.GetPieceIdentifier(), piece.PieceNumber);
            if (boardPiece is null)
            {
                board.Set(piece);
            }
            else
            {
                board.Remove(boardPiece.Position!);
                board.Set(piece);
            }

            Console.WriteLine("OK");
        } 
        catch (Exception e)
        {
            Console.WriteLine("Unknown error");
            Console.WriteLine(e.Message);
        }
    }
}