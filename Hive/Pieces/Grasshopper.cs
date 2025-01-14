using Hive.Movement;

namespace Hive.Pieces;

public class Grasshopper : IPiece
{
    public Position? Position { get; set; }

    public char GetPieceIdentifier()
    {
        return 'G';
    }

    public bool Color { get; set; }
    
    public int PieceNumber { get; set; }
    public IEnumerable<Position> GetValidMoves(Board board)
    {
        if (Position is null)
        {
            return board.GetInitializablePositions(Color);
        }
        
        if (board.GetAll(Position).Count > 1 || board.IsPieceHiveConnectivitySignificant(this))
        {
            return new List<Position>();
        }
        
        var result = new List<Position>();
        
        var surroundingPositions = MovementUtilities.GetSurroundingPositions(Position!);
        foreach (var surroundingPosition in surroundingPositions.Where(it => board.Get(it) is not null))
        {
            var deltaQ = surroundingPosition.Q - Position!.Q;
            var deltaR = surroundingPosition.R - Position!.R;
            var latestDirectionalPosition = surroundingPosition;
            while (board.Get(latestDirectionalPosition) is not null)
            {
                latestDirectionalPosition = new Position(latestDirectionalPosition.Q + deltaQ, latestDirectionalPosition.R + deltaR);
            }
            result.Add(latestDirectionalPosition);
            // todo testing
        }

        return result;
    }
}