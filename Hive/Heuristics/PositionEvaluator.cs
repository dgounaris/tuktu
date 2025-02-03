using Hive.Movement;
using Hive.Pieces;

namespace Hive.Heuristics;

public class PositionEvaluator
{
    public List<PositionEvaluationResult> Evaluate(Board board, bool color, int maxDepth)
    {
        List<PositionEvaluationResult> result = [];
        var allValidMoves = board.GetAllValidMoves(color).SelectMany(it => it.Value.Select(p => (it.Key, p)));

        int alpha = -1000000;
        int beta = 1000000;
        
        foreach (var move in allValidMoves)
        {
            var previousPosition = move.Key.Position;
            board.Set(move.Key, move.p);
            var localEval = -Evaluate(board, !color, maxDepth - 1, -beta, -alpha);
            board.Set(move.Key, previousPosition);
            
            result.Add(new PositionEvaluationResult
            {
                Piece = board.GetPiece(move.Key.Color, move.Key.GetPieceIdentifier(), move.Key.PieceNumber),
                Move = PieceMoveParsingUtilities.PositionToMove(board, move.p),
                Score = localEval
            });
            alpha = Math.Max(alpha, localEval);
        }

        return result.OrderByDescending(it => it.Score).ToList();
    }

    private int Evaluate(Board board, bool color, int depth, int alpha, int beta)
    {
        if (depth == 0)
        {
            var score = CalculateEvaluation(board, true) - CalculateEvaluation(board, false);
            return color ? score : -score;
        }
        
        var allValidMoves = board.GetAllValidMoves(color).SelectMany(it => it.Value.Select(p => (it.Key, p)));
        var localEval = -1000000;
        foreach (var move in allValidMoves)
        {
            var previousPosition = move.Key.Position;
            board.Set(move.Key, move.p);
            localEval = Math.Max(localEval, -Evaluate(board, !color, depth-1, -beta, -alpha));
            alpha = Math.Max(alpha, localEval);
            board.Set(move.Key, previousPosition);
            if (alpha > beta)
            {
                break;
            }
        }

        return localEval;
    }

    private int CalculateEvaluation(Board board, bool color)
    {
        var evaluation = 0;
        
        // Check if the queen is surrounded
        var queen = board.GetPiece(color, 'Q', 1);
        if (queen.Position is not null)
        {
            var surroundingPieces = MovementUtilities.GetSurroundingPositions(queen.Position)
                .Select(it => board.Get(it)).Where(it => it is not null).ToList();
            if (surroundingPieces.Count == 6)
            {
                return -1000000;
            }
            var queenSurroundedScore = surroundingPieces.Aggregate(0, (sum, piece) => sum += piece!.Color == color ? 50 : 100);
            evaluation -= queenSurroundedScore;
        }
        
        // Check if the queen can move
        if (queen.Position is not null)
        {
            var queenValidMoves = queen.GetValidMoves(board).ToList();
            if (queenValidMoves.Count == 0)
            {
                evaluation -= 25;
            }
        }
        
        // Check the amount of possible moves
        var allValidMoves = board.GetAllValidMoves(color);
        evaluation += allValidMoves.Count;

        return evaluation;
    }
}