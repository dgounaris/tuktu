using Hive.Movement;
using Hive.Pieces;

namespace Hive.Heuristics;

public class PositionEvaluator
{
    public List<PositionEvaluationResult> Evaluate(Game game, bool color, int maxDepth)
    {
        if (maxDepth == 0 || (game.IsGameOver() != -1))
        {
            var score = CalculateEvaluation(game, true) - CalculateEvaluation(game, false);
        }
        
        List<PositionEvaluationResult> result = [];
        var allValidMoves = game.GetAllValidMoves(color).SelectMany(it => it.Value.Select(p => (it.Key, p)));

        foreach (var move in allValidMoves)
        {
            game.Board.Set(move.Key, move.p);
            var localEval = Evaluate(game, !color, maxDepth - 1,
                color ? Int32.MinValue : Int32.MaxValue, color ? Int32.MaxValue : Int32.MinValue);
            game.Board.UndoLastMove();
            
            result.Add(new PositionEvaluationResult
            {
                Piece = game.GetPiece(move.Key.Color, move.Key.GetPieceIdentifier(), move.Key.PieceNumber),
                Move = PieceMoveParsingUtilities.PositionToMove(game.Board, move.p),
                Score = localEval
            });
        }

        return color
            ? result.OrderByDescending(it => it.Score).ToList()
            : result.OrderBy(it => it.Score).ToList();
    }

    private int Evaluate(Game game, bool color, int depth, int alpha, int beta)
    {
        if (depth == 0 || (game.IsGameOver() != -1))
        {
            return CalculateEvaluation(game, true) - CalculateEvaluation(game, false);
        }
        
        var allValidMoves = game.GetAllValidMoves(color).SelectMany(it => it.Value.Select(p => (it.Key, p)));
        int localEval = Int32.MinValue;
        foreach (var move in allValidMoves)
        {
            game.Board.Set(move.Key, move.p);
            localEval = -Evaluate(game, !color, depth-1, -beta, -alpha);
            alpha = Math.Max(alpha, localEval);
            game.Board.UndoLastMove();
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
        var piecesOfColor = game.Board.GetAll(color);
        
        // Check if the queen is surrounded
        var queen = game.Board.GetPiece(color, 'Q', 1);
        if (queen.Position is not null)
        {
            var surroundingPieces = MovementUtilities.GetSurroundingPositions(queen.Position)
                .Select(it => game.Board.Get(it)).Where(it => it is not null);
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