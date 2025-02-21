using Hive.Heuristics;
using Hive.Movement;
using Hive.Pieces;

namespace Hive.Commands;

public class BestMoveCommandHandler : ICommandHandler
{
    public string CommandString { get; } = "bestmove";
    private NaivePositionEvaluator positionEvaluator = new NaivePositionEvaluator();

    public void Handle(Game game, string command)
    {
        Move bestMove;
        if (command.Split(' ')[1].Equals("time"))
        {
            bestMove = positionEvaluator.Evaluate(game, TimeSpan.Zero);
        }
        else if (command.Split(' ')[1].Equals("depth"))
        {
            bestMove = positionEvaluator.Evaluate(game, 2);
        }
        else
        {
            throw new NotImplementedException($"Unknown command: {command}");
        }
        
        Console.WriteLine($"{bestMove.Piece.Print()} {PieceMoveParsingUtilities.PositionToMove(game.Board, bestMove.NewPosition)}".Trim());
    }
}