using Hive.Pieces;

namespace Hive.Movement;

public record Move
{
    public MoveType MoveType { get; set; } = MoveType.Action;
    public IPiece Piece { get; set; }
    public Position? PreviousPosition { get; set; }
    public Position NewPosition { get; set; }
}

public enum MoveType
{
    Action,
    Pass
}