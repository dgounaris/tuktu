using Hive.Movement;

namespace Hive.Heuristics;

public class PositionEvaluator
{
    // todo use this function in some command
    public double Evaluate(Game game)
    {
        var evaluation = 0.0;
        for (var i = 0; i <= 1; i++)
        {
            var evaluatingColor = i == 0;
            
            // Check if the queen is surrounded
            var queen = game.Board.GetPiece(evaluatingColor, 'Q', 1);
            if (queen.Position is not null)
            {
                var surroundingPieces = MovementUtilities.GetSurroundingPositions(queen.Position)
                    .Select(it => game.Board.Get(it)).Where(it => it is not null);
                var queenSurroundedScore = surroundingPieces.Aggregate(0.0, (sum, piece) => sum += piece!.Color == evaluatingColor ? 0.5 : 1);
                if (evaluatingColor)
                {
                    evaluation -= queenSurroundedScore;
                }
                else
                {
                    evaluation += queenSurroundedScore;
                }
            }
            
            // Check if the queen can move
            if (queen.Position is not null)
            {
                var queenValidMoves = queen.GetValidMoves(game.Board).ToList();
                if (queenValidMoves.Count == 0)
                {
                    if (evaluatingColor)
                    {
                        evaluation -= 3;
                    }
                    else
                    {
                        evaluation += 3;
                    }
                }
            }
            
            // Check the amount of possible moves
            var allValidMoves = game.GetAllValidMoves(evaluatingColor);
            if (evaluatingColor)
            {
                evaluation -= allValidMoves.Count * 0.1;
            }
            else
            {
                evaluation += allValidMoves.Count * 0.1;
            }
        }

        return evaluation;
    }
}