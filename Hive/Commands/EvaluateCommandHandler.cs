using Hive.Heuristics;

namespace Hive.Commands;

public class EvaluateCommandHandler : ICommandHandler
{
    public string CommandString { get; } = "e";
    private PositionEvaluator _positionEvaluator = new PositionEvaluator();

    public void Handle(Game game, string command)
    {
        try
        {
            var color = command[(command.IndexOf(' ') + 1)..command.LastIndexOf(' ')];
            var depth = int.Parse(command[command.LastIndexOf(' ')..]);
            Console.WriteLine("Calculating evaluation...");
            var result = _positionEvaluator.Evaluate(game.Board, color == "w", depth);
            if (color == "w")
            {
                result = result.OrderByDescending(it => it.Score).ToList();
            }
            else
            {
                result = result.OrderBy(it => it.Score).ToList();
            }
            Console.WriteLine("Best move:");
            Console.WriteLine($"{result.First().Piece.Print()} {result.First().Move} {result.First().Score}");
            Console.WriteLine("Also considering:");
            Console.WriteLine($"{result.Skip(1).First().Piece.Print()} {result.Skip(1).First().Move} {result.Skip(1).First().Score}");
            Console.WriteLine($"{result.Skip(2).First().Piece.Print()} {result.Skip(2).First().Move} {result.Skip(2).First().Score}");
        }
        catch (Exception e)
        {
            Console.WriteLine("Unknown error");
            Console.WriteLine(e.StackTrace);
        }
    }
}