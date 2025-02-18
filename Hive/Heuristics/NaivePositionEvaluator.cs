using Hive.Movement;
using Hive.Pieces;

namespace Hive.Heuristics;

public class NaivePositionEvaluator
{
    private Random _random = new Random();
    
    public Move Evaluate(Game game, int maxDepth)
    {
        var allValidMoves = game.GetAllValidMoves();
        var selectedIndex = _random.Next(allValidMoves.Count);
        return allValidMoves[selectedIndex];
    }
    
    public Move Evaluate(Game game, TimeSpan maxTime)
    {
        var allValidMoves = game.GetAllValidMoves();
        var selectedIndex = _random.Next(allValidMoves.Count);
        return allValidMoves[selectedIndex];
    }
}