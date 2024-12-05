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

        return candidatePositions.Where(it => board.Get(it) is null &&
                                              board.IsPositionConnectedToHive(it) &&
                                              board.IsAdjacentPositionSlideReachable(Position, it));
    }
}