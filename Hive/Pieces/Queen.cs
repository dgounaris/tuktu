using Hive.Movement;

namespace Hive.Pieces;

public class Queen : IPiece
{
    public Position? Position { get; set; }

    public char GetPieceIdentifier()
    {
        return 'Q';
    }
    
    public bool Color { get; set; }
    
    public int PieceNumber { get; set; }
    
    public IEnumerable<Move> GetValidMoves(Board board)
    {
        if (Position is null)
        {
            return board.GetInitializablePositions(Color).Select(it => new Move { Piece = this, PreviousPosition = null, NewPosition = it });
        }
        var candidatePositions = MovementUtilities.GetSurroundingPositions(Position!);

        return candidatePositions.Where(it => board.Get(it) is null &&
                                              board.GetAll(Position).Count == 1 &&
                                              board.IsPositionConnectedToHive(it).Count > 1 && // should "connect" to the piece and also to the rest of hive 
                                              !board.IsPieceHiveConnectivitySignificant(this) &&
                                              board.IsAdjacentPositionSlideReachable(Position, it))
            .Select(it => new Move { Piece = this, PreviousPosition = Position, NewPosition = it });
    }
}