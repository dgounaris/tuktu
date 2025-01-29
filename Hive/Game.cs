using Hive.Movement;
using Hive.Pieces;

namespace Hive;

public class Game
{
    public Board Board;
    private int _currentPlayerIndex = -1;
    private int _currentTurn = -1;
    private Stack<(IPiece, Position?, Position)> MoveHistory = new ();
    
    public Game()
    {
        Board = new Board();
    }
    
    public void PlayMove(IPiece parsedPiece, Position parsedPosition)
    {
        var boardPiece = Board.GetPiece(parsedPiece.Color, parsedPiece.GetPieceIdentifier(), parsedPiece.PieceNumber);
        if (boardPiece.GetValidMoves(Board).Contains(parsedPosition) is false)
        {
            throw new InvalidOperationException($"Invalid move: piece {parsedPiece.Print()}, new position {parsedPosition}");
        }
            
        MoveHistory.Push((boardPiece, boardPiece.Position, parsedPosition));
        Board.Set(boardPiece, parsedPosition);
    }
    
    public void UndoLastMove()
    {
        var (piece, previousPosition, _) = MoveHistory.Pop();
        Board.Set(piece, previousPosition);
    }

    public Dictionary<IPiece, List<Position>> GetAllValidMoves(bool color)
    {
        Dictionary<IPiece, List<Position>> allValidMoves = new Dictionary<IPiece, List<Position>>();

        if (_currentTurn == 7 && Board.GetPiece(true, 'Q', 1).Position is null)
        {
            var queen = Board.GetPiece(true, 'Q', 1);
            allValidMoves.Add(queen, queen.GetValidMoves(Board).ToList());
            return allValidMoves;
        }
        else if (_currentTurn == 8 && Board.GetPiece(false, 'Q', 1).Position is null)
        {
            var queen = Board.GetPiece(false, 'Q', 1);
            allValidMoves.Add(queen, queen.GetValidMoves(Board).ToList());
            return allValidMoves;
        }
        
        foreach (var piece in Board.GetAll(color))
        {
            allValidMoves.Add(piece, piece.GetValidMoves(Board).ToList());
        }

        return allValidMoves;
    }

    public void Print()
    {
        Board.Print();
        Console.WriteLine("Board notation representation: " + ParseToNotation());
    }

    private string ParseToNotation()
    {
        return $":{(_currentPlayerIndex == 1 ? "b" : "w")}{Board.ParseToNotation()}";
    }

    // returns -1 for no game over, otherwise returns the index of the winning player
    public int IsGameOver()
    {
        for (int i = 0; i <= 1; i++)
        {
            if (Board.GetPiece(i == 0, 'Q', 1)!.Position != null)
            {
                bool isBeeSurrounded = true;
                foreach (var position in MovementUtilities.GetSurroundingPositions(Board.GetPiece(i == 0, 'Q', 1)!.Position!))
                {
                    if (Board.Get(position) is null)
                    {
                        isBeeSurrounded = false;
                    }
                }
                if (isBeeSurrounded)
                {
                    return i;
                }
            }
        }
        return -1;
    }
}