using Hive.Movement;
using Hive.Pieces;

namespace Hive.Heuristics;

public class PositionEvaluator
{
    public List<PositionEvaluationResult> Evaluate(Game game, bool color, int maxDepth)
    {
        List<PositionEvaluationResult> result = [];
        var allValidMoves = game.GetAllValidMoves(color).SelectMany(it => it.Value.Select(p => (it.Key, p)));

        int alpha = int.MinValue;
        int beta = int.MaxValue;
        
        foreach (var move in allValidMoves)
        {
            game.PlayMove(move.Key, move.p);
            var localEval = -Evaluate(game, !color, maxDepth - 1, -beta, -alpha);
            game.UndoLastMove();
            
            result.Add(new PositionEvaluationResult
            {
                Piece = game.Board.GetPiece(move.Key.Color, move.Key.GetPieceIdentifier(), move.Key.PieceNumber),
                Move = PieceMoveParsingUtilities.PositionToMove(game.Board, move.p),
                Score = localEval
            });
            alpha = Math.Max(alpha, localEval);
        }

        return result.OrderByDescending(it => it.Score).ToList();
    }

    private int Evaluate(Game game, bool color, int depth, int alpha, int beta)
    {
        if (depth == 0 || (game.IsGameOver() != -1))
        {
            var score = CalculateEvaluation(game, true) - CalculateEvaluation(game, false);
            return color ? score : -score;
        }
        
        var allValidMoves = game.GetAllValidMoves(color).SelectMany(it => it.Value.Select(p => (it.Key, p)));
        int localEval = int.MinValue;
        foreach (var move in allValidMoves)
        {
            game.PlayMove(move.Key, move.p);
            localEval = Math.Max(localEval, -Evaluate(game, !color, depth-1, -beta, -alpha));
            alpha = Math.Max(alpha, localEval);
            game.UndoLastMove();
            if (alpha >= beta)
            {
                break;
            }
        }

        return localEval;
    }

    private int CalculateEvaluation(Game game, bool color)
    {
        var evaluation = 0;
        
        // Check if the queen is surrounded
        var queen = game.Board.GetPiece(color, 'Q', 1);
        if (queen.Position is not null)
        {
            var surroundingPieces = MovementUtilities.GetSurroundingPositions(queen.Position)
                .Select(it => game.Board.Get(it)).Where(it => it is not null).ToList();
            if (surroundingPieces.Count == 6)
            {
                return int.MinValue;
            }
            var queenSurroundedScore = surroundingPieces.Aggregate(0, (sum, piece) => sum += piece!.Color == color ? 50 : 100);
            evaluation -= queenSurroundedScore;
        }
        
        // Check if the queen can move
        if (queen.Position is not null)
        {
            var queenValidMoves = queen.GetValidMoves(game.Board).ToList();
            if (queenValidMoves.Count == 0)
            {
                evaluation -= 25;
            }
        }
        
        // Check the amount of possible moves
        var allValidMoves = game.GetAllValidMoves(color);
        evaluation += allValidMoves.Count;

        return evaluation;
    }
}