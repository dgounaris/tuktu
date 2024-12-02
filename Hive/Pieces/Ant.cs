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
}