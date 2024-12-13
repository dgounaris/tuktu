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

    public IEnumerable<Position> GetValidMoves(Board board)
    {
        if (Position is null)
        {
            return board.GetInitializablePositions(Color);
        }
        var candidatePositions = MovementUtilities.GetSurroundingPositions(Position!);
        if (Position is null)
        {
            return candidatePositions.Where(it => board.IsPositionInitializable(it, Color));
        }
        return candidatePositions.Where(it => IsEmptyPositionToMoveTo(board, it) || IsOccupiedPositionToJumpOn(board, it));
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