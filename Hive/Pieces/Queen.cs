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
        var validMoves = new List<Position>();
        if (Position is null)
        {
            // todo return new positions
            return validMoves;
        }
        foreach (var candidatePosition in board.GetSurroundingPositions(Position!))
        {
            if (board.Get(candidatePosition) is null && board.IsPositionConnectedToHive(candidatePosition))
                // todo also 2 adjacent slots in the direction are not covered (to not create pocket)
            // function to return this will take both candidate and current position, because it will be reused for ant, spider and beetle
            {
                
                validMoves.Add(candidatePosition);
            }
        }
        return validMoves;
    }
}