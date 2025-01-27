using Hive.Pieces;

namespace Hive.Heuristics;

public class PositionEvaluationResult
{
    public IPiece Piece { get; set; }
    public string Move { get; set; }
    public double Score { get; set; }
}