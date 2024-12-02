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
}