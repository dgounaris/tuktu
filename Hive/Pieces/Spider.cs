using Hive.Movement;

namespace Hive.Pieces;

public class Spider : IPiece
{
    public Position? Position { get; set; }

    public char GetPieceIdentifier()
    {
        return 'S';
    }
    
    public bool Color { get; set; }
    
    public int PieceNumber { get; set; }
    public IEnumerable<Position> GetValidMoves(Board board)
    {
        var originalPosition = Position;
        if (Position is null)
        {
            return board.GetInitializablePositions(Color);
        }

        if (board.GetAll(Position).Count > 1)
        {
            return new List<Position>();
        }
        
        return GetValidMovesRecursive(Position, board, new List<Position> { Position }).Distinct();
    }

    private IEnumerable<Position> GetValidMovesRecursive(Position currentPosition, Board board, List<Position> traversedPositions)
    {
        var result = new List<Position>();
        if (traversedPositions.Count > 3)
        {
            return new List<Position> { currentPosition };
        }
        
        var candidatePositions = MovementUtilities.GetSurroundingPositions(Position!);

        foreach (var candidatePosition in candidatePositions)
        {
            if (traversedPositions.Contains(candidatePosition))
            {
                continue;
            }
            
            if (board.Get(candidatePosition) is null &&
                board.IsPositionConnectedToHive(candidatePosition).Count > 1 && // should "connect" to the piece and also to the rest of hive 
                !board.IsPieceHiveConnectivitySignificant(this) &&
                board.IsAdjacentPositionSlideReachable(currentPosition, candidatePosition))
            {
                var previousPosition = this.Position;
                board.Set(this, candidatePosition);
                result.AddRange(GetValidMovesRecursive(candidatePosition, board, traversedPositions.Append(candidatePosition).ToList()));
                board.Set(this, previousPosition);
            }
        }

        return result;
    }
}