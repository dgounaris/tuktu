using System.Runtime.CompilerServices;

namespace Hive.Movement;

public static class MovementUtilities
{
    // Piece-
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Position GetPositionSW(Position p)
    {
        return new Position(p.Q - 1, p.R + 1);
    }
    
    // -Piece
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Position GetPositionW(Position p)
    {
        return new Position(p.Q - 1, p.R);
    }
    
    // \Piece
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Position GetPositionNW(Position p)
    {
        return new Position(p.Q, p.R - 1);
    }
    
    // Piece/
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Position GetPositionNE(Position p)
    {
        return new Position(p.Q + 1, p.R - 1);
    }
    
    // Piece-
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Position GetPositionE(Position p)
    {
        return new Position(p.Q + 1, p.R);
    }
    
    // Piece\
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Position GetPositionSE(Position p)
    {
        return new Position(p.Q, p.R + 1);
    }
    
    public static IEnumerable<Position> GetSurroundingPositions(Position position)
    {
        var positions = new List<Position>
        {
            GetPositionSW(position),
            GetPositionW(position),
            GetPositionNW(position),
            GetPositionNE(position),
            GetPositionE(position),
            GetPositionSE(position)
        };
        return positions;
    }
}