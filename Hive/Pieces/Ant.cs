using Hive.Movement;

namespace Hive.Pieces;

public class Ant : IPiece
{
    public Position? Position { get; set; }

    public char GetPieceIdentifier()
    {
        return 'A';
    }
    
    public bool Color { get; set; }
    
    public int PieceNumber { get; set; }
    public IEnumerable<Move> GetValidMoves(Board board)
    {
        if (Position is null)
        {
            return board.GetInitializablePositions(Color).Select(it => new Move { Piece = this, PreviousPosition = null, NewPosition = it });
        }

        if (board.GetAll(Position).Count > 1 || board.IsPieceHiveConnectivitySignificant(this))
        {
            return new List<Move>();
        }
        
        var validMoves = GetValidMovesRecursive(Position, board, new List<Position> { Position }).Distinct().ToList();
        validMoves.Remove(Position);
        return validMoves.Select(it => new Move { Piece = this, PreviousPosition = Position, NewPosition = it });;
    }
    
    private IEnumerable<Position> GetValidMovesRecursive(Position currentPosition, Board board, List<Position> traversedPositions)
    {
        var result = new List<Position>();

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

        result.Add(currentPosition);
        return result;
    }
}