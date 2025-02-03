using Hive.Heuristics;

namespace Hive.Tests.Heuristics;

public class PositionEvaluatorTests
{
    [Theory]
    [InlineData(":wwB1wA1+0+0wS1+0+1wQ+0-1bQ+1+0wA3+1+1wG1+1-1bS1+1-2bG1+2+0wB2+2-2wG2+2-3bB1+3-2bB2+3-3wA2+4-2***@bA**@bG*@bS*@wG*@wS", true, "wG2")]
    [InlineData(":wwB1wA1+0+0wS1+0+1bG1+0+2wQ+0-1wG2+0-2bQ+1+0wG1+1-1bB1+1-2bB2+1-3bS1+1-4wB2+2-2wA3-1+0wA2-1+1***@bA**@bG*@bS*@wG*@wS", false, "bS1")]
    public void EvaluateFindsWinIn1(string notation, bool color, string piece)
    {
        var evaluator = new PositionEvaluator();
        var board = new Board();
        board.LoadFromNotation(notation);

        var evaluation = evaluator.Evaluate(board, color, 2);
        
        Assert.Equal(piece, evaluation.First().Piece.Print());
    }
    
    [Theory]
    [InlineData(":wbA2+0-1bB1bB2wQ+0-2bA1+0-3bQ+0-4bG2+1-4wA3+2-2bS1+2-3wA2+2-4wG2+3-4wA1-1-1bG1-1-2wB2-1-4wG1-2+0wB1-2-1*@bA*@bG*@bS*@wG**@wS", false)]
    public void EvaluateFindsWinAtDepth(string notation, bool color)
    {
        var evaluator = new PositionEvaluator();
        var board = new Board();
        board.LoadFromNotation(notation);

        var evaluation = evaluator.Evaluate(board, color, 4);
        
        Assert.Equal(int.MaxValue, evaluation.First().Score);
    }
}