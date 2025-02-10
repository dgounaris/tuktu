using Hive.Movement;

namespace Hive.Pieces;

public class Beetle : IPiece
{
    public Position? Position { get; set; }

    public char GetPieceIdentifier()
    {
        return 'B';
    }
    
    public bool Color { get; set; }
    
    public int PieceNumber { get; set; }

    public IEnumerable<Move> GetValidMoves(Board board)
    {
        if (Position is null)
        {
            return board.GetInitializablePositions(Color).Select(it => new Move { Piece = this, PreviousPosition = null, NewPosition = it });
        }

        if (board.Get(Position) != this) // this beetle is not the top piece in case of stacked pieces
        {
            return new List<Move>();
        }
        
        var candidatePositions = MovementUtilities.GetSurroundingPositions(Position!);
        return candidatePositions.Where(it => IsEmptyPositionToMoveTo(board, it) || IsOccupiedPositionToJumpOn(board, it))
            .Select(it => new Move { Piece = this, PreviousPosition = Position, NewPosition = it });
    }

    private bool IsEmptyPositionToMoveTo(Board board, Position it)
    {
        return board.Get(it) is null &&
               board.IsPositionConnectedToHive(it).Count > 1 && // should "connect" to the piece and also to the rest of hive 
               !board.IsPieceHiveConnectivitySignificant(this) &&
               (board.IsAdjacentPositionSlideReachable(Position, it) || board.GetAll(Position).Count > 1);
    }
    
    private bool IsOccupiedPositionToJumpOn(Board board, Position it)
    {
        return board.Get(it) is not null &&
               (board.IsPositionConnectedToHive(it).Count > 1 || board.GetAll(Position).Count > 1) &&
               !board.IsPieceHiveConnectivitySignificant(this) &&
               MovementUtilities.GetSurroundingPositions(Position).Contains(it);
    }
}