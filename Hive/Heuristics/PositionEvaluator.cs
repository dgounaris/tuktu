using Hive.Movement;
using Hive.Pieces;

namespace Hive.Heuristics;

public class PositionEvaluator
{
    public List<PositionEvaluationResult> Evaluate(Game game, bool color, int maxDepth)
    {
        List<PositionEvaluationResult> result = [];
        var allValidMoves = game.GetAllValidMoves(color).SelectMany(it => it.Value.Select(p => (it.Key, p)));

        foreach (var move in allValidMoves)
        {
            game.Board.Set(move.Key, move.p);
            var localEval = Evaluate(game, !color, maxDepth - 1,
                color ? Double.NegativeInfinity : Double.PositiveInfinity, color ? Double.PositiveInfinity : Double.NegativeInfinity);
            game.Board.UndoLastMove();
            
            result.Add(new PositionEvaluationResult
            {
                Piece = game.GetPiece(move.Key.Color, move.Key.GetPieceIdentifier(), move.Key.PieceNumber),
                Move = PieceMoveParsingUtilities.PositionToMove(game.Board, move.p),
                Score = localEval
            });
        }

        return result;
    }

    public double Evaluate(Game game, bool color, int depth, double aScore, double bScore)
    {
        if (depth == 0 || (game.IsGameOver() != -1))
        {
            return color ? CalculateEvaluation(game) : -CalculateEvaluation(game);
        }
        
        var allValidMoves = game.GetAllValidMoves(color).SelectMany(it => it.Value.Select(p => (it.Key, p)));
        double localEval = Double.NegativeInfinity;
        foreach (var move in allValidMoves)
        {
            game.Board.Set(move.Key, move.p);
            localEval = Math.Max(localEval, -Evaluate(game, !color, depth-1, -bScore, -aScore));
            aScore = Math.Max(aScore, localEval);
            game.Board.UndoLastMove();
            if (aScore >= bScore)
            {
                break;
            }
        }

        return localEval;
    }

    private double CalculateEvaluation(Game game)
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
                evaluation += allValidMoves.Count * 0.1;
            }
            else
            {
                evaluation -= allValidMoves.Count * 0.1;
            }
        }

        return evaluation;
    }
}