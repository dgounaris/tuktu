using Hive.Pieces;

namespace Hive.Heuristics;

public class PositionEvaluationResult
{
    public IPiece Piece { get; set; }
    public string Move { get; set; }
    public int Score { get; set; }
}