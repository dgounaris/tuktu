using Hive.Pieces;

namespace Hive.Movement;

public record Move
{
    public MoveType MoveType { get; set; } = MoveType.Action;
    public IPiece Piece { get; set; }
    public Position? PreviousPosition { get; set; }
    public Position NewPosition { get; set; }
    public string MoveString { get; set; } = string.Empty;
    
    public virtual bool Equals(Move? item)
    {
        if (item is null)
        {
            return false;
        }

        return this.Piece.Equals(item.Piece) &&
               this.PreviousPosition == item.PreviousPosition &&
               this.NewPosition == item.NewPosition;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(this.Piece.GetHashCode(), this.PreviousPosition?.GetHashCode(), this.NewPosition.GetHashCode());
    }
}

public enum MoveType
{
    Action,
    Pass
}