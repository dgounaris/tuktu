namespace Hive.Pieces;

public static class PieceUtilities
{
    public static IPiece ResolvePieceFromId(char pieceId)
    {
        return pieceId switch
        {
            'Q' => new Queen(),
            'B' => new Beetle(),
            'G' => new Grasshopper(),
            'S' => new Spider(),
            'A' => new Ant(),
            _ => throw new Exception($"Unknown piece type {pieceId}")
        };
    }
}