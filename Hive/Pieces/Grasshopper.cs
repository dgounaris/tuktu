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
        throw new NotImplementedException();
    }
}