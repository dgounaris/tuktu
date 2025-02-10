using Hive.Pieces;

namespace Hive.Tests.Pieces;

public class SpiderTests
{
    [Fact]
    public void GetValidMovesOnEmptyBoardSucceeds()
    {
        var board = new Board();
        var piece = new Spider { Color = true, PieceNumber = 1 };

        var moves = board.GetPiece(true, 'S', 1).GetValidMoves(board).ToList();
        
        Assert.Single(moves);
        Assert.Null(moves[0].PreviousPosition);
        Assert.Equal(new Position(0, 0), moves[0].NewPosition);
    }

    [Theory]
    [InlineData(":wwQ+0-2bA2+0-3wA2+0-4wS1+1-4bA1-1-1bB1-1-2wB2-1-3bB2-2+0wB1-2-1bA3-2-2wA1-3+0***@bG*@bQ**@bS*@wA***@wG*@wS", 2)]
    [InlineData(":wwQ+0-2wS1+1-3bA2+2-4bA1+3-4wA2+3-5bB1-1-2wB2-1-3bB2-2+0wB1-2-1bA3-2-2wA1-3+0***@bG*@bQ**@bS*@wA***@wG*@wS", 0)]
    [InlineData(":wwB1bB2+0-1wQ+0-2wA2+0-4wS1+1-3bA2+1-4bA1+2-4bB1-1-2wB2-1-3wA1-2-1bA3-2-2***@bG*@bQ**@bS*@wA***@wG*@wS", 2)]
    [InlineData(":wwB1bB2+0-1wQ+0-2bG1+0-4bA2+1-3wA2+1-4bB1+2-2bA1+2-3wS1-1+0wB2-1-3wA1-2-1bA3-2-2**@bG*@bQ**@bS*@wA***@wG*@wS", 2)]
    [InlineData(":wwB1bB2+0-1bG1+0-4wQ+1-2bA2+1-3wA2+1-4bB1+2-2bA1+2-3wS1-1+0wB2-1-3wA1-2-1bA3-2-2**@bG*@bQ**@bS*@wA***@wG*@wS", 3)]
    [InlineData(":wwB1bB2+0-1bG1+0-4wQ+1-2bA2+1-3wA2+1-4bA1+2-3bB1wS1-1+0wB2-1-3wA1-2-1bA3-2-2**@bG*@bQ**@bS*@wA***@wG*@wS", 0)]
    [InlineData(":wwB1+0-1wQ+0-2wB2+1-3wA2+1-4wS1+1-5bA2+2-3bA1-1-1bB1-1-2bB2-1-4bA3-2-2wA1-2-3***@bG*@bQ**@bS*@wA***@wG*@wS", 4)]
    public void GetValidMovesOnBoardSucceeds(string notation, int validMoves)
    {
        var board = new Board();
        board.LoadFromNotation(notation);
        var piece = board.GetPiece(true, 'S', 1);
        var positionBefore = piece.Position;

        var moves = piece.GetValidMoves(board).ToList();
        
        Assert.Equal(validMoves, moves.Count);
        Assert.Equal(positionBefore, piece.Position);
    }
}