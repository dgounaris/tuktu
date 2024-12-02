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
}