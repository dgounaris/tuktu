using Hive.Heuristics;

namespace Hive.Tests;

public class PositionEvaluatorTests
{
    [Theory]
    [InlineData(":wwB1wA1+0+0wS1+0+1wQ+0-1bQ+1+0wA3+1+1wG1+1-1bS1+1-2bG1+2+0wB2+2-2wG2+2-3bB1+3-2bB2+3-3wA2+4-2***@bA**@bG*@bS*@wG*@wS", "wG2")]
    public void EvaluateFindsWinIn1(string notation, string piece)
    {
        var evaluator = new PositionEvaluator();
        var game = new Game();
        game.Board.LoadFromNotation(notation);

        var evaluation = evaluator.Evaluate(game, true, 2);
        
        Assert.Equal(evaluation.First().Piece.Print(), piece);
    }
}