using Hive.Pieces;

namespace Hive.Tests.Pieces;

public class GrasshopperTests
{
    [Fact]
    public void GetValidMovesOnEmptyBoardSucceeds()
    {
        var board = new Board();
        var piece = new Grasshopper { Color = true, PieceNumber = 1 };

        var moves = piece.GetValidMoves(board).ToList();
        
        Assert.Single(moves);
        Assert.Null(moves[0].PreviousPosition);
        Assert.Equal(new Position(0, 0), moves[0].NewPosition);
    }

    [Theory]
    [InlineData(":wbG1+0-1bA1+0-3wA1-1-1bB1-1-2wB1-2-1**@bA*@bB**@bG*@bQ**@bS**@wA*@wB***@wG*@wQ**@wS", 1)]
    [InlineData(":wbG2+0-1bG1+0-2bA1+0-3wA1-1-1bB1-1-2wB2wG1-2+0wB1-2-1wA2-3+1**@bA*@bB*@bG*@bQ**@bS*@wA**@wG*@wQ**@wS", 4)]
    [InlineData(":wbG2+0-1bG1+0-2bA1+0-3wG2+1-4bA2+2-3wA3+2-4bB1-1-2wB1-2-1wA1-2-2wG1-3+0wA2-3+1wB2-3-1*@bA*@bB*@bG*@bQ**@bS*@wG*@wQ**@wS", 0)]
    [InlineData(":wwB2+0+0bG2+0-1bG1+0-2bA1+0-3wB1+1-1wG2+1-4bA2+2-2wA1+2-3wA2+2-4wG1+3-3wA3-1-1bB1-1-2*@bA*@bB*@bG*@bQ**@bS*@wG*@wQ**@wS", 4)]
    [InlineData(":wwB2+0+0bG2+0-1bG1+0-2bA1+0-3bB1wB1+1-1wG2+1-4bA2+2-2wA1+2-3wA2+2-4wG1+3-3wA3+3-4*@bA*@bB*@bG*@bQ**@bS*@wG*@wQ**@wS", 2)]
    public void GetValidMovesOnBoardSucceeds(string notation, int validMoves)
    {
        var board = new Board();
        board.LoadFromNotation(notation);
        var piece = board.GetPiece(false, 'G', 1);
        var positionBefore = piece.Position;

        var moves = piece.GetValidMoves(board).ToList();
        
        Assert.Equal(validMoves, moves.Count);
        Assert.Equal(positionBefore, piece.Position);
    }
}