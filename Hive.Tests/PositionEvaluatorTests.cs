using Hive.Heuristics;

namespace Hive.Tests;

public class PositionEvaluatorTests
{
    [Theory]
    [InlineData(":wwB1wA1+0+0wS1+0+1wQ+0-1bQ+1+0wA3+1+1wG1+1-1bS1+1-2bG1+2+0wB2+2-2wG2+2-3bB1+3-2bB2+3-3wA2+4-2***@bA**@bG*@bS*@wG*@wS", true, "wG2")]
    [InlineData(":wwB1wA1+0+0wS1+0+1bG1+0+2wQ+0-1wG2+0-2bQ+1+0wG1+1-1bB1+1-2bB2+1-3bS1+1-4wB2+2-2wA3-1+0wA2-1+1***@bA**@bG*@bS*@wG*@wS", false, "bS1")]
    public void EvaluateFindsWinIn1(string notation, bool color, string piece)
    {
        var evaluator = new PositionEvaluator();
        var game = new Game();
        game.Board.LoadFromNotation(notation);

        var evaluation = evaluator.Evaluate(game, color, 2);
        
        Assert.Equal(piece, evaluation.First().Piece.Print());
    }
}